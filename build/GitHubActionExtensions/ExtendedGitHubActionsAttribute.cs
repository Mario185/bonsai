using System.Collections.Generic;
using System.Linq;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.CI.GitHubActions.Configuration;

namespace GitHubActionExtensions;

public class ExtendedGitHubActionsAttribute : GitHubActionsAttribute
{
  public ExtendedGitHubActionsAttribute (string name, GitHubActionsImage image, params GitHubActionsImage[] images)
    : base (name, image, images)
  {
  }

  public bool OnReleasePublished { get; set; }

  protected override IEnumerable<GitHubActionsDetailedTrigger> GetTriggers ()
  {
    if (OnReleasePublished)
    {
      return base.GetTriggers().Append (new ReleaseGitHubActionsDetailedTrigger());
    }

    return base.GetTriggers();
  }
}