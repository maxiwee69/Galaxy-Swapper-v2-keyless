namespace LilySwapper.Workspace.Structs;

public class MaterialOverrideFlagsData
{
    public byte[] SearchBuffer { get; set; } = default!;
    public long Offset { get; set; } = default!;
    public int MaterialOverrideFlags { get; set; } = default!;
}