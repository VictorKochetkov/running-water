using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RunningWater.Converters
{
    public class DateTimeUnixTimeConverter : JsonConverter<DateTimeOffset>
    {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => TimeZoneInfo.ConvertTime(DateTimeOffset.FromUnixTimeSeconds(reader.GetInt32()), TimeZoneInfo.Local);

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
