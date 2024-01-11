using System.ComponentModel;
using System.Threading;

namespace LilySwapper.Workspace.Usercontrols.Overlays;

public partial class EpicGamesLauncherDirEmpty : UserControl
{
    private BackgroundWorker DetectWorker;
    private bool EndWorker;

    public EpicGamesLauncherDirEmpty()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        EndWorker = true;
        Memory.MainView.RemoveOverlay();
    }

    private void EpicGamesLauncherDirEmpty_Loaded(object sender, RoutedEventArgs e)
    {
        Header.Text = Languages.Read(Languages.Type.View, "EpicGamesLauncherDirEmpty", "Header");
        Description.Text = Languages.Read(Languages.Type.View, "EpicGamesLauncherDirEmpty", "Description");

        DetectWorker = new BackgroundWorker();
        DetectWorker.DoWork += DetectWorker_DoWork;
        DetectWorker.RunWorkerCompleted += DetectWorker_Completed;
        DetectWorker.RunWorkerAsync();
    }

    private async void DetectWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        while (true)
        {
            if (EndWorker)
                return;

            var epicgamesLauncher = Process.GetProcessesByName("EpicGamesLauncher");

            if (epicgamesLauncher.Length != 0)
            {
                var path = epicgamesLauncher[0].MainModule.FileName;

                Log.Information($"Detected EpicGamesLauncher ({epicgamesLauncher[0].Id}) with path: {path}");

                if (path.Contains("\\Epic Games"))
                    path = path.Split("\\Epic Games").First();

                if (!Directory.Exists($"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64") ||
                    !File.Exists($"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe"))
                {
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                        Languages.Read(Languages.Type.Message, "EpicDetectedInvalidPath"));
                    return;
                }

                path = $"{path}\\Epic Games\\Launcher\\Portal\\Binaries\\Win64\\EpicGamesLauncher.exe";

                Settings.Edit(Settings.Type.EpicInstalltion, path);
                Log.Information($"Set new path to: {path}");

                EpicGamesLauncher.Close();
                Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                    string.Format(Languages.Read(Languages.Type.Message, "EpicDetectedNewPath"), path));
                return;
            }

            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }

    private void DetectWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
    {
        Memory.SettingsView.SettingsView_Loaded(null!, null!);
        Close_Click(null!, null!);
    }
}