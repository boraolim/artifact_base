using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Bankaool.Core.Shared.Utils;

using MainConstantsCore = Bankaool.Core.Shared.Constants.MainConstants;
using RegexConstantsCore = Bankaool.Core.Shared.Constants.RegexConstants;
using MessageConstantsCore = Bankaool.Core.Shared.Constants.MessageConstants;

namespace Bankaool.Core.Shared.Converters
{
    public class UuidV4JsonConverter : JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(reader.GetString()) || !Functions.IsValidGuidV4(reader.GetString()))
                throw new JsonException(string.Format(MessageConstantsCore.MSG_FAIL_TO_UUID_V4, reader.GetString()));

            return Guid.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }
}
