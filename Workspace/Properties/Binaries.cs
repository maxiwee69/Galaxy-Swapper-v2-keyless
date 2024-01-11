using System.Reflection;

namespace LilySwapper.Workspace.Properties;

public static class Binaries
{
    public static readonly string Path = App.Config + "\\Binaries";

    private static readonly List<BinaryData> BinaryData = new()
    {
        new BinaryData("8A06BB79F400AEC933CD8BADD5B63DF54DA5B02B95CF7C0EA29E0CDB40F72E6E", "oo2core_5_win64.dll",
            "Galaxy_Swapper_v2.Workspace.Assets.oo2core_5_win64.dll")
    };

    public static void Initialize()
    {
        Win32.SetDllDirectory(Path);

        foreach (var binarydata in BinaryData)
        {
            var path = Path + "\\" + binarydata.Name;
            if (File.Exists(path))
            {
                if (!path.CanEdit())
                {
                    Log.Warning(path + " is currently in use and we can't verify It's hash");
                    continue;
                }

                if (Misc.Hash(path) == binarydata.Hash)
                {
                    Log.Information($"{path} hash OK");
                    continue;
                }

                Log.Warning(path + " hash miss matched! Attempting to remove");
                File.Delete(path);
            }

            Log.Information("Attempting to generate " + binarydata.Name + " from resource " + binarydata.Path);

            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(binarydata.Path);
            using var destination = new FileStream(path, FileMode.Create);

            try
            {
                stream.CopyTo(destination);
                stream.Close();
                destination.Close();
            }
            catch (Exception exception)
            {
                Log.Fatal("Failed to write resource from " + binarydata.Path + " to " + path);
                throw;
            }
        }
    }
}