namespace LilySwapper.Workspace.Structs;

public class TextureParameter
{
    public int MaterialIndexForTextureParameter { get; set; } = default!;
    public string TextureParameterNameForMaterial { get; set; } = default!;
    public string TextureOverride { get; set; } = default!;
}