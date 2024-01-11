namespace LilySwapper.Workspace.Verify.EpicManifestParser.Enums;

public enum EManifestMetaVersion : byte
{
    Original = 0,
    SerialisesBuildId,
    LatestPlusOne,
    Latest = LatestPlusOne - 1
}