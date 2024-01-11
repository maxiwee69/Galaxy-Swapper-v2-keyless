namespace LilySwapper.Workspace.Structs;

public class TextureData
{
    public List<TextureParameter> TextureParameters = new();
    public byte[] SearchBuffer { get; set; } = default!;
    public long Offset { get; set; } = default!;
}