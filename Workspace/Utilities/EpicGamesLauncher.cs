using System.Text.RegularExpressions;

namespace LilySwapper.Workspace.Utilities;

public static class EpicGamesLauncher
{
    private const string LaunchArg = "com.epicgames.launcher://apps/Fortnite?action=launch&silent=true";
    private const string VerifyArg = "com.epicgames.launcher://apps/Fortnite?action=verify&silent=false";

    private static readonly string ProgramData =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)}\\Epic";

    private static readonly string SavedLogs =
        $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\EpicGamesLauncher\\Saved\\Logs";

    private static readonly string AppName = "Fortnite";

    private static readonly string[] Processes =
    {
        "EpicGamesLauncher", "FortniteLauncher", "FortniteClient-Win64-Shipping", "FortniteClient-Win64-Shipping_BE",
        "FortniteClient-Win64-Shipping_EAC", "FortniteClient-Win64-Shipping_EAC_EOS", "CrashReportClient"
    };

    public static string Installation()
    {
        return TryInstallation(out var location) ? location : string.Empty;
    }

    public static string FortniteInstallation()
    {
        return TryFortniteInstallation(out var location) ? location : null!;
    }

    public static void Launch()
    {
        LaunchArg.UrlStart();
    }

    public static void Close(bool Fortnite = true)
    {
        TryClose(Fortnite);
    }

    public static void Verify()
    {
        VerifyArg.UrlStart();
    }

    public static bool IsOpen()
    {
        return Process.GetProcessesByName("EpicGamesLauncher").Length != 0;
    }

    public static bool TryInstallation(out string location)
    {
        location = string.Empty;
        if (!Directory.Exists(SavedLogs) || !File.Exists($"{SavedLogs}\\EpicGamesLauncher.log")) return false;
        if (File.Exists($"{SavedLogs}\\EpicGamesLauncher.temp")) File.Delete($"{SavedLogs}\\EpicGamesLauncher.temp");
        File.Copy($"{SavedLogs}\\EpicGamesLauncher.log", $"{SavedLogs}\\EpicGamesLauncher.temp");
        foreach (var line in File.ReadAllLines($"{SavedLogs}\\EpicGamesLauncher.temp"))
        {
            var match = Regex.Match(line, @"LogInit: Base Directory: (.*)");
            if (match.Success)
            {
                var win64 = $"{match.Groups[1].Value.Trim()}EpicGamesLauncher.exe";
                if (!File.Exists(win64)) continue;
                location = win64;
                return true;
            }
        }

        return false;
    }

    public static bool TryFortniteInstallation(out string location)
    {
        location = null!;
        if (!Directory.Exists(ProgramData)) return false;
        var launcherinstalled = $"{ProgramData}\\UnrealEngineLauncher\\LauncherInstalled.dat";
        if (!File.Exists(launcherinstalled)) return false;
        var content = File.ReadAllText(launcherinstalled);
        if (string.IsNullOrEmpty(content) || !content.ValidJson()) return false;
        var parse = JObject.Parse(content);
        foreach (var installation in parse["InstallationList"])
            if (installation["AppName"].Value<string>() == AppName)
            {
                var installlocation = installation["InstallLocation"].Value<string>();
                if (!Directory.Exists(installlocation) ||
                    !Directory.Exists($"{installlocation}\\FortniteGame\\Content\\Paks")) continue;
                location = installlocation;
            }

        return location != null;
    }

    public static bool TryLaunch()
    {
        try
        {
            LaunchArg.UrlStart();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryClose(bool Fortnite = true)
    {
        try
        {
            foreach (var name in Processes)
            {
                var process = Process.GetProcessesByName(name);
                if (process.Length == 0) continue;
                process[0].Kill();
                process[0].WaitForExit();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryVerify()
    {
        try
        {
            VerifyArg.UrlStart();
            return true;
        }
        catch
        {
            return false;
        }
    }
}