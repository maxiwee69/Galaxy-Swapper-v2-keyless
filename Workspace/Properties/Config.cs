namespace LilySwapper.Workspace.Properties;

public static class Config
{
    public static readonly string Path =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Galaxy-Swapper-v2-Config";

    public static readonly string[] Directories = { Path, $"{Path}\\DLLS", $"{Path}\\Plugins", $"{Path}\\LOGS" };

    public static void Initialize()
    {
        if (File.Exists($"{Path}\\Key.config")) //Old Config Folder
            Directory.Delete(Path, true);

        foreach (var Dir in Directories) Directory.CreateDirectory(Dir);
    }
}