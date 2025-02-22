﻿using System.ComponentModel;
using System.Threading;
using LilySwapper.Workspace.CProvider;
using LilySwapper.Workspace.Hashes;
using LilySwapper.Workspace.Verify.EpicGames;
using LilySwapper.Workspace.Verify.EpicManifestParser.Objects;

namespace LilySwapper.Workspace.Usercontrols.Overlays;

public partial class VerifyView : UserControl
{
    public enum Type
    {
        None, // ?
        Info,
        Warning,
        Error
    }

    public VerifyView()
    {
        InitializeComponent();
        Worker = new BackgroundWorker();
        Worker.DoWork += Worker_Convert;
        Worker.RunWorkerAsync();
    }

    private BackgroundWorker Worker { get; } = default!;

    private void VerifyView_Loaded(object sender, RoutedEventArgs e)
    {
        Header.Text = Languages.Read(Languages.Type.View, "VerifyView", "Header");
        Description.Text = Languages.Read(Languages.Type.View, "VerifyView", "Description");
    }

    public void Output(string content, Type type)
    {
        Dispatcher.Invoke(() =>
        {
            if (OutputBlock.ActualHeight > 350) OutputBlock.Text = string.Empty;

            OutputBlock.Text += $"[LOG] {content}\n";

            switch (type)
            {
                case Type.Info:
                    Log.Information(content);
                    break;
                case Type.Warning:
                    Log.Warning(content);
                    break;
                case Type.Error:
                    Log.Error(content);
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    break;
            }
        });
    }

    private void Exit()
    {
        Memory.MainView.RemoveOverlay();
    }

    private void Worker_Convert(object sender, DoWorkEventArgs e)
    {
        var installtion = $"{Settings.Read(Settings.Type.Installtion)}";
        var pakDirectoryInfo = new DirectoryInfo($"{installtion}\\FortniteGame\\Content\\Paks");

        Output("Disposing providers", Type.Info);
        CProviderManager.DefaultProvider?.Dispose();
        CProviderManager.DefaultProvider = null!;
        CProviderManager.UEFNProvider?.Dispose();
        CProviderManager.UEFNProvider = null!;

        Output("Restoring Epic Games Launcher", Type.Info);
        CustomEpicGamesLauncher.Revert();

        Output("Removing UEFN game files", Type.Info);
        UEFN.Clear(pakDirectoryInfo.FullName);

        Output("Attempting to download Fortnite manifest file", Type.Info);

        var oauth = new OAuth();
        if (!oauth.Download(this))
        {
            Message.DisplaySTA("Error", "Failed to establish a connection to the Epic Games endpoint!",
                solutions: new[]
                    { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
            Exit();
            return;
        }

        var liveManifest = new LiveManifest();
        if (!liveManifest.Download(this, oauth.Access_Token))
        {
            Message.DisplaySTA("Error", "Failed to establish a connection to the Epic Games endpoint!",
                solutions: new[]
                    { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" });
            Exit();
            return;
        }

        Output("Parsing manifest", Type.Info);

        var manifestBufer = new ManifestInfo(liveManifest.Parse.ToString()).DownloadManifestData();
        var manifest = new Manifest(manifestBufer);

        Output("Populating file list", Type.Info);

        var pakFileManifests = new List<FileManifest>();
        foreach (var fileManifest in manifest.FileManifests)
        {
            var directoryPath = fileManifest.Name.SubstringBeforeWithLast('/');
            if (directoryPath != "FortniteGame/Content/Paks/") continue;

            pakFileManifests.Add(fileManifest);
        }

        Output("Validating game files", Type.Info);

        foreach (var gameFile in pakDirectoryInfo.GetFiles())
        {
            if (!gameFile.Exists || gameFile.Name.ToUpper().Contains("UEFN") ||
                gameFile.Name.ToUpper().Contains("UNREALEDITOR") || gameFile.Name.ToUpper().Contains(".O."))
                continue;

            Output($"Validating {gameFile.Name}", Type.Info);

            var fileManifest = pakFileManifests.Find(x => x.Name.Contains(gameFile.Name));
            if (fileManifest is null)
            {
                Output($"Deleting: {gameFile.Name} unknown file", Type.Warning);
                File.Delete(gameFile.FullName);
                continue;
            }

            if (gameFile.Length > 2090000000)
            {
                Output($"{gameFile.Name} is a large file and wont be hash checked", Type.Warning);
                continue;
            }

            using (var fileStream = File.OpenRead(gameFile.FullName))
            {
                if (SHA1Hash.HashStream(fileStream) == fileManifest.Hash)
                {
                    Output($"{gameFile.Name} hash Is OK", Type.Info);
                    fileStream.Close();
                }
                else
                {
                    Output($"Deleting: {gameFile.Name} hash Is invalid", Type.Warning);
                    fileStream.Close();
                    File.Delete(gameFile.FullName);
                }
            }
        }

        Output("Checking for left over .backup files", Type.Info);

        foreach (var gamefile in pakDirectoryInfo.GetFiles("*.backup*", SearchOption.TopDirectoryOnly))
        {
            Output($"Deleting: {gamefile.Name}", Type.Info);
            File.Delete(gamefile.FullName);
        }

        Output("Scanning for unknown game directories", Type.Info);
        foreach (var gameDirectoryInfo in pakDirectoryInfo.GetDirectories())
        {
            Output($"Deleting: {gameDirectoryInfo.Name}", Type.Info);
            Directory.Delete(gameDirectoryInfo.FullName, true);
        }

        SwapLogs.Clear();
        EpicGamesLauncher.Verify();

        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
            Languages.Read(Languages.Type.Message, "Verify"));
        Environment.Exit(0);
    }
}