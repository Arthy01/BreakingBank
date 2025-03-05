using BreakingBank.Models.SaveGame;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BreakingBank.JsonConverters
{
    public class DirtyFieldJsonConverter<T> : JsonConverter<DirtyField<T>>
    {
        public override DirtyField<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Falls du deserialisieren willst, hier anpassen:
            return new DirtyField<T> { Value = JsonSerializer.Deserialize<T>(ref reader, options) };
        }

        public override void Write(Utf8JsonWriter writer, DirtyField<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}
