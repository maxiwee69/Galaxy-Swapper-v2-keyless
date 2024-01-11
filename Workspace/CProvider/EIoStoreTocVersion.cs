namespace LilySwapper.Workspace.CProvider;

public enum EIoStoreTocVersion : byte
{
    Invalid = 0,
    Initial,
    DirectoryIndex,
    PartitionSize,
    PerfectHash,
    PerfectHashWithOverflow,
    LatestPlusOne,
    Latest = LatestPlusOne - 1
}