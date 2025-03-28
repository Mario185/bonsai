using Nuke.Common.CI.GitHubActions.Configuration;
using Nuke.Common.Utilities;

namespace GitHubActionExtensions;

public class ReleaseGitHubActionsDetailedTrigger : GitHubActionsDetailedTrigger
{
  public override void Write (CustomFileWriter writer)
  {
    writer.WriteLine ("release:");
    using (writer.Indent())
    {
      writer.WriteLine ("types: [published]");
    }
  }
}