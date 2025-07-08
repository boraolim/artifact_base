using System;
using System.Text.Json;
using System.Globalization;
using System.Text.Json.Serialization;

using FormatConstantsCore = Hogar.Core.Shared.Constants.FormatConstants;

namespace Hogar.Core.Shared.Converters
{
    public class DateTimeJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
             DateTime.ParseExact(reader.GetString(), FormatConstantsCore.FMT_DATE_ISO_8601_V1, CultureInfo.InvariantCulture);

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString(FormatConstantsCore.FMT_DATE_ISO_8601_V1));
    }
}
