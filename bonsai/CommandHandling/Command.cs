using System.Text.Json.Serialization;
using clui.Controls;
using consoleTools;

namespace bonsai.CommandHandling
{
  public abstract class Command : IListItem
  {
    [JsonInclude]
    public string Action { get; private set; }

    [JsonInclude]
    public string DisplayName { get; private set; }

    [JsonConstructor]
    protected Command()
    {
      Action = string.Empty;
      DisplayName = string.Empty;
    }

    protected Command(string action, string displayName)
    {
      Action = action;
      DisplayName = displayName;
    }

    public abstract string GetExecutableAction();
    public void Write(ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      writer.WriteTruncated(DisplayName, 0, maxLength);
    }
  }
}