namespace LilySwapper.Workspace.CProvider.Objects;

public class GameFile
{
    public readonly FIoOffsetAndLength ChunkOffsetLengths;
    public readonly FIoStoreTocHeader IoStoreTocHeader;
    public readonly string Path;
    public readonly long Size;
    public readonly uint TocEntryIndex;
    public byte[] CompressedBuffer;
    public FIoStoreTocCompressedBlockEntry CompressionBlock;
    public bool IsEncrypted;
    public int LastPartition;
    public string LastUcas;
    public long Offset;
    public string Ucas;
    public byte[] UncompressedBuffer;
    public string Utoc;

    public GameFile(string path, uint tocentryindex, FIoOffsetAndLength chunkOffsetLengths,
        FIoStoreTocHeader ioStoreTocHeader)
    {
        Path = path;
        TocEntryIndex = tocentryindex;
        ChunkOffsetLengths = chunkOffsetLengths;
        Size = (long)ChunkOffsetLengths.Length;
        IoStoreTocHeader = ioStoreTocHeader;
    }

    public GameFile()
    {
    }
}