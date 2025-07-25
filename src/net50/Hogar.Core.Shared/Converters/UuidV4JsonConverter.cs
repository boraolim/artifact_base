﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Hogar.Core.Shared.Utils;

using MainConstantsCore = Hogar.Core.Shared.Constants.MainConstants;
using RegexConstantsCore = Hogar.Core.Shared.Constants.RegexConstants;
using MessageConstantsCore = Hogar.Core.Shared.Constants.MessageConstants;

namespace Hogar.Core.Shared.Converters
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
