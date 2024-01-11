using LilySwapper.Workspace.Compression;
using LilySwapper.Workspace.Hashes;
using LilySwapper.Workspace.Swapping.Compression.Types;
using RestSharp;

namespace LilySwapper.Workspace.Swapping.Providers;

public static class StreamDataProvider
{
    public enum CompressionType
    {
        None = 0,
        Aes = 1,
        Zlib = 2,
        Oodle = 3,
        GZip = 4
    }

    private const string Domain = "https://galaxyswapperv2.com/API/StreamData/{0}.chunk";

    public static byte[] Download(string path)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var chunkName = Convert.ToHexString(MD5.Hash(path));
        var url = string.Format(Domain, chunkName);

        using (var client = new RestClient())
        {
            var request = new RestRequest(new Uri(url));

            Log.Information($"Sending {request.Method} request to {url}");

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK || response.RawBytes is null || response.RawBytes.Length == 0)
            {
                Log.Fatal(
                    $"Failed to download response from StreamData! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                Message.DisplaySTA("Error", "Webclient caught a exception while downloading StreamData.", discord: true,
                    solutions: new[]
                        { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                return null!;
            }

            Log.Information(
                $"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

            var reader = new Reader(response.RawBytes);
            reader.BaseStream.Position += 4;
            var compressionType = (CompressionType)reader.Read<int>();

            switch (compressionType)
            {
                case CompressionType.None:
                    return response.RawBytes;
                case CompressionType.Aes:
                {
                    var keyHash = reader.Read<ulong>();
                    var keyLength = reader.Read<int>();
                    var key = reader.ReadBytes(keyLength);

                    if (CityHash.Hash(key) != keyHash)
                    {
                        Log.Error("StreamData encryption key hash miss match");
                        throw new CustomException("StreamData encryption key hash miss match");
                    }

                    var encryptedHash = reader.Read<ulong>();
                    var unencryptedHash = reader.Read<ulong>();
                    var encryptedLength = reader.Read<int>();
                    var encryptedBuffer = reader.ReadBytes(encryptedLength);

                    if (CityHash.Hash(encryptedBuffer) != encryptedHash)
                    {
                        Log.Error("StreamData encryptedBuffer hash miss match");
                        throw new CustomException("StreamData encryptedBuffer hash miss match");
                    }

                    var unencryptedBuffer = aes.Decrypt(encryptedBuffer, key);

                    if (CityHash.Hash(unencryptedBuffer) != unencryptedHash)
                    {
                        Log.Error("StreamData unencryptedBuffer hash miss match");
                        throw new CustomException("StreamData unencryptedBuffer hash miss match");
                    }

                    return unencryptedBuffer;
                }
                case CompressionType.Zlib:
                {
                    var compressedHash = reader.Read<ulong>();
                    var uncompressedHash = reader.Read<ulong>();
                    var compressedSize = reader.Read<int>();
                    var uncompressedSize = reader.Read<int>();

                    var compressedBuffer = reader.ReadBytes(compressedSize);

                    if (compressedHash != CityHash.Hash(compressedBuffer))
                    {
                        Log.Error("StreamData compressedHash miss match");
                        throw new CustomException("StreamData compressedHash miss match!");
                    }

                    var uncompressedBuffer = zlib.Decompress(compressedBuffer, uncompressedSize);

                    if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                    {
                        Log.Error("StreamData uncompressedHash miss match");
                        throw new CustomException("StreamData uncompressedHash miss match!");
                    }

                    return uncompressedBuffer;
                }
                case CompressionType.Oodle:
                {
                    var compressedHash = reader.Read<ulong>();
                    var uncompressedHash = reader.Read<ulong>();
                    var compressedSize = reader.Read<int>();
                    var uncompressedSize = reader.Read<int>();

                    var compressedBuffer = reader.ReadBytes(compressedSize);

                    if (compressedHash != CityHash.Hash(compressedBuffer))
                    {
                        Log.Error("StreamData compressedHash miss match");
                        throw new CustomException("StreamData compressedHash miss match!");
                    }

                    var uncompressedBuffer = Oodle.Decompress(compressedBuffer, uncompressedSize);

                    if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                    {
                        Log.Error("StreamData uncompressedHash miss match");
                        throw new CustomException("StreamData uncompressedHash miss match!");
                    }

                    return uncompressedBuffer;
                }
                case CompressionType.GZip:
                {
                    var compressedHash = reader.Read<ulong>();
                    var uncompressedHash = reader.Read<ulong>();
                    var compressedSize = reader.Read<int>();


                    var compressedBuffer = reader.ReadBytes(compressedSize);

                    if (compressedHash != CityHash.Hash(compressedBuffer))
                    {
                        Log.Error("StreamData compressedHash miss match");
                        throw new CustomException("StreamData compressedHash miss match!");
                    }

                    var uncompressedBuffer = gzip.Decompress(compressedBuffer);

                    if (uncompressedHash != CityHash.Hash(uncompressedBuffer))
                    {
                        Log.Error("StreamData uncompressedHash miss match");
                        throw new CustomException("StreamData uncompressedHash miss match!");
                    }

                    return uncompressedBuffer;
                }
                default:
                    Log.Error("StreamData unknown compression type");
                    throw new CustomException("StreamData unknown compression type");
            }
        }
    }
}