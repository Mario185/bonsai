using System;

namespace bonsai.JsonConverter
{
  internal sealed class ConsoleModifiersEnumConverter : EnumJsonConverter<ConsoleModifiers>
  {
    public ConsoleModifiersEnumConverter ()
        : base(ConsoleModifiers.None)
    {
    }
  }
}