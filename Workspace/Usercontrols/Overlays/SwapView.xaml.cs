using System.ComponentModel;
using LilySwapper.Workspace.Components;
using LilySwapper.Workspace.Compression;
using LilySwapper.Workspace.CProvider;
using LilySwapper.Workspace.Swapping;
using LilySwapper.Workspace.Swapping.Providers;
using Colors = LilySwapper.Workspace.Properties.Colors;

namespace LilySwapper.Workspace.Usercontrols.Overlays;

public partial class SwapView : UserControl
{
    public enum Type
    {
        None, // ?
        Info,
        Warning,
        Error
    }

    private readonly Option Option;

    private bool IsWorkerBusy;

    public SwapView(Option option)
    {
        InitializeComponent();
        Option = option;
        DisplayName.Text = Option.Name;
    }

    private BackgroundWorker Worker { get; set; } = default!;

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        Memory.MainView.RemoveOverlay();
    }

    private void SwapView_Loaded(object sender, RoutedEventArgs e)
    {
        if (Option.Nsfw)
            if (Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"),
                    Languages.Read(Languages.Type.View, "SwapView", "NSFW"), MessageBoxButton.YesNo) ==
                MessageBoxResult.No)
            {
                Memory.MainView.RemoveOverlay();
                return;
            }


        Icon.LoadImage(Option.Icon);
        OverrideIcon.LoadImage(Option.OverrideIcon);

        Convert.Content = Languages.Read(Languages.Type.View, "SwapView", "Convert");
        Revert.Content = Languages.Read(Languages.Type.View, "SwapView", "Revert");

        foreach (var social in Option.Socials)
            Socials.Children.Add(new CSocialControl(social) { Margin = new Thickness(5, 0, 5, 0) });

        if (Option.UEFNFormat && SwapLogs.IsSwappedUEFNSwapped(Option.Name, out var revertitem))
        {
            Message.Display(Languages.Read(Languages.Type.Header, "Warning"),
                string.Format(Languages.Read(Languages.Type.Message, "UEFNRevert"), revertitem));
            Memory.MainView.RemoveOverlay();
            return;
        }

        if (SwapLogs.IsSwapped(Option.Name))
        {
            Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "ON");
            Converted.Foreground = Colors.Text;
        }
        else if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
        {
            SwapLogs.Read(out var Count, out var AssetCount, out var Ucas, out var Utoc);
            if (AssetCount + Option.Exports.Count > 13)
            {
                Message.Display(Languages.Read(Languages.Type.Header, "Warning"),
                    string.Format(Languages.Read(Languages.Type.Message, "MaxAssetCount"), 13));
                Memory.MainView.RemoveOverlay();
                return;
            }
        }
        else
        {
            Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "OFF");
        }

        if (Settings.Read(Settings.Type.CloseFortnite).Value<bool>())
            EpicGamesLauncher.Close();

        if (!string.IsNullOrEmpty(Option.Message) && Option.Message.ToLower() != "false")
            Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Option.Message);
        if (!string.IsNullOrEmpty(Option.OptionMessage) && Option.OptionMessage.ToLower() != "false")
            Message.Display(Languages.Read(Languages.Type.Header, "Warning"), Option.OptionMessage);

        Presence.Update(Option.Name);
    }

    public void Output(string Content, Type Type)
    {
        Dispatcher.Invoke(() =>
        {
            switch (Type)
            {
                case Type.Info:
                    LOG.Foreground = Colors.Text;
                    Log.Information(Content);
                    break;
                case Type.Warning:
                    LOG.Foreground = Colors.Yellow;
                    Log.Warning(Content);
                    break;
                case Type.Error:
                    LOG.Foreground = Colors.Red;
                    Log.Error(Content);
                    break;
            }

            LOG.Text = Content;

            if (LOG.Text.Length > 64)
                LOG.FontSize = 10F;
            else
                LOG.FontSize = 14F;
        });
    }

    private void Worker_Click(object sender, RoutedEventArgs e)
    {
        if (IsWorkerBusy)
        {
            Message.Display(Languages.Read(Languages.Type.Header, "Warning"),
                Languages.Read(Languages.Type.View, "SwapView", "WorkerBusy"));
            return;
        }

        IsWorkerBusy = true;

        var epicinstallation = Settings.Read(Settings.Type.EpicInstalltion).Value<string>();
        if (string.IsNullOrEmpty(epicinstallation) || !File.Exists(epicinstallation))
        {
            Log.Information(Languages.Read(Languages.Type.Message, "EpicGamesLauncherPathEmpty"));
            Memory.MainView.SetOverlay(new EpicGamesLauncherDirEmpty());
            return;
        }

        EpicGamesLauncher.Close();

        Worker = new BackgroundWorker();
        Worker.RunWorkerCompleted += Worker_Completed;

        if (((Button)sender).Name == "Convert")
            Worker.DoWork += Worker_Convert;
        else
            Worker.DoWork += Worker_Revert;

        var StoryBoard = Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = DisplayName, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 100) }
            },
            new Interface.BaseAnim
            {
                Element = LOG, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 100),
                    BeginTime = new TimeSpan(0, 0, 0, 0, 100)
                }
            });
        StoryBoard.Completed += delegate { Worker.RunWorkerAsync(); };
        StoryBoard.Begin();
    }

    private void Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
        IsWorkerBusy = false;
        Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = DisplayName, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation
                {
                    From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 100),
                    BeginTime = new TimeSpan(0, 0, 0, 0, 100)
                }
            },
            new Interface.BaseAnim
            {
                Element = LOG, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 100) }
            }).Begin();
    }

    private void Worker_Convert(object sender, DoWorkEventArgs e)
    {
        try
        {
            var Stopwatch = new Stopwatch();
            Stopwatch.Start();

            var paks = $"{Settings.Read(Settings.Type.Installtion).Value<string>()}{CProviderManager.Paks}";

            Output(Languages.Read(Languages.Type.View, "SwapView", "InitializingProvider"), Type.Info);
            CProviderManager.InitDefault();
            CProviderManager.InitUEFN();

            var Ucas = new List<string>();
            var Utocs = new List<string>();
            foreach (var Asset in Option.Exports)
            {
                Output(
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"),
                        Path.GetFileNameWithoutExtension(Asset.Object)), Type.Info);

                var exported = CProviderManager.DefaultProvider.Save(Asset.Object);

                if (exported is null)
                    throw new Exception($"Failed to export {Asset.Object}");

                Asset.Object = FormatObject(Asset.Object);
                Asset.Export = exported;

                if (!string.IsNullOrEmpty(Asset.OverrideObject))
                {
                    if (Asset.IsStreamData)
                    {
                        Output(Languages.Read(Languages.Type.View, "SwapView", "StreamData"), Type.Info);
                        var streamDataBuffer = StreamDataProvider.Download(Asset.OverrideObject);

                        if (streamDataBuffer is null) return;

                        Asset.OverrideExport = new GameFile { UncompressedBuffer = streamDataBuffer };
                        continue;
                    }

                    if (string.IsNullOrEmpty(Asset.OverrideBuffer))
                    {
                        Output(
                            string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"),
                                Path.GetFileNameWithoutExtension(Asset.OverrideObject)), Type.Info);

                        var overrideExported = CProviderManager.DefaultProvider.Save(Asset.OverrideObject);

                        if (overrideExported is null)
                            throw new Exception($"Failed to export {Asset.OverrideObject}");

                        Asset.OverrideExport = overrideExported;
                    }
                    else
                    {
                        Output(Languages.Read(Languages.Type.View, "SwapView", "Decompressing"), Type.Info);
                        Asset.OverrideExport = new GameFile
                            { UncompressedBuffer = gzip.Decompress(Asset.OverrideBuffer) };
                    }

                    Asset.OverrideObject = FormatObject(Asset.OverrideObject);
                }

                Ucas.AddRange(Ucas.Contains(Asset.Export.Ucas)
                    ? Enumerable.Empty<string>()
                    : new[] { Asset.Export.Ucas });
                Utocs.AddRange(Utocs.Contains(Asset.Export.Utoc)
                    ? Enumerable.Empty<string>()
                    : new[] { Asset.Export.Utoc });
            }

            if (Settings.Read(Settings.Type.KickWarning).Value<bool>())
            {
                SwapLogs.Kick(out var UcasK, out var UtocK, Ucas, Utocs);

                if (UtocK)
                {
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Warning"),
                        string.Format(Languages.Read(Languages.Type.Message, "MaxPakChunkCount"), "Utoc", 2));
                    return;
                }
            }

            if (Option.UseMainUEFN)
            {
                Output(Languages.Read(Languages.Type.View, "SwapView", "DownloadingMainUEFN"), Type.Info);
                UEFN.DownloadMain(paks, Option.UEFNTag);
            }

            if (Option.Downloadables != null && Option.Downloadables.Count > 0)
            {
                Output(Languages.Read(Languages.Type.View, "SwapView", "DownloadingUEFN"), Type.Info);
                foreach (var downloadable in Option.Downloadables) UEFN.Add(paks, Option.Name, downloadable);
            }

            Output(Languages.Read(Languages.Type.View, "SwapView", "ModifyingEpicGamesLauncher"), Type.Info);
            CustomEpicGamesLauncher.Convert();

            Output(Languages.Read(Languages.Type.View, "SwapView", "ConvertingAssets"), Type.Info);

            foreach (var Export in Option.Exports)
            {
                var Swap = new Swap(this, null, Export);
                if (!Swap.Convert())
                    return;
            }

            if (Option.Cosmetic is not null && !Option.IsPlugin &&
                Settings.Read(Settings.Type.ShareStats).Value<bool>())
                CosmeticTracker.Update(Option.Cosmetic.Name, Option.Cosmetic.ID, Option.Cosmetic.Type.ToString());

            SwapLogs.Add(Option.Name, Option.Icon, Option.OverrideIcon, Option.Exports.Count, Ucas, Utocs,
                Option.UEFNFormat);

            Dispatcher.Invoke(() =>
            {
                Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "ON");
                Converted.Foreground = Colors.Text;
            });

            var TimeSpan = Stopwatch.Elapsed;
            if (TimeSpan.Minutes > 0)
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "ConvertedMinutes"),
                        TimeSpan.Minutes));
            else
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "Converted"), TimeSpan.Seconds));
        }
        catch (FortniteDirectoryEmptyException Exception)
        {
            Dispatcher.Invoke(() =>
            {
                Log.Error(Exception.Message, "Fortnite directory is null or empty");
                Memory.MainView.SetOverlay(new FortniteDirEmpty());
            });
        }
        catch (CustomException CustomException)
        {
            Log.Error(CustomException.Message, "Caught CustomException");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                string.Format(Languages.Read(Languages.Type.Message, "ConvertError"), Option.Name,
                    CustomException.Message), discord: true);
        }
        catch (Exception Exception)
        {
            Log.Error(Exception.Message, "Caught Exception");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                string.Format(Languages.Read(Languages.Type.Message, "ConvertError"), Option.Name, Exception.Message),
                solutions: Languages.ReadSolutions(Languages.Type.Message, "ConvertError"), discord: true);
        }
    }

    private void Worker_Revert(object sender, DoWorkEventArgs e)
    {
        try
        {
            var Stopwatch = new Stopwatch();
            Stopwatch.Start();

            Output(Languages.Read(Languages.Type.View, "SwapView", "InitializingProvider"), Type.Info);
            CProviderManager.InitDefault();

            foreach (var Asset in Option.Exports)
            {
                Output(
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "Exporting"),
                        Path.GetFileNameWithoutExtension(Asset.Object)), Type.Info);

                var exported = CProviderManager.DefaultProvider.Save(Asset.Object);

                if (exported is null)
                    throw new Exception($"Failed to export {Asset.Object}");

                Asset.Object = FormatObject(Asset.Object);
                Asset.Export = exported;
            }

            Output(Languages.Read(Languages.Type.View, "SwapView", "RevertingAssets"), Type.Info);

            foreach (var Export in Option.Exports)
            {
                var Swap = new Swap(this, null, Export);
                if (!Swap.Revert())
                    return;
            }

            if (Option.Downloadables != null && Option.Downloadables.Count > 0)
            {
                Output(Languages.Read(Languages.Type.View, "SwapView", "RemovingUEFN"), Type.Info);
                foreach (var downloadable in Option.Downloadables) UEFN.Remove(Option.Name);
            }

            SwapLogs.Remove(Option.Name);

            Dispatcher.Invoke(() =>
            {
                Converted.Text = Languages.Read(Languages.Type.View, "SwapView", "OFF");
                Converted.Foreground = Colors.Text2;
            });

            var TimeSpan = Stopwatch.Elapsed;
            if (TimeSpan.Minutes > 0)
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "RevertedMinutes"),
                        TimeSpan.Minutes));
            else
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                    string.Format(Languages.Read(Languages.Type.View, "SwapView", "Reverted"), TimeSpan.Seconds));
        }
        catch (FortniteDirectoryEmptyException Exception)
        {
            Dispatcher.Invoke(() =>
            {
                Log.Error(Exception.Message, "Fortnite directory is null or empty");
                Memory.MainView.SetOverlay(new FortniteDirEmpty());
            });
        }
        catch (CustomException CustomException)
        {
            Log.Error(CustomException.Message, "Caught CustomException");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                string.Format(Languages.Read(Languages.Type.Message, "RevertError"), Option.Name,
                    CustomException.Message), discord: true);
        }
        catch (Exception Exception)
        {
            Log.Error(Exception.Message, "Caught Exception");
            Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                string.Format(Languages.Read(Languages.Type.Message, "RevertError"), Option.Name, Exception.Message),
                discord: true, solutions: Languages.ReadSolutions(Languages.Type.Message, "ConvertError"));
        }
    }

    private static string FormatObject(string Path)
    {
        return (Path.Contains('.') ? Path.Split('.').First() : Path).Replace("FortniteGame/Content/", "/Game/")
            .Replace("FortniteGame/Plugins/GameFeatures/BRCosmetics/Content/", "/BRCosmetics/") + ".uasset";
    }
}