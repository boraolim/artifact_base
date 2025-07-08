namespace Hogar.Core.Shared.Tests;

public class ByteArrayJsonConverterTests
{
    private readonly ByteArrayJsonConverter _converter = new ByteArrayJsonConverter();

    [Fact]
    public void Write_ShouldSerializeByteArray_AsBase64String()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3, 4, 5 };
        var options = new JsonSerializerOptions();
        options.Converters.Add(_converter);

        // Act
        var json = JsonSerializer.Serialize(bytes, options);

        // Assert
        var expected = $"\"{Convert.ToBase64String(bytes)}\"";
        Assert.Equal(expected, json);
    }

    [Fact]
    public void Read_ShouldDeserializeBase64String_ToByteArray()
    {
        // Arrange
        var bytes = new byte[] { 10, 20, 30, 40, 50 };
        var base64String = Convert.ToBase64String(bytes);
        var json = $"\"{base64String}\"";

        var options = new JsonSerializerOptions();
        options.Converters.Add(_converter);

        // Act
        var result = JsonSerializer.Deserialize<byte[]>(json, options);

        // Assert
        Assert.Equal(bytes, result);
    }

    [Fact]
    public void Read_InvalidBase64_ShouldThrowFormatException()
    {
        // Arrange
        var json = "\"InvalidBase64@@@\"";
        var options = new JsonSerializerOptions();
        options.Converters.Add(_converter);

        // Act & Assert
        Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<byte[]>(json, options));
    }

}
