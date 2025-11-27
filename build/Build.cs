using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using GitHubActionExtensions;
using Newtonsoft.Json.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities;
using Serilog;

[ExtendedGitHubActions (
    "release",
    GitHubActionsImage.WindowsLatest,
    OnReleasePublished = true,
    WritePermissions = [GitHubActionsPermissions.Contents],
    EnableGitHubToken = true,
    InvokedTargets = [nameof(Publish)])]
[GitHubActions("ci",
  GitHubActionsImage.WindowsLatest, GitHubActionsImage.UbuntuLatest,
  On = [ GitHubActionsTrigger.Push],
  InvokedTargets = [nameof(RunTests)]
  )]
partial class Build : NukeBuild
{
  private static readonly AbsolutePath s_releaseOutputRoot = (RootDirectory / "_release").CreateOrCleanDirectory();
  private static readonly AbsolutePath s_artifactsPath = (RootDirectory / "_artifacts").CreateOrCleanDirectory();
  
  private static readonly AbsolutePath s_consoleToolsTestResultPath = (s_artifactsPath / "tests.zip");
  private static readonly AbsolutePath s_wingetManifestZip = (s_artifactsPath / "winget_manifest.zip");

  private static string s_gitHubBonsaiDownloadUrl;

  [Parameter ("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
  [SuppressMessage ("CodeQuality", "IDE0052:Remove unread private members", Justification = "Nuke")]
  // ReSharper disable once UnusedMember.Local
  readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

  [NuGetPackage("ReportGenerator", "ReportGenerator.exe")]
  private readonly Tool ReportGeneratorTool = default!;

  [Required]
  [Solution (GenerateProjects = true)]
  readonly Solution Solution;

  [SuppressMessage ("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
  // ReSharper disable once UnusedMember.Local
  Target Clean => d => d
      .Before (Restore)
      .Executes (() =>
      {
        GitTasks.Git ("clean -xfd -e *.vsidx -e build/* -e .nuke/temp/*");
      });

  Target Restore => d => d
      .Executes (() =>
      {
        DotNetTasks.DotNetRestore (s => s
            .SetForce (true)
            .SetForceEvaluate (true)
            .SetProjectFile (Solution)
        );
      });

  Target Compile => d => d
      .DependsOn (Restore)
      .Executes (() =>
      {
        DotNetTasks.DotNetBuild(s => s
          .SetConfiguration(Configuration)
          .SetProjectFile(Solution)
        );

        if (IsServerBuild)
        {
          Log.Information("GitHubEvent data:\r\n" + GitHubActions.Instance.GitHubEvent);
        }

      });

  Target RunTests => d => d
    .DependsOn(Compile)
    .ProceedAfterFailure()
    .Requires(() => ReportGeneratorTool)
    .Executes(() =>
    {
      var settings = new DotNetRunSettings()
        .EnableNoBuild()
        .EnableNoRestore()
        .SetConfiguration(Configuration)
        .AddCodeCoverageParameter()
        .SetProjectFile(Solution.Tests);
      try
      {
        DotNetTasks.DotNetRun(settings);
      }
      finally
      {
        CreateCoverageReport(Solution.Tests);

        var testResultsPath = Solution.Tests.GetOutputPath(Configuration) / "TestResults";
        testResultsPath.ZipTo(s_consoleToolsTestResultPath);

        var coverageXmlPath = testResultsPath / "coverage.xml";
        PrintCoverageSummary(coverageXmlPath);

        Log.Information(coverageXmlPath);

      }
    })
    .Produces(s_consoleToolsTestResultPath);

  Target Publish => d => d
      .DependsOn (RunTests)
      .OnlyWhenDynamic(() => SucceededTargets.Contains(RunTests), "Tests succeeded")
      .Triggers(CreateWingetManifest)
      .Executes (async () =>
      {
        string versionNumber = GetVersionNumber();

        DotNetTasks.DotNetPublish (c => c
          .SetProject (Solution.bonsai)
            .SetConfiguration (Configuration)
            .SetOutput (s_releaseOutputRoot)  
            //.EnablePublishSingleFile()
            .SetProperty("PublishAot", true)
            .SetProperty ("FileVersion", versionNumber)
            .SetProperty ("AssemblyVersion", versionNumber)
            .SetProperty ("InformationalVersion", versionNumber)
        );

        if (IsServerBuild)
        {
          string assetsUrl = GitHubActions.Instance.GitHubEvent["release"]!["upload_url"]!.ToString();
          assetsUrl = assetsUrl.Split ('{')[0] + "?name=bonsai.exe";

          HttpClient client = new();
          client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", GitHubActions.Instance.Token);
          client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/vnd.github+json"));
          client.DefaultRequestHeaders.Add ("User-Agent", "bonsai");
          await using (FileStream zipStream = new(s_releaseOutputRoot / "bonsai.exe", FileMode.Open))
          {
            StreamContent content = new(zipStream);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse ("application/zip");
            HttpResponseMessage response = await client.PostAsync (assetsUrl, content);
            Log.Information ("Upload response: " + response.StatusCode);

            var uploadResponseMessage = await response.Content.ReadAsStringAsync();
            Log.Information ("Upload message:" + uploadResponseMessage );

            var responseJsonObject = JObject.Parse(uploadResponseMessage);
            s_gitHubBonsaiDownloadUrl = responseJsonObject["browser_download_url"].ToString();
            
            if (!response.IsSuccessStatusCode)
            {
              Log.Error ("Upload failed.");
              throw new Exception ("Uploading assets failed.");
            }
          }
        }
      });

  Target CreateWingetManifest => d => d
    .OnlyWhenDynamic(() => SucceededTargets.Contains(Publish), "Publish succeeds")
    .OnlyWhenStatic(() => IsServerBuild, "Is server build")
    .Executes(async () =>
    {
      var wingetCreateDownloadUrl = "https://aka.ms/wingetcreate/latest";
      Log.Information($"Downloading wingetcreate from {wingetCreateDownloadUrl}");
      var wingetcreateToolPath = TemporaryDirectory / "wingetcreate.exe";
      using (var client = new HttpClient())
      {
        var result = await client.GetAsync("https://aka.ms/wingetcreate/latest");
        using (var downloadStream = result.Content.ReadAsStream())
        using (var fileStream = new FileStream(wingetcreateToolPath, FileMode.Create))
        {
          downloadStream.CopyTo(fileStream);
        }
      }

      Log.Information($"Download finished.");

      var versionNumber = GetVersionNumber();
      AbsolutePath wingetManifestOutputPath = s_artifactsPath / "winget";
      var wingetArgs = $"update donmar.bonsai -u {s_gitHubBonsaiDownloadUrl} -v {versionNumber} -o \"{wingetManifestOutputPath}\"";
      var process = ProcessTasks.StartProcess(wingetcreateToolPath, wingetArgs);
      process.AssertZeroExitCode();

      (wingetManifestOutputPath / "manifests" / "d" / "donmar" / "bonsai").ZipTo(s_wingetManifestZip);
    })
    .Produces(s_wingetManifestZip);


  static string GetVersionNumber()
  {
    IReadOnlyCollection<Output> result = GitTasks.Git("tag --points-at HEAD").EnsureOnlyStd();

    string versionNumber = "0.0.0";
    if (IsServerBuild)
    {
      Match[] matches = result.Select(r => ReleaseVersionTagRegex().Match(r.Text)).Where(m => m.Success).ToArray();

      if (matches.Length == 0)
      {
        throw new Exception("No release version tag found. Create a tag in this format \"release_1.2.3\"");
      }

      if (matches.Length > 1)
      {
        throw new Exception("Multiple release version tags found. Tags: " + string.Join(", ", matches.Select(m => m.Value.ToString())));
      }

      versionNumber = matches[0].Groups["version"].Value;
    }

    return versionNumber;
  }

  static void PrintCoverageSummary(AbsolutePath coverageXmlPath)
  {
    XDocument coverageXml = XDocument.Load(coverageXmlPath);

    List<CoverageResult> coverageResults = new();

    foreach (var module in coverageXml.XPathSelectElements("results/modules/module"))
    {
      var name = module.Attribute("name")!.Value;
      var blockCoverage = decimal.Parse(module.Attribute("block_coverage")!.Value, CultureInfo.InvariantCulture);
      var lineCoverage = decimal.Parse(module.Attribute("line_coverage")!.Value, CultureInfo.InvariantCulture);

      coverageResults.Add(new CoverageResult(name, blockCoverage, lineCoverage));
    }

    StringBuilder coverageMarkdown = new();

    coverageMarkdown.AppendLine("# Test coverage");
    coverageMarkdown.AppendLine("|Module|Block coverage|Line coverage");
    coverageMarkdown.AppendLine("|:-|-:|-:|");

    foreach (var result in coverageResults)
    {
      coverageMarkdown.AppendLine($"|{result.Module}|{GetIcon(result.BlockCoverage)} {result.BlockCoverageText} %|{GetIcon(result.LineCoverage)} {result.LineCoverageText} %|");
    }

    if (Environment.GetEnvironmentVariable("GITHUB_STEP_SUMMARY") is { } summaryPath)
    {
      File.WriteAllText(summaryPath, coverageMarkdown.ToString());
    }
    else
    {

      File.WriteAllText(s_artifactsPath / "testcoverage.md", coverageMarkdown.ToString());
    }
  }

  private static string GetIcon(decimal value)
  {
    if (value < 100)
      return "\u26a0\ufe0f";

    return "\u2705";
  }

  private void CreateCoverageReport(Project project)
  {
    Log.Information($"Starting {nameof(ReportGeneratorTool)} for project {project.Name} ...");
    ReportGeneratorTool.Invoke("-reports:TestResults\\coverage.xml -targetdir:TestResults\\coveragereport", project.GetOutputPath(Configuration));
    Log.Information($"{nameof(ReportGeneratorTool)} for project {project.Name} finished.");
  }

  public static int Main () => Execute<Build> (x => x.Compile);

  [GeneratedRegex ("^(release_)(?<version>[0-9]{1,}[.][0-9]{1,}[.][0-9]{1,})$")]
  private static partial Regex ReleaseVersionTagRegex ();
}

public record CoverageResult(string Module, decimal BlockCoverage, decimal LineCoverage)
{
  public string BlockCoverageText => BlockCoverage.ToString(CultureInfo.InvariantCulture);
  public string LineCoverageText => LineCoverage.ToString(CultureInfo.InvariantCulture);
}