namespace LilySwapper.Workspace.CProvider.Objects;

public struct FZenPackageSummary
{
    public uint bHasVersioningInfo;
    public uint HeaderSize;
    public FMappedName Name;
    public EPackageFlags PackageFlags;
    public uint CookedHeaderSize;
    public int ImportedPublicExportHashesOffset;
    public int ImportMapOffset;
    public int ExportMapOffset;
    public int ExportBundleEntriesOffset;
    public int GraphDataOffset;
    public int DependencyBundleHeadersOffset;
    public int DependencyBundleEntriesOffset;
    public int ImportedPackageNamesOffset;
}