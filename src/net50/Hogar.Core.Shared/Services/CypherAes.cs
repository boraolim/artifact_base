using System;
using System.Text;
using System.Buffers.Binary;
using System.Security.Cryptography;

using Hogar.Core.Shared.Utils;

using MainConstantsCore = Hogar.Core.Shared.Constants.MainConstants;
using MessageConstantsCore = Hogar.Core.Shared.Constants.MessageConstants;

namespace Hogar.Core.Shared.Services
{
    public sealed class CypherAes : ICypherAes
    {
        private byte[] keySecret { get; set; }

        public long Nonce { get; set; }

        public CypherAes(string secretValue)
        {
            if (secretValue.Length != MainConstantsCore.CFG_MAX_KEY_NUMBER)
                throw new ArgumentException(MessageConstantsCore.MSG_EXACTLY_32_CHARS);

            keySecret = Encoding.UTF8.GetBytes(secretValue);
        }

        public string AESEncryptionGCM(string plainText)
        {
            // Get bytes of plaintext string
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            // Get parameter sizes
            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            // We write everything into one big array for easier encoding
            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < MainConstantsCore.CFG_BUFFER_VALUE ?
                stackalloc byte[encryptedDataLength] : new byte[encryptedDataLength].AsSpan();

            // Copy parameters
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Generate secure nonce
            RandomNumberGenerator.Fill(nonce);

            // Save nonce
            Nonce = BitConverter.ToInt64(nonce.ToArray(), 0);

            // Encrypt
            using var aes = new AesGcm(keySecret);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            // Encode for transmission
            return BitConverter.ToString(encryptedData.ToArray()).Replace("-", string.Empty);
        }
        public string AESEncryptionGCM(string secretValue, string plainText)
        {
            keySecret = Encoding.UTF8.GetBytes(secretValue);
            return AESEncryptionGCM(plainText);
        }
        public string AESDecryptionGCM(string encryptedText)
        {
            // Decode
            var encryptedData = Functions.HexToString(encryptedText).AsSpan();

            // Extract parameter sizes
            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            // Extract parameters
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < MainConstantsCore.CFG_BUFFER_VALUE ?
                stackalloc byte[cipherSize] : new byte[cipherSize];

            // Save nonce
            Nonce = BitConverter.ToInt64(nonce.ToArray(), 0);

            using var aes = new AesGcm(keySecret);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.UTF8.GetString(plainBytes);
        }
        public string AESDecryptionGCM(string secretValue, string encryptedText)
        {
            keySecret = Encoding.UTF8.GetBytes(secretValue);
            return AESDecryptionGCM(encryptedText);
        }
    }
}
