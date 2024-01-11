namespace LilySwapper.Workspace.CProvider.Objects;

[StructLayout(LayoutKind.Sequential)]
public readonly struct FIoContainerId
{
    public readonly ulong Id;

    public override string ToString()
    {
        return Id.ToString();
    }
}