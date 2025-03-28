using Nuke.Common.IO;
using Nuke.Common;
using Serilog;
using System;
using Nuke.Common.ProjectModel;

public static class ProjectExtensions
{
  public static AbsolutePath GetOutputFilePath(this Project project, Configuration configuration) => project.GetMSBuildProject(configuration: configuration).GetOutputFilePath();

  public static AbsolutePath GetOutputPath(this Project project, Configuration configuration) => project.GetMSBuildProject(configuration: configuration).GetOutputPath();

  public static AbsolutePath GetOutputPath(this Microsoft.Build.Evaluation.Project project)
  {
    ArgumentNullException.ThrowIfNull(project);

    try
    {
      var outputPath = (AbsolutePath)project.DirectoryPath / project.GetProperty("OutputPath").EvaluatedValue;
      Assert.DirectoryExists(outputPath);
      return outputPath;
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Error while extracting OutputPath for {ProjectPath}", project.FullPath);
      throw;
    }
  }

  public static AbsolutePath GetOutputFilePath(this Microsoft.Build.Evaluation.Project project)
  {
    ArgumentNullException.ThrowIfNull(project);

    try
    {
      var assembly = project.GetOutputPath() / project.GetProperty("TargetFileName").EvaluatedValue;
      Assert.FileExists(assembly);
      return assembly;
    }
    catch (Exception ex)
    {
      Log.Error(ex, "Error while extracting OutputPathFilePath for {ProjectPath}", project.FullPath);
      throw;
    }
  }

}
