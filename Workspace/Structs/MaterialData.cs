namespace LilySwapper.Workspace.Structs;

public class MaterialData
{
    public List<OverrideMaterial> Materials = new();
    public byte[] SearchBuffer { get; set; } = default!;
    public long Offset { get; set; } = default!;
    public MaterialOverrideFlagsData MaterialOverrideFlags { get; set; } = default!;
}