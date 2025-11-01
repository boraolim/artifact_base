namespace Utilities.Core.Shared.Tests;

public class CypherAesTests
{
    private const string ValidKey = "12345678901234567890123456789012"; // Exactamente 32 chars
    private const string InvalidKey = "short_key"; // Menos de 32 chars
    private const string PlainText = "Este es un mensaje secreto.";

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyLengthIsInvalid()
    {
        var ex = Assert.Throws<ArgumentException>(() => new CypherAes(InvalidKey));
        Assert.Equal(MessageConstantsCore.MSG_EXACTLY_32_CHARS, ex.Message);
    }

    [Fact]
    public void AESEncryptionGCM_ShouldEncryptAndDecryptSuccessfully()
    {
        var cypher = new CypherAes(ValidKey);
        var encrypted = cypher.AESEncryptionGCM(PlainText);

        Assert.False(string.IsNullOrWhiteSpace(encrypted));
        Assert.NotEqual(PlainText, encrypted);
        Assert.NotEqual(0, cypher.Nonce);

        var decrypted = cypher.AESDecryptionGCM(encrypted);
        Assert.Equal(PlainText, decrypted);
    }

    [Fact]
    public void AESEncryptionGCM_WithSecretParameter_ShouldWorkCorrectly()
    {
        var cypher = new CypherAes(ValidKey);
        var encrypted = cypher.AESEncryptionGCM(ValidKey, PlainText);
        var decrypted = cypher.AESDecryptionGCM(ValidKey, encrypted);

        Assert.Equal(PlainText, decrypted);
    }

    [Fact]
    public void AESDecryptionGCM_ShouldFailWithWrongKey()
    {
        var cypher = new CypherAes(ValidKey);
        var encrypted = cypher.AESEncryptionGCM(PlainText);

        var wrongCypher = new CypherAes(ValidKey.Replace("1", "2")); // Cambiar clave
        Assert.ThrowsAny<CryptographicException>(() => wrongCypher.AESDecryptionGCM(encrypted));
    }

    [Fact]
    public void AESDecryptionGCM_ShouldFailWithCorruptedData()
    {
        var cypher = new CypherAes(ValidKey);
        var encrypted = cypher.AESEncryptionGCM(PlainText);
        // Dañar el texto cifrado
        var corruptedEncrypted = encrypted.Substring(0, encrypted.Length - 2) + "FF";

        Assert.ThrowsAny<CryptographicException>(() => cypher.AESDecryptionGCM(corruptedEncrypted));
    }
}
