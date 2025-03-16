using BreakingBank.Models.SaveGame;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BreakingBank.JsonConverters
{
    public class DirtyFieldJsonConverter<T> : JsonConverter<DirtyField<T>>
    {
        public override DirtyField<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T? value = JsonSerializer.Deserialize<T>(ref reader, options);
            return value != null ? new DirtyField<T> { Value = value } : null;
        }

        public override void Write(Utf8JsonWriter writer, DirtyField<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }

}
