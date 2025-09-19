using System;
using System.Text.Json.Serialization;

namespace bonsai.CommandHandling
{
  public class FileCommand : Command
  {
    private readonly bool _isDefault;

    [JsonConstructor]
    public FileCommand ()
    {
      Extension = string.Empty;
    }

    public FileCommand (string action, string displayName, string extension, bool isDefault = false)
        : base (action, displayName)
    {
      _isDefault = isDefault;
      Extension = extension;
    }

    [JsonInclude]
    public string Extension { get; set; }

    public FileCommand CloneForExecution (string path)
    {
      return new FileCommand (Action.Replace ("[path]", path), DisplayName, Extension);
    }

    public override string GetExecutableAction ()
    {
      return $"f{(_isDefault ? "y" : "n")}" + Action;
    }
  }
}