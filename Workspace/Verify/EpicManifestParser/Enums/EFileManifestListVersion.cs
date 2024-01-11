namespace LilySwapper.Workspace.Verify.EpicManifestParser.Enums;

public enum EFileManifestListVersion : byte
{
    Original = 0,
    LatestPlusOne,
    Latest = LatestPlusOne - 1
}