using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Utilities;

public class ExtendedGitHubActionsAttribute : GitHubActionsAttribute
{
  public ExtendedGitHubActionsAttribute (string name, GitHubActionsImage image, params GitHubActionsImage[] images)
      : base(name, image, images)
  {
  }

  protected override IEnumerable<GitHubActionsDetailedTrigger> GetTriggers ()
  {
    if (OnReleasePublished)
      return base.GetTriggers().Append (new ReleaseGitHubActionsDetailedTrigger());

    return base.GetTriggers();
  }

  public bool OnReleasePublished { get; set; }
}

public class ReleaseGitHubActionsDetailedTrigger : GitHubActionsDetailedTrigger
{
  public override void Write (CustomFileWriter writer)
  {
    writer.WriteLine("release:");
    using (writer.Indent())
    {
      writer.WriteLine ("types: [published]");
    }
  }
}

[ExtendedGitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    On = [ GitHubActionsTrigger.WorkflowDispatch ],
    //OnReleasePublished = true,
    InvokedTargets = new[] { nameof(Publish) })]
partial class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    static readonly AbsolutePath ReleaseOutputRoot = (RootDirectory / "_release").CreateOrCleanDirectory();

    [Required]
    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [GeneratedRegex("^(release_)(?<version>[0-9]{1,}[.][0-9]{1,}[.][0-9]{1,})$")]
    private static partial Regex ReleaseVersionTagRegex();

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
          GitTasks.Git("clean -xfd -e *.vsidx -e build/* -e .nuke/temp/*");
        });

    Target Restore => _ => _
        .Executes(() =>
        {
          DotNetTasks.DotNetRestore(_ => _
            .SetForce(true)
            .SetForceEvaluate(true)
            .SetProjectFile(Solution)
          );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
          Console.WriteLine ("YEH");
        });


  // ReSharper disable once UnusedMember.Local
  [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Nuke Targets are only used implicit")]
 
  Target Publish => d => d
      //.DependsOn(Restore)
      //.Produces(ReleaseOutputRoot / "bonsai.exe" )
      .Executes(() =>
      {
        IReadOnlyCollection<Output> result = GitTasks.Git("tag --points-at HEAD").EnsureOnlyStd();

        string versionNumber = "0.0.1";
        //if (IsServerBuild)
        //{
        //  var matches = result.Select(r => ReleaseVersionTagRegex().Match(r.Text)).Where(m => m.Success).ToArray();
        
        //  if (matches.Length == 0)
        //    throw new Exception("No release version tag found. Create a tag in this format \"release_1.2.3\"");

        //  if (matches.Length > 1)
        //    throw new Exception("Multiple release version tags found. Tags: " + string.Join(", ", matches.Select(m => m.Value.ToString())));

        //  versionNumber = matches[0].Groups["version"].Value;
        //}

        Console.WriteLine ("GITHUBEVENT START");
        Console.WriteLine (GitHubActions.Instance.GitHubEvent.ToString());
        Console.WriteLine ("GITHUBEVENTEND");
        

        //DotNetTasks.DotNetPublish(c => c
        //  .SetProject(Solution.bonsai)
        //  .SetConfiguration(Configuration.Release)
        //  .SetPublishSingleFile(true)
        //  .SetSelfContained(false)
        //  .SetOutput(ReleaseOutputRoot)
        //  .SetProperty("FileVersion", versionNumber)
        //  .SetProperty("AssemblyVersion", versionNumber)
        //  .SetProperty("InformationalVersion", versionNumber)
        //);
      });

}
