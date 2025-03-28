using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using GitHubActionExtensions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Serilog;

[ExtendedGitHubActions (
    "release",
    GitHubActionsImage.WindowsLatest,
    OnReleasePublished = true,
    WritePermissions = [GitHubActionsPermissions.Contents],
    EnableGitHubToken = true,
    InvokedTargets = [nameof(Publish)])]
[GitHubActions("ci",
  GitHubActionsImage.WindowsLatest,
  On = [ GitHubActionsTrigger.Push],
  InvokedTargets = [nameof(RunTests)]
  )]
partial class Build : NukeBuild
{
  private static readonly AbsolutePath s_releaseOutputRoot = (RootDirectory / "_release").CreateOrCleanDirectory();
  private static readonly AbsolutePath s_artifactsPath = (RootDirectory / "_artifacts").CreateOrCleanDirectory();
  
  
  private static readonly AbsolutePath s_consoleToolsTestResultPath = (s_artifactsPath / "consoleTools_Tests.zip");



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
      });


  Target RunTests => d => d
    .DependsOn(Compile)
    .DependsOn(RunCluiTests, RunConsoleToolsTests);

  Target RunCluiTests => d => d
    .DependsOn(Compile)
    .ProceedAfterFailure()
    .Executes(() =>
    {
      var settings = new DotNetTestSettings()
        .EnableNoBuild()
        .EnableNoRestore()
        .SetConfiguration(Configuration)
        //.AddLoggers("console;verbosity=detailed")
        .SetProjectFile(Solution.clui_Tests);

      DotNetTasks.DotNetTest(settings);
    });

  Target RunConsoleToolsTests => d => d
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
        .SetProjectFile(Solution.consoleTools_Tests);

      DotNetTasks.DotNetRun(settings);

      CreateCoverageReportOnDemand(Solution.consoleTools_Tests);

      var testResultsPath = Solution.consoleTools_Tests.GetOutputPath(Configuration) / "TestResults";
      testResultsPath.ZipTo(s_consoleToolsTestResultPath);

    })
    .Produces(s_consoleToolsTestResultPath);

  private void CreateCoverageReportOnDemand(Project project)
  {
    Log.Information($"Starting {nameof(ReportGeneratorTool)} for project {project.Name} ...");
    ReportGeneratorTool.Invoke("-reports:TestResults\\coverage.xml -targetdir:TestResults\\coveragereport", project.GetOutputPath(Configuration));
    Log.Information($"{nameof(ReportGeneratorTool)} for project {project.Name} finished.");
  }

  // ReSharper disable once UnusedMember.Local

  Target Publish => d => d
      .DependsOn (RunTests)
      .Executes (async () =>
      {
        IReadOnlyCollection<Output> result = GitTasks.Git ("tag --points-at HEAD").EnsureOnlyStd();

        string versionNumber = "0.0.1";
        if (IsServerBuild)
        {
          Match[] matches = result.Select (r => ReleaseVersionTagRegex().Match (r.Text)).Where (m => m.Success).ToArray();

          if (matches.Length == 0)
          {
            throw new Exception ("No release version tag found. Create a tag in this format \"release_1.2.3\"");
          }

          if (matches.Length > 1)
          {
            throw new Exception ("Multiple release version tags found. Tags: " + string.Join (", ", matches.Select (m => m.Value.ToString())));
          }

          versionNumber = matches[0].Groups["version"].Value;
        }

        DotNetTasks.DotNetPublish (c => c
          .SetProject (Solution.bonsai)
            .SetConfiguration (Configuration)
            .SetPublishSingleFile (true)
            .SetSelfContained (false)
            .SetOutput (s_releaseOutputRoot)
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
            Log.Information ("Upload message:" + await response.Content.ReadAsStringAsync());

            if (!response.IsSuccessStatusCode)
            {
              Log.Error ("Upload failed.");
              throw new Exception ("Uploading assets failed.");
            }
          }
        }
      });
  public static int Main () => Execute<Build> (x => x.Compile);

  [GeneratedRegex ("^(release_)(?<version>[0-9]{1,}[.][0-9]{1,}[.][0-9]{1,})$")]
  private static partial Regex ReleaseVersionTagRegex ();
}