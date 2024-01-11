using System.Security.Cryptography;

namespace LilySwapper.Workspace.Compression;

public static class aes
{
    public static byte[] Encrypt(string plainText, byte[] key)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream))
                {
                    streamWriter.Write(plainText);
                }

                return memoryStream.ToArray();
            }
        }
    }

    public static byte[] Decrypt(byte[] encryptedBytes, byte[] key)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            var iv = new byte[16];
            Array.Copy(encryptedBytes, 0, iv, 0, 16);
            aesAlg.IV = iv;

            using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
            using (var memoryStream = new MemoryStream(encryptedBytes, 16, encryptedBytes.Length - 16))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                using (var decryptedMemoryStream = new MemoryStream())
                {
                    cryptoStream.CopyTo(decryptedMemoryStream);
                    return decryptedMemoryStream.ToArray();
                }
            }
        }
    }
}