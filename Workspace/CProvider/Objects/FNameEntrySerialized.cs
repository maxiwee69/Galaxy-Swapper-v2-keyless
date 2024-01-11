namespace LilySwapper.Workspace.CProvider.Objects;

public struct FNameEntrySerialized
{
    public string? Name;
    public ulong hashVersion;
#if NAME_HASHES
        public readonly ushort NonCasePreservingHash;
        public readonly ushort CasePreservingHash;
#endif

    public FNameEntrySerialized(string name, ulong HashVersion = 0)
    {
        Name = name;
        hashVersion = HashVersion;
    }
}