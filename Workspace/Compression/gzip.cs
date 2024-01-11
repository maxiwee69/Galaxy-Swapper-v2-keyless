using System.IO.Compression;

namespace LilySwapper.Workspace.Compression;

public static class gzip
{
    public static byte[] Compress(byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                gzipStream.Write(data, 0, data.Length);
            }

            return compressedStream.ToArray();
        }
    }

    public static byte[] Compress(string data)
    {
        return Compress(Encoding.ASCII.GetBytes(data));
    }

    public static byte[] Decompress(byte[] compressedData)
    {
        using (var compressedStream = new MemoryStream(compressedData))
        using (var decompressedStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
                    decompressedStream.Write(buffer, 0, bytesRead);
            }

            return decompressedStream.ToArray();
        }
    }

    public static byte[] Decompress(string base64)
    {
        return Decompress(Convert.FromBase64String(base64));
    }
}