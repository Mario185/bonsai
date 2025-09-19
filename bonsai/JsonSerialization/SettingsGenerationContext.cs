using System.Drawing;
using System.Text.Json.Serialization;
using bonsai.JsonConverter;

namespace bonsai.JsonSerialization
{
  [JsonSourceGenerationOptions(WriteIndented = true, 
      PropertyNameCaseInsensitive = false, 
      PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
      Converters = [typeof(ColorJsonConverter), typeof(ActionTypeEnumConverter), typeof (ConsoleKeyEnumConverter), typeof(ConsoleModifiersEnumConverter)])]
  [JsonSerializable(typeof(Settings))]
  internal partial class SettingsGenerationContext : JsonSerializerContext
  {
  }
}