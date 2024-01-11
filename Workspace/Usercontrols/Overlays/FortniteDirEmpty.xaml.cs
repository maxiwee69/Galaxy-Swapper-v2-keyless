using System.ComponentModel;
using System.Threading;

namespace LilySwapper.Workspace.Usercontrols.Overlays;

public partial class FortniteDirEmpty : UserControl
{
    private readonly string[] UsableProcesses =
    {
        "FortniteClient-Win64-Shipping_BE", "FortniteClient-Win64-Shipping_EAC", "FortniteClient-Win64-Shipping_EAC_EOS"
    };

    private BackgroundWorker DetectWorker;
    private bool EndWorker;

    public FortniteDirEmpty()
    {
        InitializeComponent();
    }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        EndWorker = true;
        Memory.MainView.RemoveOverlay();
    }

    private void FortniteDirEmpty_Loaded(object sender, RoutedEventArgs e)
    {
        Header.Text = Languages.Read(Languages.Type.View, "FortniteDirEmpty", "Header");
        Description.Text = Languages.Read(Languages.Type.View, "FortniteDirEmpty", "Description");

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

            foreach (var processName in UsableProcesses)
            {
                var usableProcess = Process.GetProcessesByName("FortniteClient-Win64-Shipping_EAC_EOS");

                if (usableProcess.Length != 0)
                {
                    var path = usableProcess[0].MainModule.FileName;

                    Log.Information($"Detected {processName} ({usableProcess[0].Id}) with path: {path}");

                    if (path.Contains("\\FortniteGame"))
                        path = path.Split("\\FortniteGame").First();

                    if (!Directory.Exists($"{path}\\FortniteGame\\Content\\Paks"))
                    {
                        Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Error"),
                            string.Format(Languages.Read(Languages.Type.Message, "DetectedInvalidPath"), path),
                            discord: true);
                        return;
                    }

                    Settings.Edit(Settings.Type.Installtion, path);
                    Log.Information($"Set new path to: {path}");

                    EpicGamesLauncher.Close();
                    Message.DisplaySTA(Languages.Read(Languages.Type.Header, "Info"),
                        string.Format(Languages.Read(Languages.Type.Message, "DetectedNewPath"), path));
                    return;
                }
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