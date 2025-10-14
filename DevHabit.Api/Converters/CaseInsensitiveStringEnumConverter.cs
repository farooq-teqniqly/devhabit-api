using System.Text.Json;
using System.Text.Json.Serialization;

namespace DevHabit.Api.Converters;

public class CaseInsensitiveStringEnumConverter : JsonConverterFactory
{
  public override bool CanConvert(Type typeToConvert)
  {
    ArgumentNullException.ThrowIfNull(typeToConvert);

    return typeToConvert.IsEnum;
  }

  public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
  {
    ArgumentNullException.ThrowIfNull(typeToConvert);

    var converterType = typeof(CaseInsensitiveEnumConverter<>).MakeGenericType(typeToConvert);

    return (JsonConverter)(
      Activator.CreateInstance(converterType)
      ?? throw new InvalidOperationException(
        $"Failed to create converter for type {typeToConvert.Name}"
      )
    );
  }

  private sealed class CaseInsensitiveEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
  {
    public override T Read(
      ref Utf8JsonReader reader,
      Type typeToConvert,
      JsonSerializerOptions options
    )
    {
      switch (reader.TokenType)
      {
        case JsonTokenType.String:
        {
          var stringValue = reader.GetString();

          if (
            stringValue != null
            && Enum.TryParse<T>(stringValue, ignoreCase: true, out var result)
          )
          {
            return result;
          }

          throw new JsonException(
            $"Unable to convert \"{stringValue}\" to enum type {typeof(T).Name}"
          );
        }
        case JsonTokenType.Number:
        {
          var intValue = reader.GetInt32();

          if (Enum.IsDefined(typeof(T), intValue))
          {
            return (T)Enum.ToObject(typeof(T), intValue);
          }

          throw new JsonException($"Unable to convert {intValue} to enum type {typeof(T).Name}");
        }
        default:
          throw new JsonException($"Unexpected token {reader.TokenType} when parsing enum");
      }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
      writer.WriteStringValue(value.ToString());
    }
  }
}
