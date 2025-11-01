namespace Utilities.Core.Shared.Services
{
    public interface ICypherAes
    {
        long Nonce { get; set; }
        string AESEncryptionGCM(string plainText);
        string AESEncryptionGCM(string secretValue, string plainText);
        string AESDecryptionGCM(string encryptedText);
        string AESDecryptionGCM(string secretValue, string encryptedText);
    }
}
