﻿namespace LilySwapper.Workspace.CProvider;

public class FIoStoreTocHeader
{
    public readonly uint ChunkPerfectHashSeedsCount;
    public readonly uint ChunksWithoutPerfectHashCount;
    public readonly uint CompressedBlockEntryCount;
    public readonly uint CompressedBlockEntrySize;
    public readonly uint CompressionBlockSize;
    public readonly uint CompressionMethodNameCount;
    public readonly uint CompressionMethodNameLength;
    public readonly EIoContainerFlags ContainerFlags;
    public readonly FIoContainerId ContainerId;
    public readonly uint DirectoryIndexSize;
    public readonly FGuid EncryptionKeyGuid;
    public readonly uint EntryCount;

    public readonly byte[] HeaderMagic;
    public readonly uint HeaderSize;

    private readonly byte[] Magic =
        { 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D, 0x2D, 0x3D, 0x3D, 0x2D };

    public readonly uint PartitionCount;
    public readonly ulong PartitionSize;
    public readonly EIoStoreTocVersion Version;

    public FIoStoreTocHeader(Reader reader)
    {
        HeaderMagic = reader.ReadBytes(Magic.Length);

        if (!HeaderMagic.SequenceEqual(Magic))
        {
            Log.Error($"{reader.Name} has invalid magic!");
            throw new Exception($"{reader.Name} has invalid magic! Please verify your game files and try again.");
        }

        Version = reader.Read<EIoStoreTocVersion>();

        //_reserved0 _reserved1 not needed so skip
        reader.Position += sizeof(byte);
        reader.Position += sizeof(ushort);

        HeaderSize = reader.Read<uint>();
        EntryCount = reader.Read<uint>();
        CompressedBlockEntryCount = reader.Read<uint>();
        CompressedBlockEntrySize = reader.Read<uint>();
        CompressionMethodNameCount = reader.Read<uint>();
        CompressionMethodNameLength = reader.Read<uint>();
        CompressionBlockSize = reader.Read<uint>();
        DirectoryIndexSize = reader.Read<uint>();
        PartitionCount = reader.Read<uint>();
        ContainerId = reader.Read<FIoContainerId>();
        EncryptionKeyGuid = reader.Read<FGuid>();
        ContainerFlags = reader.Read<EIoContainerFlags>();

        //_reserved3 _reserved4 not needed so skip
        reader.Position += sizeof(byte);
        reader.Position += sizeof(ushort);

        ChunkPerfectHashSeedsCount = reader.Read<uint>();
        PartitionSize = reader.Read<ulong>();
        ChunksWithoutPerfectHashCount = reader.Read<uint>();

        //_reserved7 _reserved8 not needed so skip
        reader.Position += sizeof(uint);
        reader.Position += 5 * sizeof(ulong);
    }

    public bool Compare(FIoStoreTocHeader header)
    {
        if (HeaderSize != header.HeaderSize)
            return false;
        if (Version != header.Version)
            return false;
        if (EntryCount != header.EntryCount)
            return false;
        if (HeaderSize != header.HeaderSize)
            return false;
        if (CompressedBlockEntryCount != header.CompressedBlockEntryCount)
            return false;
        if (CompressedBlockEntrySize != header.CompressedBlockEntrySize)
            return false;
        if (CompressionMethodNameCount != header.CompressionMethodNameCount)
            return false;
        if (CompressionMethodNameLength != header.CompressionMethodNameLength)
            return false;
        if (CompressionBlockSize != header.CompressionBlockSize)
            return false;
        if (DirectoryIndexSize != header.DirectoryIndexSize)
            return false;
        if (PartitionCount != header.PartitionCount)
            return false;
        if (ContainerId.Id != header.ContainerId.Id)
            return false;
        if (EncryptionKeyGuid.A != header.EncryptionKeyGuid.A || EncryptionKeyGuid.B != header.EncryptionKeyGuid.B ||
            EncryptionKeyGuid.C != header.EncryptionKeyGuid.C || EncryptionKeyGuid.D != header.EncryptionKeyGuid.D)
            return false;
        if (ContainerFlags != header.ContainerFlags)
            return false;
        if (ChunkPerfectHashSeedsCount != header.ChunkPerfectHashSeedsCount)
            return false;
        if (PartitionSize != header.PartitionSize)
            return false;
        if (ChunksWithoutPerfectHashCount != header.ChunksWithoutPerfectHashCount)
            return false;

        return true;
    }
}