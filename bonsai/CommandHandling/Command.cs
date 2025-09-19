using System;
using System.Text.Json.Serialization;
using clui.Controls;
using consoleTools;

namespace bonsai.CommandHandling
{
  public abstract class Command : IListItem
  {
    [JsonConstructor]
    protected Command ()
    {
      Action = string.Empty;
      DisplayName = string.Empty;
    }

    protected Command (string action, string displayName)
    {
      Action = action;
      DisplayName = displayName;
    }

    [JsonInclude]
    public string Action { get; set; }

    [JsonInclude]
    public string DisplayName { get; set; }

    public void Write (ConsoleWriter writer, int maxLength, bool isFocusedItem)
    {
      writer.WriteTruncated (DisplayName, 0, maxLength);
    }

    public abstract string GetExecutableAction ();
  }
}