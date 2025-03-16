using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace bonsai.JsonConverter
{
  public class EnumJsonConverter<T> : JsonConverter<T>
    where T : struct, Enum
  {
    private readonly T _defaultValue;

    public EnumJsonConverter(T defaultValue)
    {
      _defaultValue = defaultValue;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      string? stringValue = reader.GetString();

      if (Enum.TryParse(stringValue, false, out T value))
      {
        return value;
      }

      return _defaultValue;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.ToString());
    }
  }
}