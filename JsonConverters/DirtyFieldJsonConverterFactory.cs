using BreakingBank.Models.SaveGame;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace BreakingBank.JsonConverters
{
    public class DirtyFieldJsonConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(DirtyField<>);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type valueType = typeToConvert.GetGenericArguments()[0]; // Generischen Typ `T` extrahieren
            Type converterType = typeof(DirtyFieldJsonConverter<>).MakeGenericType(valueType);
            return (JsonConverter?)Activator.CreateInstance(converterType);
        }
    }
}
