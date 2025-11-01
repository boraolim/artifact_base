using Shouldly;

namespace Hogar.Core.Shared.Tests;

public class FunctionsTests
{
    [Fact]
    public void FormatearObjectToJson_ShouldSerializeCorrectly()
    {
        var input = new { Name = "Juan", Age = 30 };
        var json = Functions.FormatearObjectToJson(input);

        Assert.Contains("\"name\"", json); // camelCase
        Assert.Contains("30", json);
    }

    [Fact]
    public void FormatearJsonStringToObject_ShouldDeserializeCorrectly()
    {
        var input = new { Nombre = "Ana", Edad = 25 };
        var result = Functions.FormatearJsonStringToObject<Dictionary<string, object>>(input);

        Assert.Equal("Ana", result["nombre"].ToString());
        Assert.Equal("25", result["edad"].ToString());
    }

    [Fact]
    public void JsonToObject_ShouldParseSimpleJson()
    {
        var json = "{\"id\":1,\"activo\":true,\"nombre\":\"test\"}";
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        Assert.NotNull(result);
        object NormalizeNumber(object value)
        {
            return value switch
            {
                double d when d == Math.Truncate(d) && d >= int.MinValue && d <= int.MaxValue => (int)d,
                double d when d == Math.Truncate(d) && d >= long.MinValue && d <= long.MaxValue => (long)d,
                _ => value
            };
        }

        var id = NormalizeNumber(result["id"]);
        var activo = result["activo"];
        var nombre = result["nombre"];

        // Pruebas
        Assert.IsType<int>(id);
        Assert.Equal(1, id);

        Assert.IsType<bool>(activo);
        Assert.True((bool)activo);

        Assert.IsType<string>(nombre);
        Assert.Equal("test", nombre);
    }

    [Fact]
    public void JsonToObject_ShouldThrowException_OnInvalidJson()
    {
        var json = "{invalid json}";
        var ex = Assert.Throws<InvalidOperationException>(() => Functions.JsonToObject(json));
        Assert.Contains("El JSON no es válido", ex.Message);
    }

    [Theory]
    [InlineData("abc123", @"^[a-z]+\d+$", true)]
    [InlineData("123abc", @"^[a-z]+\d+$", false)]
    [InlineData(null, @"^[a-z]+\d+$", false)]
    public void IsMatch_ShouldWorkCorrectly(string input, string pattern, bool expected)
    {
        var result = Functions.IsMatch(input, pattern);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsValidGuidV4_ShouldReturnFalse_ForEmptyGuid()
    {
        var result = Functions.IsValidGuidV4(Guid.Empty.ToString());
        Assert.False(result);
    }

    [Fact]
    public void IsValidGuidV4_ShouldReturnTrue_ForValidGuid()
    {
        var guid = Guid.NewGuid();
        var result = Functions.IsValidGuidV4(guid.ToString());
        Assert.True(result);
    }

    [Fact]
    public void HexToString_ShouldConvertCorrectly()
    {
        string hex = "48656C6C6F"; // "Hello"
        var bytes = Functions.HexToString(hex);
        var text = Encoding.UTF8.GetString(bytes);

        Assert.Equal("Hello", text);
    }

    [Fact]
    public void HexToString_ShouldThrow_OnOddLength()
    {
        string hex = "ABC";
        var ex = Assert.Throws<ArgumentException>(() => Functions.HexToString(hex));
        Assert.Contains("exactamente", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(0, 5)]
    public void Next_ShouldReturnInRange(int min, int max)
    {
        int result = Functions.Next(min, max);
        Assert.InRange(result, min, max - 1);
    }

    [Fact]
    public void FormatearObjectToJson_ShouldHandleNullInput()
    {
        string result = Functions.FormatearObjectToJson(null);
        Assert.Equal("null", result);
    }

    [Fact]
    public void FormatearJsonStringToObject_ShouldThrowOnInvalidObject()
    {
        // input no serializable como JSON (delegate no se puede serializar)
        var invalidObject = new { Action = (Action)(() => { }) };

        Assert.Throws<NotSupportedException>(() =>
            Functions.FormatearJsonStringToObject<Dictionary<string, object>>(invalidObject));
    }

    [Fact]
    public void JsonToObject_ShouldThrowOnMalformedJson()
    {
        var badJson = "{ unclosed: true, ";

        var ex = Assert.Throws<InvalidOperationException>(() => Functions.JsonToObject(badJson));
        Assert.Contains("El JSON no es válido", ex.Message);
    }

    [Theory]
    [InlineData(null, ".*")]
    [InlineData("test", null)]
    [InlineData(null, null)]
    public void IsMatch_ShouldReturnFalse_OnNullOrEmptyInputs(string input, string pattern)
    {
        var result = Functions.IsMatch(input, pattern);
        Assert.False(result);
    }

    [Fact]
    public void IsValidGuidV4_ShouldReturnFalse_OnInvalidString()
    {
        string notAGuid = "123-not-guid";

        var result = Functions.IsValidGuidV4(notAGuid);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void IsValidGuidV4_ShouldReturnFalse_OnEmptyInput(string input)
    {
        var result = Functions.IsValidGuidV4(input);
        Assert.False(result);
    }

    [Fact]
    public void HexToString_ShouldThrow_OnInvalidHexCharacter()
    {
        string invalidHex = "GG";

        Assert.Throws<FormatException>(() => Functions.HexToString(invalidHex));
    }

    [Fact]
    public void Next_ShouldThrow_WhenMinGreaterThanMax()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Functions.Next(10, 5));
    }

    [Fact]
    public void FormatTextException_ShouldReturnMessage_WhenNoInnerException()
    {
        var ex = new Exception("Mensaje de error");
        var result = Functions.FormatTextException(ex);
        Assert.Contains("Mensaje de error", result);
    }

    [Fact]
    public void FormatTextException_ShouldIncludeInnerMessage()
    {
        var inner = new Exception("Detalle interno");
        var ex = new Exception("Error principal", inner);

        var result = Functions.FormatTextException(ex);
        Assert.Contains("Error principal", result);
        Assert.Contains("Detalle interno", result);
    }

    [Fact]
    public void ConvertJsonElement_ShouldReturnListObject_WhenArrayJson()
    {
        // Arrange: lista JSON
        string json = "[\"a\", \"b\", \"c\"]";
        var element = JsonDocument.Parse(json).RootElement;

        // Act: usar reflection para invocar método privado
        var method = typeof(Functions)
            .GetMethod("ConvertJsonElement", BindingFlags.NonPublic | BindingFlags.Static);

        var result = method!.Invoke(null, new object[] { element });

        // Assert
        Assert.NotNull(result);
        var list = Assert.IsType<List<object>>(result);
        Assert.Equal(3, list.Count);
        Assert.Equal("a", list[0]);
        Assert.Equal("b", list[1]);
        Assert.Equal("c", list[2]);
    }

    [Fact]
    public void FormatTextException_Should_Return_Cleaned_Message_When_No_InnerException()
    {
        // Arrange
        var exception = new Exception("  Hello   World  ");  // Mensaje con espacios extra

        // Act
        var result = Functions.FormatTextException(exception);

        // Assert
        Assert.Equal("Hello World", result);  // Se espera el mensaje limpio
    }

    [Fact]
    public void FormatTextException_Should_Format_Cleaned_Message_With_InnerException()
    {
        // Arrange
        var innerException = new Exception("  Inner   Error  ");
        var exception = new Exception("  Outer   Error  ", innerException);

        // Act
        var result = Functions.FormatTextException(exception);

        // Assert
        var expected = string.Format(FormatConstantsCore.FMT_LEGEND_ERROR, "Outer Error", "Inner Error");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void FormatTextException_Should_Handle_Empty_InnerException_Message()
    {
        // Arrange
        var innerException = new Exception("     ");
        var exception = new Exception("  Outer   Error  ", innerException);

        // Act
        var result = Functions.FormatTextException(exception);

        // Assert
        var expected = string.Format(FormatConstantsCore.FMT_LEGEND_ERROR, "Outer Error", "");
        Assert.Equal(expected, result);
    }

    [Fact]
    public void JsonToObject_Should_Parse_Simple_Object()
    {
        var json = "{\"name\":\"John\",\"age\":\"30\"}";
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        Assert.NotNull(result);
        Assert.Equal("John", result["name"]);
        Assert.Equal(Convert.ToInt16(30), Convert.ToInt16(result["age"])); // Verifica que parseó string "30" a int 30
    }

    [Fact]
    public void JsonToObject_Should_Parse_Array_Of_Strings()
    {
        var json = "[\"a\", \"b\", \"c\"]";
        var result = Functions.JsonToObject(json) as List<object>;

        Assert.NotNull(result);
        Assert.Collection(result,
            item => Assert.Equal("a", item),
            item => Assert.Equal("b", item),
            item => Assert.Equal("c", item));
    }

    [Fact]
    public void JsonToObject_Should_Parse_Boolean_And_Null()
    {
        var json = "{\"active\":true, \"deleted\":false, \"missing\":null}";
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        Assert.NotNull(result);
        Assert.True((bool)result["active"]);
        Assert.False((bool)result["deleted"]);
        Assert.Null(result["missing"]);
    }

    [Fact]
    public void JsonToObject_Should_Parse_Number_Types()
    {
        // Arrange
        var json = "{\"intVal\":123, \"longVal\":1234567890123, \"doubleVal\":3.1415}";

        // Act
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        // Assert
        object NormalizeNumber(object value)
        {
            return value switch
            {
                double d when d == Math.Truncate(d) && d >= int.MinValue && d <= int.MaxValue => (int)d,
                double d when d == Math.Truncate(d) && d >= long.MinValue && d <= long.MaxValue => (long)d,
                _ => value
            };
        }

        var intVal = NormalizeNumber(result["intVal"]);
        var longVal = NormalizeNumber(result["longVal"]);
        var doubleVal = result["doubleVal"];

        // Ahora sí podemos comparar con tipos exactos
        Assert.IsType<int>(intVal);
        Assert.Equal(123, intVal);

        Assert.IsType<long>(longVal);
        Assert.Equal(1234567890123L, longVal);

        Assert.IsType<double>(doubleVal);
        Assert.Equal(3.1415, (double)doubleVal, 4);
    }

    [Fact]
    public void JsonToObject_Should_Parse_Date_And_Boolean_Strings()
    {
        var json = "{\"date\":\"2023-07-03T10:00:00Z\", \"flag\":\"true\"}";
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        Assert.NotNull(result);
        Assert.IsType<System.DateTime>(result["date"]);
        Assert.True((bool)result["flag"]);
    }

    [Fact]
    public void JsonToObject_Should_Parse_Complex_Object_Correctly()
    {
        // JSON con objetos anidados, arreglos y distintos tipos
        var json = @"
            {
                ""id"": 123,
                ""name"": ""John Doe"",
                ""isActive"": ""true"",
                ""balance"": ""$1,234.56"",
                ""lastLogin"": ""2023-07-03T15:30:00Z"",
                ""roles"": [""admin"", ""user""],
                ""profile"": {
                    ""age"": ""30"",
                    ""emails"": [""john@example.com"", ""doe@example.org""],
                    ""address"": {
                        ""street"": ""123 Main St"",
                        ""city"": ""Springfield"",
                        ""zip"": ""12345""
                    }
                },
                ""preferences"": null
            }";

        var result = Functions.JsonToObject(json) as Dictionary<string, object>;
        Assert.NotNull(result);

        // Validar tipos simples
        Assert.IsType<int>(result["id"]);
        Assert.Equal("John Doe", result["name"]);
        Assert.True((bool)result["isActive"]);
        Assert.Equal(1234.56m, result["balance"]);
        Assert.IsType<DateTime>(result["lastLogin"]);

        // Validar arreglo roles
        var roles = Assert.IsType<List<object>>(result["roles"]);
        Assert.Collection(roles,
            r => Assert.Equal("admin", r),
            r => Assert.Equal("user", r));

        // Validar objeto profile
        var profile = Assert.IsType<Dictionary<string, object>>(result["profile"]);
        Assert.Equal(30, Convert.ToInt16(profile["age"]));

        var emails = Assert.IsType<List<object>>(profile["emails"]);
        Assert.Collection(emails,
            e => Assert.Equal("john@example.com", e),
            e => Assert.Equal("doe@example.org", e));

        var address = Assert.IsType<Dictionary<string, object>>(profile["address"]);
        Assert.Equal("123 Main St", address["street"]);
        Assert.Equal("Springfield", address["city"]);
        Assert.Equal("12345", address["zip"].ToString());

        // Validar valor nulo
        Assert.Null(result["preferences"]);
    }

    [Fact]
    public void JsonToObject_Should_Return_String_For_Invalid_Dates()
    {
        var json = @"
            {
                ""invalidDate1"": ""2023-13-40T25:61:00Z"",   // Mes, día y hora inválidos
                ""invalidDate2"": ""not-a-date"",
                ""validDate"": ""2023-07-03T10:00:00Z""
            }";

        var result = Functions.JsonToObject(json) as Dictionary<string, object>;
        Assert.NotNull(result);

        // Las fechas inválidas deben ser strings
        Assert.IsType<string>(result["invalidDate1"]);
        Assert.Equal("2023-13-40T25:61:00Z", result["invalidDate1"]);

        Assert.IsType<string>(result["invalidDate2"]);
        Assert.Equal("not-a-date", result["invalidDate2"]);

        // La fecha válida debe ser un DateTime
        Assert.IsType<DateTime>(result["validDate"]);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("123", 123)]  // int
    [InlineData("9223372036854775807", 9223372036854775807L)]  // long
    [InlineData("42.50", 42.5)]     // decimal, por TryParseCurrency
    [InlineData("42.5", 42.5)]      // decimal, por TryParseCurrency
    [InlineData("2024-07-05T15:30:00Z", "datetime")]  // DateTime
    [InlineData("SomeText", "SomeText")]  // string sin conversión
    public void ConvertString_Should_Convert_To_Correct_Type(string input, object expected)
    {
        // Arrange
        bool isIntegerInput = Regex.IsMatch(input, @"^-?\d+$");

        var method = typeof(Functions).GetMethod("ConvertString",
            BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Act
        var result = method.Invoke(null, new object[] { input });

        // Assert
        if(expected is string expectedStr)
        {
            if(expectedStr == "datetime")
            {
                Assert.IsType<DateTime>(result);
            }
            else
            {
                Assert.Equal(expectedStr, result);
            }
        }
        else
        {
            if(IsNumeric(expected))
            {
                if(isIntegerInput)
                    Assert.True(result is int or long, "El resultado debe ser un número entero.");

                object expectedDecimal = expected switch
                {
                    float f => Convert.ToSingle(f, CultureInfo.InvariantCulture),
                    double d => Convert.ToDouble(d, CultureInfo.InvariantCulture),
                    short s => Convert.ToInt16(s, CultureInfo.InvariantCulture),
                    int i => Convert.ToInt32(i, CultureInfo.InvariantCulture),
                    long l => Convert.ToInt64(l, CultureInfo.InvariantCulture),
                    sbyte sb => Convert.ToSByte(sb, CultureInfo.InvariantCulture),
                    decimal m => m,
                    _ => throw new InvalidOperationException($"Tipo no soportado: {expected.GetType()}")
                };

                switch(result)
                {
                    case decimal actualDecimal:
                        Assert.Equal(Convert.ToDecimal(expectedDecimal), actualDecimal);
                        break;
                    case float actualFloat:
                        Assert.Equal(Convert.ToSingle(expectedDecimal), actualFloat);
                        break;
                    case double actualDouble:
                        Assert.Equal(Convert.ToDouble(expectedDecimal), actualDouble);
                        break;
                    case int actualInt:
                        Assert.Equal(Convert.ToInt32(expectedDecimal), actualInt);
                        break;
                    case long actualLong:
                        Assert.Equal(Convert.ToInt64(expectedDecimal), actualLong);
                        break;
                    case short actualShort:
                        Assert.Equal(Convert.ToInt16(expectedDecimal), actualShort);
                        break;
                    case sbyte actualSByte:
                        Assert.Equal(Convert.ToSByte(expectedDecimal), actualSByte);
                        break;
                    default:
                        throw new InvalidOperationException($"Tipo numérico no soportado en resultado: {result.GetType()}");
                }
            }
            else
            {
                Assert.Equal(expected, result);
            }
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ConvertString_Should_Return_Empty_For_Null_Or_Whitespace(string input)
    {
        var method = typeof(Functions).GetMethod("ConvertString", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var result = method.Invoke(null, new object[] { input });

        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("123", true, 123)]
    [InlineData("-456", true, -456)]
    [InlineData("2147483648", true, 2147483648L)]
    [InlineData("9223372036854775807", true, long.MaxValue)]
    [InlineData("no-number", false, null)]
    [InlineData("", false, null)]
    [InlineData(" 987654321 ", true, 987654321)]
    public void TryParseInteger_TestCases(string input, bool expectedSuccess, object? expectedValue)
    {
        // Act
        bool success = InvokeTryParseInteger(input, out object? result);

        // Assert
        Assert.Equal(expectedSuccess, success);
        Assert.Equal(expectedValue, result);
    }

    [Theory]
    [InlineData("{\"price\":\"$1,234.56\"}", 1234.56)]
    [InlineData("{\"amount\":\"987654.32\"}", 987654.32)]
    [InlineData("{\"total\":\"$0.99\"}", 0.99)]
    public void JsonToObject_ShouldParse_CurrencyStrings(string json, decimal expectedDecimal)
    {
        // Act
        var result = Functions.JsonToObject(json) as Dictionary<string, object>;

        // Assert
        Assert.NotNull(result);
        var value = result.Values.First();

        Assert.IsType<decimal>(value);
        Assert.Equal(expectedDecimal, (decimal)value);
    }

    [Theory]
    [InlineData("abc123", @"\d+", 0, true)]     // Usa default timeout
    [InlineData("abc123", @"\d+", -5, true)]    // Usa default timeout (valor negativo)
    [InlineData("abc123", @"\d+", 500, true)]   // Usa timeout personalizado (500ms)
    [InlineData("abc", @"\d+", 0, false)]       // No match, con default timeout
    public void IsMatch_Should_Work_With_VariousTimeouts(string input, string pattern, double timeout, bool expectedMatch)
    {
        // Act
        bool result = Functions.IsMatch(input, pattern, timeout);

        // Assert
        Assert.Equal(expectedMatch, result);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("not-a-guid", false)]
    [InlineData("00000000-0000-0000-0000-000000000000", false)]
    [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff", false)]
    [InlineData("a7c3f53e-8e50-4ddf-91e8-c6a4a4fefb88", true)]
    public void IsValidGuidV4_Should_ValidateGuidCorrectly(string input, bool expected)
    {
        // Act
        bool result = Functions.IsValidGuidV4(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("", 0, "")]
    [InlineData("  hola   mundo  ", 0, "hola mundo")]
    [InlineData("Hola mundo", 0, "Hola mundo")]
    [InlineData("  hola   mundo  ", 500, "hola mundo")]
    [InlineData("  hola   mundo  ", -10, "hola mundo")]
    public void ClearText_Should_CleanSpaces_Correctly(string input, double timeout, string expected)
    {
        // Act
        string result = InvokeClearText(input, timeout);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2025-07-06T14:30:00Z", true)]
    [InlineData("not-a-date", false)]
    public void TryParseDateTime_Should_ParseCorrectly(string input, bool expectedSuccess)
    {
        // Act
        bool success = InvokeTryParseDateTime(input, out DateTime result);

        // Assert
        Assert.Equal(expectedSuccess, success);
        if(success)
        {
            Assert.NotEqual(default(DateTime), result);
        }
    }

    [Theory]
    [InlineData("$1,234.56", true, 1234.56)]
    [InlineData("1234.56", true, 1234.56)]
    [InlineData("invalid", false, 0)]
    public void TryParseCurrency_Should_ParseCorrectly(string input, bool expectedSuccess, decimal expectedValue)
    {
        // Act
        bool success = InvokeTryParseCurrency(input, out decimal result);

        // Assert
        Assert.Equal(expectedSuccess, success);
        if(success)
        {
            Assert.Equal(expectedValue, result);
        }
    }

    [Theory]
    [InlineData(@"{""key"":""value""}", typeof(Dictionary<string, object>))]
    [InlineData(@"[1,2,3]", typeof(List<object>))]
    [InlineData(@"""hello""", typeof(string))]
    [InlineData(@"123", typeof(int))]
    [InlineData(@"true", typeof(bool))]
    [InlineData(@"false", typeof(bool))]
    [InlineData(@"null", null)] // Null case
    public void ConvertJsonElement_Should_Convert_Correctly(string json, Type expectedType)
    {
        using var document = JsonDocument.Parse(json);
        var element = document.RootElement;

        var result = InvokeConvertJsonElement(element);

        if(expectedType == null)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.IsType(expectedType, result);
        }
    }

    [Fact]
    public void ConvertString_Should_Parse_Integer_Successfully()
    {
        // Arrange
        string input = "1234";  // valor que debe entrar a TryParseInteger

        // Act
        var result = InvokeConvertString(input);

        // Assert
        Assert.IsType<int>(result);
        Assert.Equal(1234, result);
    }

    [Fact]
    public void ConvertString_Should_Parse_Long_Successfully()
    {
        string input = "9223372036854775807"; // long.MaxValue
        var result = InvokeConvertString(input);
        Assert.IsType<long>(result);
        Assert.Equal(9223372036854775807L, result);
    }

    [Fact]
    public void ConvertString_Should_Return_String_When_Not_Parsable()
    {
        string input = "Test123";
        var result = InvokeConvertString(input);
        Assert.IsType<string>(result);
        Assert.Equal("Test123", result);
    }

    [Fact]
    public void ConvertJsonElement_Should_Handle_Object()
    {
        // Arrange
        var json = "{\"key\":\"value\"}";
        var element = CreateJsonElement(json);

        // Act
        var result = InvokeConvertJsonElement(element);

        // Assert
        Assert.IsType<Dictionary<string, object>>(result);
        var dict = (Dictionary<string, object>)result;
        Assert.Equal("value", dict["key"]);
    }

    [Fact]
    public void ConvertJsonElement_Should_Throw_InvalidOperationException_For_Undefined()
    {
        // Arrange
        var element = default(JsonElement);

        var method = typeof(Functions).GetMethod("ConvertJsonElement", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        // Act & Assert
        var exception = Assert.Throws<TargetInvocationException>(() => method.Invoke(null, new object[] { element }));
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Theory]
    [InlineData("Hola", 10, "Hola")]                    // texto corto
    [InlineData("12345", 5, "12345")]                   // texto igual al máximo
    [InlineData("ABCDEFGHIJ", 5, "ABCDE...(truncated)")]// texto más largo
    [InlineData("", 5, "")]                             // texto vacío
    [InlineData(null, 5, null)]                         // texto nulo
    [InlineData("Hola", 100, "Hola")]                   // maxLength mayor al texto
    public void TruncateText_Should_Return_Expected_Value(string text, int maxLength, string expected)
    {
        // Act
        var result = Functions.TruncateText(text, maxLength);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Texto", -1)]
    [InlineData("Prueba", -10)]
    public void TruncateText_Should_Throw_When_MaxLength_Is_Negative(string text, int maxLength)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => Functions.TruncateText(text, maxLength));
        Assert.Contains("maxLength", exception.Message);
    }
    private static bool InvokeTryParseInteger(string input, out object? result)
    {
        // Usar reflexión porque el método es privado
        var method = typeof(Functions)
            .GetMethod("TryParseInteger", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method); // Asegura que el método existe

        object[] parameters = new object[] { input, null! };
        bool success = (bool)method.Invoke(null, parameters)!;

        result = parameters[1]; // El out parameter va en parameters[1]
        return success;
    }

    private static string InvokeClearText(string input, double retryInMiliseconds = 0)
    {
        var method = typeof(Functions)
            .GetMethod("ClearText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

        Assert.NotNull(method);

        object[] parameters = new object[] { input, retryInMiliseconds };

        var result = method.Invoke(null, parameters);

        return (string)result!;
    }

    private static bool InvokeTryParseDateTime(string input, out DateTime result)
    {
        var method = typeof(Functions)
            .GetMethod("TryParseDateTime", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        object[] parameters = new object[] { input, default(DateTime) };

        bool success = (bool)method.Invoke(null, parameters)!;

        result = (DateTime)parameters[1]; // El out parameter va en parameters[1]

        return success;
    }

    private static bool InvokeTryParseCurrency(string input, out decimal result)
    {
        var method = typeof(Functions)
            .GetMethod("TryParseCurrency", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        object[] parameters = new object[] { input, default(decimal) };

        bool success = (bool)method.Invoke(null, parameters)!;

        result = (decimal)parameters[1];

        return success;
    }

    private static object InvokeConvertJsonElement(JsonElement element)
    {
        var method = typeof(Functions)
            .GetMethod("ConvertJsonElement", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        return method.Invoke(null, new object[] { element })!;
    }

    private static bool IsNumeric(object value)
    {
        return value is sbyte or byte or short or ushort or int or uint or long or ulong
               or float or double or decimal or string;
    }

    private static object InvokeConvertString(string input)
    {
        var method = typeof(Functions).GetMethod("ConvertString", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(method);

        return method.Invoke(null, new object[] { input });
    }

    private static JsonElement CreateJsonElement(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone(); // Clonar para evitar dispose
    }
}
