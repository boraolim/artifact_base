namespace Hogar.Core.Shared.Tests;

public class UuidV4JsonConverterTests
{
    [Fact]
    public void Write_ShouldSerializeGuidAsString()
    {
        var guid = Guid.NewGuid();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UuidV4JsonConverter());

        var json = JsonSerializer.Serialize(guid, options);

        Assert.Equal($"\"{guid}\"", json);
    }

    [Fact]
    public void Read_ShouldDeserializeValidGuidV4()
    {
        var guid = Guid.NewGuid();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UuidV4JsonConverter());

        var json = $"\"{guid}\"";

        var result = JsonSerializer.Deserialize<Guid>(json, options);

        Assert.Equal(guid, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Read_ShouldThrowJsonException_OnNullOrEmpty(string input)
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UuidV4JsonConverter());

        var json = $"\"{input}\"";

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Guid>(json, options)
        );

        Assert.Contains(input ?? "", ex.Message);
    }


    [Fact]
    public void Read_ShouldThrowJsonException_OnInvalidGuidV4()
    {
        var invalidUuid = "invalid-uuid";
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UuidV4JsonConverter());

        var json = $"\"{invalidUuid}\"";

        var ex = Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize<Guid>(json, options)
        );

        Assert.Contains(invalidUuid, ex.Message);
    }

    [Fact]
    public void SerializeDeserialize_ShouldWorkCorrectly()
    {
        var guid = Guid.NewGuid();
        var options = new JsonSerializerOptions();
        options.Converters.Add(new UuidV4JsonConverter());

        var json = JsonSerializer.Serialize(guid, options);
        var result = JsonSerializer.Deserialize<Guid>(json, options);

        Assert.Equal(guid, result);
    }

}
