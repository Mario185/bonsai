using System;
using System.Text.Json.Serialization;
using bonsai.JsonConverter;
using bonsai.Theme;

namespace bonsai.JsonSerialization
{
  [JsonSourceGenerationOptions(WriteIndented = true, 
      PropertyNameCaseInsensitive = false, 
      PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
      Converters = [typeof(ColorJsonConverter)])]
  [JsonSerializable(typeof(ThemeManger))]
  internal partial class ThemeManagerGenerationContext : JsonSerializerContext
  {

  }
}