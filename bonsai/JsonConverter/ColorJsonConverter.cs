using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bonsai.JsonConverter
{
  public class ColorJsonConverter : JsonConverter<Color>
  {
    public override Color Read (ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
#pragma warning disable CS8604 // Possible null reference argument.
      return ColorTranslator.FromHtml (reader.GetString());
#pragma warning restore CS8604 // Possible null reference argument.
    }

    public override void Write (Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
    {
      writer.WriteStringValue (ColorTranslator.ToHtml (value));
    }
  }
}