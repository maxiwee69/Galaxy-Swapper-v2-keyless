using System.IO.Compression;
using System.Security.Cryptography;

namespace LilySwapper.Workspace.Utilities;

public static class Encryption
{
    public static string Compress(this string uncompressedString)
    {
        byte[] compressedBytes;

        using (var uncompressedStream = new MemoryStream(Encoding.UTF8.GetBytes(uncompressedString)))
        {
            using (var compressedStream = new MemoryStream())
            {
                using (var compressorStream = new DeflateStream(compressedStream, CompressionLevel.Optimal, true))
                {
                    uncompressedStream.CopyTo(compressorStream);
                }

                compressedBytes = compressedStream.ToArray();
            }
        }

        return Encrypt(Convert.ToBase64String(compressedBytes));
    }

    public static string Decompress(this string compressedString)
    {
        byte[] DecompressedBytes = null;
        using (var CompressedStream = new MemoryStream(Convert.FromBase64String(Decrypt(compressedString))))
        {
            using (var DecompressStream = new DeflateStream(CompressedStream, CompressionMode.Decompress))
            {
                using (var DecompressedStream = new MemoryStream())
                {
                    DecompressStream.CopyTo(DecompressedStream);
                    DecompressedBytes = DecompressedStream.ToArray();
                }
            }
        }

        return Encoding.UTF8.GetString(DecompressedBytes);
    }

    private static string Encrypt(string plainText)
    {
        var text = "ᣤᣤ";
        var text2 = "졊젙졅졎젡졇졂졗";
        var s = "@1B2c3D5e5F5b7H8";
        int num = text[0];
        var num2 = num;
        num = num2 ^ 25064;
        var num3 = num;
        num = num3 + 3829;
        var num4 = num;
        var num5 = num4 << 5;
        var num6 = num;
        var num7 = num6 & 65535;
        var num8 = num5 | (num7 >> 11);
        num = num8 & 65535;
        var text3 = text;
        var str = text3.Substring(0, 0);
        var num9 = num;
        var str2 = ((char)(num9 & 65535)).ToString();
        var text4 = text;
        text = str + str2 + text4.Substring(1);
        int num10 = text2[0];
        var num11 = num10;
        num10 = num11 - 51191;
        num10 -= 0;
        var text5 = text2;
        var str3 = text5.Substring(0, 0);
        var num12 = num10;
        var str4 = ((char)(num12 & 65535)).ToString();
        var text6 = text2;
        text2 = str3 + str4 + text6.Substring(1);
        var bytes = Encoding.UTF8.GetBytes(plainText);
        DeriveBytes deriveBytes = new Rfc2898DeriveBytes(text, Encoding.ASCII.GetBytes(text2));
        var bytes2 = deriveBytes.GetBytes(32);
        var rijndaelManaged = new RijndaelManaged
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.Zeros
        };
        var cryptoTransform = rijndaelManaged.CreateEncryptor(bytes2, Encoding.ASCII.GetBytes(s));
        byte[] inArray;
        using (var memoryStream = new MemoryStream())
        {
            Stream stream = memoryStream;
            var transform = cryptoTransform;
            using (var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Write))
            {
                Stream stream2 = cryptoStream;
                var buffer = bytes;
                stream2.Write(buffer, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                inArray = memoryStream.ToArray();
                cryptoStream.Close();
            }

            memoryStream.Close();
        }

        return Convert.ToBase64String(inArray);
    }

    private static string Decrypt(string encryptedText)
    {
        var text = "ᣤᣤ";
        var text2 = "졊젙졅졎젡졇졂졗";
        var s = "@1B2c3D5e5F5b7H8";
        int num = text[0];
        var num2 = num;
        num = num2 ^ 25064;
        var num3 = num;
        num = num3 + 3829;
        var num4 = num;
        var num5 = num4 << 5;
        var num6 = num;
        var num7 = num6 & 65535;
        var num8 = num5 | (num7 >> 11);
        num = num8 & 65535;
        var text3 = text;
        var str = text3.Substring(0, 0);
        var num9 = num;
        var str2 = ((char)(num9 & 65535)).ToString();
        var text4 = text;
        text = str + str2 + text4.Substring(1);
        int num10 = text2[0];
        var num11 = num10;
        num10 = num11 - 51191;
        num10 -= 0;
        var text5 = text2;
        var str3 = text5.Substring(0, 0);
        var num12 = num10;
        var str4 = ((char)(num12 & 65535)).ToString();
        var text6 = text2;
        text2 = str3 + str4 + text6.Substring(1);
        var array = Convert.FromBase64String(encryptedText);
        DeriveBytes deriveBytes = new Rfc2898DeriveBytes(text, Encoding.ASCII.GetBytes(text2));
        var bytes = deriveBytes.GetBytes(32);
        var rijndaelManaged = new RijndaelManaged
        {
            Mode = CipherMode.CBC,
            Padding = PaddingMode.None
        };
        var cryptoTransform = rijndaelManaged.CreateDecryptor(bytes, Encoding.ASCII.GetBytes(s));
        var memoryStream = new MemoryStream(array);
        Stream stream = memoryStream;
        var transform = cryptoTransform;
        var cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
        var array2 = new byte[array.Length];
        Stream stream2 = cryptoStream;
        var buffer = array2;
        var count = stream2.Read(buffer, 0, array2.Length);
        memoryStream.Close();
        cryptoStream.Close();
        var utf = Encoding.UTF8;
        var bytes2 = array2;
        return utf.GetString(bytes2, 0, count).TrimEnd("\0".ToCharArray());
    }
}