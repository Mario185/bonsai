using System.Text.Json.Serialization;

namespace bonsai.CommandHandling
{
  public class DirectoryCommand : Command
  {
    private readonly bool _isDefault;

    public DirectoryCommand CloneForExecution(string path)
    {
      return new DirectoryCommand(Action.Replace("[path]", path), DisplayName);
    }

    [JsonConstructor]
    public DirectoryCommand()
    {

    }

    public DirectoryCommand(string action, string displayName, bool isDefault = false)
      : base(action, displayName)
    {
      _isDefault = isDefault;
    }

    public override string GetExecutableAction()
    {
      return $"d{(_isDefault ? "y" : "n")}" + Action;
    }
  }
}