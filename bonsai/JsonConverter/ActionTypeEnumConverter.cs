using System;

namespace bonsai.JsonConverter
{
  internal sealed class ActionTypeEnumConverter : EnumJsonConverter<ActionType>
  {
    public ActionTypeEnumConverter ()
        : base(ActionType.None)
    {
    }
  }
}