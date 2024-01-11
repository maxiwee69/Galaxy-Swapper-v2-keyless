namespace LilySwapper.Workspace.Generation.Formats;

public class Asset : ICloneable
{
    public bool IsStreamData = false;
    public MaterialData MaterialData;
    public string Object;
    public string OverrideBuffer;
    public string OverrideObject;
    public JToken Swaps;
    public TextureData TextureData;
    public GameFile Export { get; set; } = default!;
    public GameFile OverrideExport { get; set; } = default!;

    public object Clone()
    {
        return MemberwiseClone();
    }
}