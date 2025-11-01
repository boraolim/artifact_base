namespace Utilities.Core.Shared.Utils;

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

    public static DateTime GetDateFromLinuxDateTime(long timeStamp) =>
        DateTimeOffset.FromUnixTimeSeconds(timeStamp).UtcDateTime;

    public static bool IsMatch(string inputValue, string patternRegex, double retryInMiliseconds = 0)
    {
        if(string.IsNullOrEmpty(inputValue) || string.IsNullOrEmpty(patternRegex))
            return false;

        double timeout = retryInMiliseconds <= 0 ?
            MainConstantsCore.CFG_DEFAULT_REGEX_TIMEOUT : retryInMiliseconds;

        var regex = _regexCache.GetOrAdd(patternRegex,
            pattern => new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(timeout)));
        return regex.IsMatch(inputValue);
    }

    public static string FormatTextException(Exception exception) =>
        GetExceptionDetails(exception);

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

    public static string TruncateText(string textToTruncate, int maxLength)
    {
        if(maxLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maxLength), "maxLength no puede ser negativo");

        if(string.IsNullOrEmpty(textToTruncate))
            return textToTruncate!;

        return textToTruncate.Length <= maxLength
            ? textToTruncate
            : textToTruncate.Substring(0, maxLength) + "...(truncated)";
    }

    public static IEnumerable<R> DataReaderMapToListAsync<R>(IDataReader drReader)
    {
        if(drReader.CheckIsNull())
            throw new ArgumentNullException(nameof(drReader));

        return DataReaderMapToListIteratorAsync<R>(drReader).ToList();
    }

    public static T? GetEnumValueFromDescription<T>(string description) where T : struct, System.Enum
    {
        foreach(var field in typeof(T).GetFields())
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if(!attribute.CheckIsNull() && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                return (T?)field.GetValue(null);
        }
        return null;
    }

    public static string GenerateRandomString(int sizeLength)
    {
        const string allowedChars = MainConstantsCore.CFG_ALPHA_COLLECTION;

        return new string(Enumerable
            .Range(0, sizeLength)
            .Select(_ => allowedChars[Random.Shared.Next(allowedChars.Length)])
            .ToArray());
    }

    public static string GenerateUniqueRandomString(int sizeLength)
    {
        const string allowedChars = MainConstantsCore.CFG_ALPHA_COLLECTION_V2;

        var random = new Random();

        // Ordenar aleatoriamente y tomar los primeros `sizeLength`
        return new string(allowedChars
            .OrderBy(_ => random.Next())
            .Take(sizeLength)
            .ToArray());
    }

    public static string GetEnvironmentConnectionString(string connectionUrl)
    {
        var dataBaseUri = new Uri(connectionUrl);
        var dbFolder = dataBaseUri.LocalPath.Trim('/');
        var userInfo = dataBaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);
        return $"Server={dataBaseUri.Host};Port={dataBaseUri.Port};Database={dbFolder};Uid={userInfo[0]};Pwd={userInfo[1]};";
    }

    public static IEnumerable<Type> GetExceptionsFromNamespace(Assembly assemblyObject, string namespaceValue)
    {
        return assemblyObject.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract &&
                   type.Namespace == namespaceValue && typeof(Exception).IsAssignableFrom(type));
    }

    public static (string HexHash, string Base64Hash) GenerateHash512(string input)
    {
        using(SHA512 sha512 = SHA512.Create())
        {
            byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
            string hexHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            string base64Hash = Convert.ToBase64String(hashBytes);
            return (hexHash, base64Hash);
        }
    }

    public static (string HexHash, string Base64Hash) GenerateHash256(string input)
    {
        using(SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            string base64Hash = Convert.ToBase64String(hashBytes);
            string hexHash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return (hexHash, base64Hash);
        }
    }


    #region "Métodos privados."

    private static string GetExceptionDetails(Exception exception)
    {
        if(exception.CheckIsNull()) return string.Empty;

        Guard.AgainstNullOrWhiteSpace(exception.Message, nameof(exception.Message));

        var trace = new StackTrace(exception, true);
        var frame = trace.GetFrame(0);

        var method = frame?.GetMethod();

        var moduleName = method?.DeclaringType?.Name ?? "Unknown";

        var declaringType = method?.DeclaringType?.FullName ?? "UnknownType";
        var methodName = $"{declaringType}.{method?.Name ?? "UnknownMethod"}";

        var innerMessage = exception.InnerException?.Message;

        // Validar mensajes internos si existen
        if(exception.InnerException != null)
            Guard.AgainstNullOrWhiteSpace(innerMessage, "InnerException.Message");

        var stackTrace = exception.StackTrace ?? string.Empty;

        var lineInfo = frame?.GetFileLineNumber() > 0 ? $" at line {frame.GetFileLineNumber()}" : string.Empty;

        return string.Format(FormatConstantsCore.FMT_FORMAT_EXCEPTION_FULL,
            (exception.InnerException is null) ? 
                ClearText(exception.Message) : string.Format(FormatConstantsCore.FMT_LEGEND_ERROR, ClearText(exception.Message), ClearText(exception.InnerException.Message)),
            moduleName, methodName, lineInfo, ClearText(stackTrace));
    }

    private static IEnumerable<T> DataReaderMapToListIteratorAsync<T>(System.Data.IDataReader drReader)
    {
        while(drReader.Read())
        {
            var item = Activator.CreateInstance<T>();

            foreach(var property in typeof(T).GetProperties())
            {
                if(!drReader.IsDBNull(drReader.GetOrdinal(property.Name)))
                {
                    var propertyValue = drReader[property.Name];
                    var convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    if(convertTo.IsEnum)
                    {
                        var method = typeof(Functions).GetMethod(nameof(GetEnumValueFromDescription))
                            .MakeGenericMethod(convertTo);
                        var hasDescription = method.Invoke(null, new object[] { propertyValue.ToString() });

                        property.SetValue(item, (!hasDescription.CheckIsNull() ? hasDescription :
                            System.Enum.Parse(convertTo, propertyValue.ToString())), null);
                    }
                    else if(convertTo == typeof(Guid))
                    {
                        Guid guidValue;

                        if(propertyValue is Guid g)
                        {
                            guidValue = g; // ya es Guid
                        }
                        else if(propertyValue is byte[] bytes && bytes.Length == 16)
                        {
                            guidValue = new Guid(bytes); // SQL Server / SQLite
                        }
                        else if(!Guid.TryParse(propertyValue.ToString(), out guidValue))
                        {
                            throw new FormatException(
                                $"El valor '{propertyValue}' no es un Guid válido para la propiedad '{property.Name}'.");
                        }

                        property.SetValue(item, guidValue, null);
                    }
                    else
                    {
                        property.SetValue(item, Convert.ChangeType(drReader[property.Name], convertTo), null);
                    }
                }
            }

            yield return item;
        }
    }

    public static bool IsValidPassword(string passwordValue)
    {
        var regex = new Regex(RegexConstantsCore.RGX_PASSWORD_PATTERN_V2);

        if(!regex.IsMatch(passwordValue))
            return false;

        for(int i = MainConstantsCore.CFG_ONE_PLUS; i < passwordValue.Length; i++)
        {
            if(passwordValue[i] == passwordValue[i - MainConstantsCore.CFG_ONE_PLUS])
                return false;
        }

        for(int i = MainConstantsCore.CFG_ONE_PLUS; i < passwordValue.Length; i++)
        {
            if(char.IsDigit(passwordValue[i]) && char.IsDigit(passwordValue[i - MainConstantsCore.CFG_ONE_PLUS]))
            {
                int diff = passwordValue[i] - passwordValue[i - MainConstantsCore.CFG_ONE_PLUS];
                if(Math.Abs(diff) == MainConstantsCore.CFG_ONE_PLUS)
                    return false;
            }
        }

        return true;
    }

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
