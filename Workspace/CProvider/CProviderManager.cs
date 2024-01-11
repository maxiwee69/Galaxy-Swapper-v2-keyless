using System.Threading.Tasks;
using LilySwapper.Workspace.Swapping.Providers;

namespace LilySwapper.Workspace.CProvider;

public static class CProviderManager
{
    public const string Paks = "\\FortniteGame\\Content\\Paks";
    public static DefaultFileProvider DefaultProvider = null!, UEFNProvider = null!;

    public static void InitDefault()
    {
        if (DefaultProvider is not null)
            return;

        var paks = $"{Settings.Read(Settings.Type.Installtion)}{Paks}";

        if (paks is null || string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            throw new FortniteDirectoryEmptyException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));

        Pakchunks.Validate(paks);
        AesProvider.Initialize();

        DefaultProvider = new DefaultFileProvider(new DirectoryInfo(paks));
        DefaultProvider.SubmitKeys(AesProvider.Keys);

        var parse = Endpoint.Read(Endpoint.Type.UEFN);

        if (parse["Enabled"].Value<bool>())
            DefaultProvider.Initialize(parse["Slots"].ToObject<List<string>>());
        else
            DefaultProvider.Initialize();

        WaitForEpicGames();
    }

    public static void InitUEFN()
    {
        if (UEFNProvider is not null)
            return;

        var paks = $"{Settings.Read(Settings.Type.Installtion)}{Paks}";

        if (paks is null || string.IsNullOrEmpty(paks) || !Directory.Exists(paks))
            throw new CustomException(Languages.Read(Languages.Type.Message, "FortniteDirectoryEmpty"));

        var parse = Endpoint.Read(Endpoint.Type.UEFN);

        UEFNProvider = new DefaultFileProvider(new DirectoryInfo(paks));
        UEFNProvider.Initialize(parse["Slots"].ToObject<List<string>>(), true);
    }

    public static string FormatUEFNGamePath(string path)
    {
        const string game = "/game/";
        const string plugin = "fortnitegame/plugins/gamefeatures/";
        var search = path.ToLower();
        var formatted = UEFNProvider.FindGameFile(search);

        if (string.IsNullOrEmpty(formatted))
        {
            Log.Error($"Could not find suitable UEFN game path: {path}");
            throw new CustomException(
                $"Failed to find suitable UEFN game path for:\n{path}\nEnsure the custom plugin path is correct and the game file contains the asset.");
        }

        if (formatted.ToLower().StartsWith(plugin))
        {
            formatted = formatted.Substring(plugin.Length);
        }
        else if (formatted.ToLower().StartsWith(game)) //It should never start with /Game/ but just in case.
        {
            formatted = formatted.Substring(game.Length);
        }
        else
        {
            Log.Error($"Failed to format UEFN game path:\n{path}\nDoes not start with {game} or {plugin}");
            throw new CustomException($"Failed to format UEFN game path:\n{path}");
        }

        formatted = string.Format("/{0}{1}", formatted.Split('/').First(), path);
        Log.Information($"Created new UEFN game path: {formatted}");

        return formatted;
    }

    private static void WaitForEpicGames()
    {
        Task.Run(async () =>
        {
            while (true)
            {
                if (DefaultProvider is null) return;
                if (EpicGamesLauncher.IsOpen())
                {
                    Log.Warning("Detected Fortnite being opened! Closing swapper to prevent read errors");
                    Environment.Exit(0);
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        });
    }
}