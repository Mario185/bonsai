using bonsai.Data;
using bonsai.JsonConverter;
using System;
using System.Text.Json.Serialization;

namespace bonsai.JsonSerialization
{
  [JsonSourceGenerationOptions(WriteIndented = true, 
      PropertyNameCaseInsensitive = false, 
      PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
      Converters = [typeof(ColorJsonConverter), typeof(ActionTypeEnumConverter), typeof (ConsoleKeyEnumConverter), typeof(ConsoleModifiersEnumConverter)])]
  [JsonSerializable(typeof(Database))]
  internal partial class DatabaseGenerationContext : JsonSerializerContext
  {

  }
}