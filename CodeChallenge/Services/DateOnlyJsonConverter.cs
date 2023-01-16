using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeChallenge.Services
{
    // Extends JSON serialization and deserailization to
    // support the DateOnly date type.
    public sealed class DateOnlyJsonConverter: JsonConverter<DateOnly>
    {
        // Deserialize method
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateOnly.FromDateTime(reader.GetDateTime());
        }

        // Serialize method
        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            var isoDate = value.ToString("O");
            writer.WriteStringValue(isoDate);
        }
    }
}
