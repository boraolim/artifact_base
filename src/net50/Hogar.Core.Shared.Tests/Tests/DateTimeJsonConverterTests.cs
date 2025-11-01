using System;
using System.Text.Json;

using Xunit;

using Utilities.Core.Shared.Converters;
using Utilities.Core.Shared.Tests.Constants;

namespace Utilities.Core.Shared.Tests
{
    public class DateTimeJsonConverterTests
    {
        private readonly DateTimeJsonConverter _converter = new DateTimeJsonConverter();
        private readonly JsonSerializerOptions _options;

        public DateTimeJsonConverterTests()
        {
            _options = new JsonSerializerOptions();
            _options.Converters.Add(_converter);
        }

        [Fact]
        public void Write_ShouldSerializeDateTime_AsFormattedString()
        {
            // Arrange
            var dateTime = new DateTime(2025, 7, 3, 15, 45, 30, DateTimeKind.Utc);

            // Act
            var json = JsonSerializer.Serialize(dateTime, _options);

            // Assert
            var expected = $"\"{dateTime.ToString(FormatConstantsCore.FMT_DATE_ISO_8601_V1)}\"";
            Assert.Equal(expected, json);
        }

        [Fact]
        public void Read_ShouldDeserializeFormattedString_ToDateTime()
        {
            // Arrange
            var expectedDate = new DateTime(2025, 12, 25, 18, 0, 0, DateTimeKind.Utc);
            var json = $"\"{expectedDate.ToString(FormatConstantsCore.FMT_DATE_ISO_8601_V1)}\"";

            // Act
            var result = JsonSerializer.Deserialize<DateTime>(json, _options);

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void Read_InvalidFormat_ShouldThrowFormatException()
        {
            // Arrange
            var invalidJson = "\"07/03/2025 15:30\"";  // Formato diferente al esperado
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeJsonConverter());

            // Act & Assert
            Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<DateTime>(invalidJson, options));
        }
    }
}
