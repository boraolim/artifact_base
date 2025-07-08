using System;
using System.Text.Json;

using Xunit;

using Hogar.Core.Shared.Converters;

namespace Hogar.Core.Shared.Tests
{
    public class EnumJsonConverterTests
    {
        [Fact]
        public void Write_ShouldSerializeEnumAsString()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<DayOfWeek>());

            var json = JsonSerializer.Serialize(DayOfWeek.Friday, options);

            Assert.Equal("\"Friday\"", json);
        }

        [Fact]
        public void Read_ShouldDeserializeValidEnumValue()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<DayOfWeek>());

            var json = "\"Monday\"";

            var result = JsonSerializer.Deserialize<DayOfWeek>(json, options);

            Assert.Equal(DayOfWeek.Monday, result);
        }

        [Fact]
        public void Read_ShouldBeCaseInsensitive()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<DayOfWeek>());

            var json = "\"sunday\"";

            var result = JsonSerializer.Deserialize<DayOfWeek>(json, options);

            Assert.Equal(DayOfWeek.Sunday, result);
        }

        [Fact]
        public void Read_ShouldThrowJsonException_OnInvalidValue()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<DayOfWeek>());

            var json = "\"InvalidValue\"";

            var exception = Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<DayOfWeek>(json, options)
            );

            Assert.Contains("InvalidValue", exception.Message);
        }

        [Fact]
        public void SerializeDeserialize_ShouldWorkCorrectly()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new EnumJsonConverter<DayOfWeek>());

            var originalValue = DayOfWeek.Wednesday;
            var json = JsonSerializer.Serialize(originalValue, options);
            var result = JsonSerializer.Deserialize<DayOfWeek>(json, options);

            Assert.Equal(originalValue, result);
        }

    }
}
