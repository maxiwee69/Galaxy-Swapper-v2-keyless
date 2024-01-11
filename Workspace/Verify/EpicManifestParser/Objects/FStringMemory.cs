namespace LilySwapper.Workspace.Verify.EpicManifestParser.Objects;

public readonly struct FStringMemory
{
    public Memory<byte> Memory { get; }
    public bool IsUnicode { get; }

    public Span<byte> GetSpan()
    {
        return Memory.Span;
    }

    public bool IsEmpty()
    {
        return Memory.IsEmpty;
    }

    public Encoding GetEncoding()
    {
        return IsUnicode ? Encoding.Unicode : Encoding.UTF8;
    }

    public FStringMemory(Memory<byte> memory, bool isUnicode)
    {
        Memory = memory;
        IsUnicode = isUnicode;
    }

    public override string ToString()
    {
        return GetEncoding().GetString(GetSpan());
    }
}