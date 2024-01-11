namespace LilySwapper.Workspace.Structs;

public class BinaryData
{
    public BinaryData(string hash, string name, string path)
    {
        Hash = hash;
        Name = name;
        Path = path;
    }

    public string Hash { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
}