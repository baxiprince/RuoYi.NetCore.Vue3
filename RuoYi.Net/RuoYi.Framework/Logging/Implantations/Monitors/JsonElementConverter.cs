using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace RuoYi.Framework.Logging;

/// <summary>
///   解决 JsonElement 问题
/// </summary>
[SuppressSniffer]
public class JsonElementConverter : Newtonsoft.Json.JsonConverter<JsonElement>
{
  /// <summary>
  ///   反序列化
  /// </summary>
  /// <param name="reader"></param>
  /// <param name="objectType"></param>
  /// <param name="existingValue"></param>
  /// <param name="hasExistingValue"></param>
  /// <param name="serializer"></param>
  /// <returns></returns>
  public override JsonElement ReadJson(JsonReader reader, Type objectType, JsonElement existingValue, bool hasExistingValue,
    JsonSerializer serializer)
  {
    var value = JToken.ReadFrom(reader).Value<string>();
    return (JsonElement)System.Text.Json.JsonSerializer.Deserialize<object>(value)!;
  }

  /// <summary>
  ///   序列化
  /// </summary>
  /// <param name="writer"></param>
  /// <param name="value"></param>
  /// <param name="serializer"></param>
  public override void WriteJson(JsonWriter writer, JsonElement value, JsonSerializer serializer)
  {
    var jsonSerializerOptions = new JsonSerializerOptions
    {
      ReferenceHandler = ReferenceHandler.IgnoreCycles,
      Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    //jsonSerializerOptions.Converters.Add(new SystemTextJsonLongToStringJsonConverter());
    //jsonSerializerOptions.Converters.Add(new SystemTextJsonNullableLongToStringJsonConverter());

    serializer.Serialize(writer, System.Text.Json.JsonSerializer.Serialize(value, jsonSerializerOptions));
  }
}
