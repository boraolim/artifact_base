using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using MessageConstantsCore = Utilities.Core.Shared.Constants.MessageConstants;

namespace Utilities.Core.Shared.Converters
{
    public class EnumJsonConverter<T> : JsonConverter<T> where T : struct, System.Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (System.Enum.TryParse(reader.GetString(), true, out T value))
                return value;

            throw new JsonException(string.Format(MessageConstantsCore.MSG_FAIL_TO_ENUM, reader.GetString(), typeof(T).Name.Trim()));
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }
}
