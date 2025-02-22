﻿namespace LilySwapper.Workspace.Swapping;

public static class LobbyBGSwap
{
    private static readonly string PersistentDownloadDir =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\FortniteGame\\Saved\\PersistentDownloadDir\\CMS";

    private static readonly string DownloadCache = "DownloadCache.json";

    private static string[] List()
    {
        var paths = new List<string>();

        if (!Directory.Exists(PersistentDownloadDir) || !File.Exists($"{PersistentDownloadDir}\\{DownloadCache}"))
        {
            Log.Error(
                $"Caught exception while converting lobby screen: Directory does not exist: {PersistentDownloadDir}");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                string.Format(Languages.Read(Languages.Type.Message, "PersistentDownloadDirError"),
                    PersistentDownloadDir), discord: true,
                solutions: Languages.ReadSolutions(Languages.Type.Message, "PersistentDownloadDirError"));
            return null!;
        }

        var content = File.ReadAllText($"{PersistentDownloadDir}\\{DownloadCache}");

        if (!content.ValidJson())
        {
            Log.Error(
                "Caught exception while converting lobby screen: DownloadCache.json is not in a valid JSON format");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                Languages.Read(Languages.Type.Message, "DownloadCacheInvalidJSON"), discord: true,
                solutions: Languages.ReadSolutions(Languages.Type.Message, "DownloadCacheInvalidJSON"));
            return null!;
        }

        var parse = JObject.Parse(content);

        foreach (var cache in parse["cache"] as JObject)
        {
            var key = cache.Key;

            if (!key.ToLower().Contains("lobby"))
                continue;

            var filePath = cache.Value["filePath"].Value<string>();

            if (File.Exists(filePath))
            {
                if (!filePath.CanEdit())
                {
                    Log.Error($"{filePath} is currently in use!");
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                        $"{filePath} is currently in use! Please close Fortnite and try swapping again.",
                        discord: true);
                    return null!;
                }

                paths.Add(filePath);
                Log.Information($"Found Lobby BG at: {filePath}");
            }
        }

        return paths.ToArray();
    }

    public static void Convert(string url)
    {
        var Stopwatch = new Stopwatch();
        Stopwatch.Start();

        var paths = List();

        if (paths is null)
            return;

        using (var WC = new WebClient())
        {
            try
            {
                foreach (var path in paths)
                {
                    File.Delete(path);
                    Log.Information($"Removed: {path}");

                    WC.DownloadFile(url, path);
                    Log.Information($"Downloaded: {url} to {path}");
                }
            }
            catch (Exception Exception)
            {
                Log.Error(Exception, $"Failed to download {url}");
                Message.DisplaySTA("Error", "Webclient caught a exception while downloading lobby background!",
                    discord: true,
                    solutions: new[]
                        { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
                return;
            }
        }

        var TimeSpan = Stopwatch.Elapsed;
        if (TimeSpan.Minutes > 0)
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"), TimeSpan.Minutes));
        else
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), TimeSpan.Seconds));
    }

    public static void Convert(FileInfo fileInfo)
    {
        var Stopwatch = new Stopwatch();
        Stopwatch.Start();

        var paths = List();

        if (paths is null)
            return;

        foreach (var path in paths)
        {
            File.Delete(path);
            Log.Information($"Removed: {path}");

            File.Copy(fileInfo.FullName, path, true);
            Log.Information($"Copied: {fileInfo.FullName} to {path}");
        }

        var TimeSpan = Stopwatch.Elapsed;
        if (TimeSpan.Minutes > 0)
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"), TimeSpan.Minutes));
        else
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), TimeSpan.Seconds));
    }

    public static void Revert()
    {
        var Stopwatch = new Stopwatch();
        Stopwatch.Start();

        var paths = List();

        if (paths is null)
            return;

        foreach (var path in paths)
        {
            File.Delete(path);
            Log.Information($"Removed: {path}");
        }

        var TimeSpan = Stopwatch.Elapsed;
        if (TimeSpan.Minutes > 0)
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "RevertedMinutes"), TimeSpan.Minutes));
        else
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                string.Format(Languages.Read(Languages.Type.View, "SwapView", "Reverted"), TimeSpan.Seconds));
    }
}