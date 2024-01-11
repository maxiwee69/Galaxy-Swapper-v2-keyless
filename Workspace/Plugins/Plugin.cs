using LilySwapper.Workspace.Compression;
using LilySwapper.Workspace.Hashes;
using Newtonsoft.Json;

namespace LilySwapper.Workspace.Plugins;

public static class Plugin
{
    public static readonly string Path = $"{App.Config}\\Plugins";

    public static void Import(FileInfo fileInfo, JObject parse)
    {
        var writer = new Writer(new byte[fileInfo.Length + 60000]);
        var type = Compression.Type.Aes;

        if (!parse["Compression"].KeyIsNullOrEmpty() && Enum.TryParse(typeof(Compression.Type),
                parse["Compression"].Value<string>(), true, out var newtype)) type = (Compression.Type)newtype;

        writer.Write(type); //1 = encrypted in case I want to add other formats later on

        if (!Compression.Compress(out var compressed, out var key, out var uncompressedsize,
                parse.ToString(Formatting.None), type))
            return;

        //Import directory
        var importpath = Encoding.ASCII.GetBytes(fileInfo.FullName);
        writer.Write(importpath.Length);
        writer.WriteBytes(importpath);

        //Encryption key
        if (key is not null)
        {
            writer.Write(CityHash.Hash(key));
            writer.Write(key.Length);
            writer.WriteBytes(key);
        }

        if (uncompressedsize != 0) writer.Write(uncompressedsize);

        //Plugin buffer
        writer.Write(CityHash.Hash(compressed));
        writer.Write(compressed.Length);
        writer.WriteBytes(compressed);

        var output = $"{Path}\\{System.IO.Path.GetRandomFileName()}.plugin";
        File.WriteAllBytes(output, writer.ToByteArray(writer.Position));

        Log.Information($"Plugin wrote to: {output}");
    }

    public static PluginData Export(FileInfo fileInfo)
    {
        var reader = new Reader(File.ReadAllBytes(fileInfo.FullName));
        var type = (Compression.Type)reader.Read<int>();
        var importpathlength = reader.Read<int>();
        var importpath = reader.ReadStrings(importpathlength);

        if (!Compression.Decompress(reader, fileInfo, out var decompressed, type))
            return null!;

        if (!decompressed.ValidJson())
        {
            Log.Warning($"{fileInfo.Name} is not in a valid json format and will be skipped");
            return null!;
        }

        var parse = JObject.Parse(decompressed);
        return new PluginData { Import = importpath, Path = fileInfo.FullName, Content = decompressed, Parse = parse };
    }

    public static PluginData ExportOld(FileInfo fileInfo)
    {
        var content = File.ReadAllText(fileInfo.FullName);

        content = Encoding.ASCII.GetString(gzip.Decompress(content));

        if (!content.ValidJson())
        {
            Log.Warning($"{fileInfo.Name} is not in a valid json format and will be skipped");
            return null!;
        }

        var parse = JObject.Parse(content);
        return new PluginData { Import = null!, Path = fileInfo.FullName, Content = content, Parse = parse };
    }

    public static List<PluginData> GetPlugins()
    {
        var list = new List<PluginData>();

        foreach (var plugin in new DirectoryInfo(Path).GetFiles("*.*", SearchOption.AllDirectories))
        {
            PluginData plugindata = null!;

            switch (plugin.Extension)
            {
                case ".encrypted":
                    plugindata = ExportOld(plugin);
                    break;
                case ".plugin":
                    plugindata = Export(plugin);
                    break;
            }

            if (plugindata is not null)
                list.Add(plugindata);
        }

        return list;
    }
}