using System;

namespace bonsai.JsonConverter
{
  internal sealed class ConsoleKeyEnumConverter : EnumJsonConverter<ConsoleKey>
  {
    public ConsoleKeyEnumConverter ()
        : base(ConsoleKey.None)
    {
    }
  }
}