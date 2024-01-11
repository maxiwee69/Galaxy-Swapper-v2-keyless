namespace LilySwapper.Workspace.CProvider.Objects;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct FIoChunkId : IEquatable<FIoChunkId>
{
    public readonly ulong ChunkId;
    private readonly ushort _chunkIndex;
    private readonly byte _padding;
    public readonly byte ChunkType;

    public FIoChunkId(ulong chunkId, ushort chunkIndex, byte chunkType)
    {
        ChunkId = chunkId;
        _chunkIndex = (ushort)(((chunkIndex & 0xFF) << 8) | ((chunkIndex & 0xFF00) >> 8)); // NETWORK_ORDER16
        ChunkType = chunkType;
        _padding = 0;
    }

    public FIoChunkId(ulong chunkId, ushort chunkIndex, EIoChunkType chunkType) : this(chunkId, chunkIndex,
        (byte)chunkType)
    {
    }

    public FIoChunkId(ulong chunkId, ushort chunkIndex, EIoChunkType5 chunkType) : this(chunkId, chunkIndex,
        (byte)chunkType)
    {
    }

    public static bool operator ==(FIoChunkId left, FIoChunkId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FIoChunkId left, FIoChunkId right)
    {
        return !left.Equals(right);
    }

    public bool Equals(FIoChunkId other)
    {
        return ChunkId == other.ChunkId && ChunkType == other.ChunkType;
    }

    public override bool Equals(object? obj)
    {
        return obj is FIoChunkId other && Equals(other);
    }

    public override string ToString()
    {
        return $"0x{ChunkId:X8} | {ChunkType}";
    }
}