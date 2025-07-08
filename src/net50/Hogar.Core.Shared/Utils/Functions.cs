using System;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

using Hogar.Core.Shared.Extensions;

using MainConstantsCore = Hogar.Core.Shared.Constants.MainConstants;
using RegexConstantsCore = Hogar.Core.Shared.Constants.RegexConstants;
using FormatConstantsCore = Hogar.Core.Shared.Constants.FormatConstants;
using MessageConstantsCore = Hogar.Core.Shared.Constants.MessageConstants;

namespace Hogar.Core.Shared.Utils
{
    public static class Functions
    {
        private static readonly ConcurrentDictionary<string, Regex> _regexCache = new();

        public static string FormatearObjectToJson(object Input)
        {
            return JsonSerializer.Serialize(Input,
                new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                });
        }

        public static T FormatearJsonStringToObject<T>(object InputObject) =>
            JsonSerializer.Deserialize<T>(Functions.FormatearObjectToJson(InputObject));

        public static object JsonToObject(string jsonString)
        {
            try
            {
                var document = JsonSerializer.Deserialize<JsonElement>(jsonString,
                    new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    });
                return ConvertJsonElement(document);
            }
            catch(JsonException ex)
            {
                throw new InvalidOperationException("El JSON no es válido.", ex);
            }
        }

        public static bool IsMatch(string inputValue, string patternRegex, double retryInMiliseconds = 0)
        {
            if(string.IsNullOrEmpty(inputValue) || string.IsNullOrEmpty(patternRegex))
                return false;

            double timeout = retryInMiliseconds <= 0 ?
                MainConstantsCore.CFG_DEFAULT_REGEX_TIMEOUT : retryInMiliseconds;

            var regex = _regexCache.GetOrAdd(patternRegex,
                pattern => new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(timeout)));
            return regex.IsMatch(inputValue);
        }

        public static string FormatTextException(Exception exception) =>
            (exception.InnerException.CheckIsNull()) ? ClearText(exception.Message) : string.Format(FormatConstantsCore.FMT_LEGEND_ERROR,
            ClearText(exception.Message), ClearText(exception.InnerException.Message));

        public static bool IsValidGuidV4(string input)
        {
            if(string.IsNullOrWhiteSpace(input)) return false;

            if(!Guid.TryParse(input, out var guid)) return false;

            var guidBytes = guid.ToByteArray();
            bool allZero = guidBytes.All(byteItem => byteItem == 0);
            bool allOne = guidBytes.All(byteItem => byteItem == 0xFF);

            return !allZero && !allOne && guid != Guid.Empty;
        }

        public static byte[] HexToString(string inputHex)
        {
            if(inputHex.Length % 2 != MainConstantsCore.CFG_ZERO)
                throw new ArgumentException(MessageConstantsCore.MSG_PAIR_EXACTLY);

            var bytesHex = new byte[inputHex.Length / 2];

            for(int i = MainConstantsCore.CFG_ZERO; i < bytesHex.Length; i++)
            {
                string byteValue = inputHex.Substring(i * 2, 2);
                bytesHex[i] = Convert.ToByte(byteValue, 16);
            }

            return bytesHex;
        }

        public static int Next(int min, int max)
        {
            if(min >= max)
                throw new ArgumentOutOfRangeException(nameof(min), "El valor mínimo debe ser menor que el máximo.");
            return RandomNumberGenerator.GetInt32(min, max);
        }

        #region "Métodos privados."

        private static string ClearText(string contentText, double retryInMiliseconds = 0)
        {
            if(string.IsNullOrWhiteSpace(contentText)) return string.Empty;

            double timeout = retryInMiliseconds <= 0 ?
                MainConstantsCore.CFG_DEFAULT_REGEX_TIMEOUT : retryInMiliseconds;

            var regex = _regexCache.GetOrAdd(RegexConstantsCore.RGX_SPACE_CLEAR,
               pattern => new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(timeout)));

            return regex.Replace(contentText, FormatConstantsCore.FMT_SPACE_BLANK).Trim();
        }

        private static object ConvertJsonElement(JsonElement element) =>
            element.ValueKind switch
            {
                JsonValueKind.Object => ConvertToDictionary(element),
                JsonValueKind.Array => ConvertToList(element),
                JsonValueKind.String => ConvertString(element.GetString()),
                JsonValueKind.Number => ConvertNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => element.GetRawText()
            };

        private static Dictionary<string, object> ConvertToDictionary(JsonElement element) =>
            element.EnumerateObject()
                   .ToDictionary(prop => prop.Name, prop => ConvertJsonElement(prop.Value));

        private static List<object> ConvertToList(JsonElement element) =>
            element.EnumerateArray()
                   .Select(ConvertJsonElement)
                   .ToList();

        private static object ConvertString(string? input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return string.Empty;

            if(bool.TryParse(input, out var boolVal))
                return boolVal;

            if(TryParseDateTime(input, out var dateTimeVal))
                return dateTimeVal;

            if(TryParseInteger(input, out var intVal))
                return intVal;

            if(TryParseCurrency(input, out var currencyVal))
                return currencyVal;

            return input!;
        }

        private static bool TryParseDateTime(string input, out DateTime result)
        {
            string[] dateFormats =
            {
                FormatConstantsCore.FMT_DATE_ISO_8601_V1,
                FormatConstantsCore.FMT_DATE_ISO_8601_V2,
                FormatConstantsCore.FMT_DATE_ISO_8601_V3,
                FormatConstantsCore.FMT_DATE_ISO_8601_V4
            };

            return DateTime.TryParseExact(input, dateFormats, CultureInfo.InvariantCulture,
                                          DateTimeStyles.AdjustToUniversal | DateTimeStyles.AllowWhiteSpaces, out result)
                   || DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result);
        }

        private static bool TryParseCurrency(string input, out decimal result)
        {
            var cleaned = input.Replace("$", "").Replace(",", "");
            return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out result);
        }

        private static bool TryParseInteger(string input, out object result)
        {
            result = null;

            if(string.IsNullOrWhiteSpace(input))
                return false;

            if(long.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal))
            {
                result = (longVal >= int.MinValue && longVal <= int.MaxValue) ? (object)(int)longVal : longVal;
                return true;
            }

            return false;
        }

        private static object ConvertNumber(JsonElement element)
        {
            if(element.TryGetInt32(out int intVal))
                return intVal;

            if(element.TryGetInt64(out long longVal))
                return longVal;

            return element.GetDouble();
        }

        #endregion
    }
}
