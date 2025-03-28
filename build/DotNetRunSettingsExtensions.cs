using Nuke.Common.Tools.DotNet;

public static class DotNetRunSettingsExtensions
{
  public static DotNetRunSettings AddCodeCoverageParameter(this DotNetRunSettings settings)
  {
    return settings.AddApplicationArguments("--coverage", "--coverage-output", "coverage.xml", "--coverage-output-format", "xml");
  }
}