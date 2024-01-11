namespace LilySwapper.Workspace.Usercontrols.Overlays;

/// <summary>
///     Interaction logic for NotesView.xaml
/// </summary>
public partial class NotesView : UserControl
{
    private bool IsLoaded;

    public NotesView()
    {
        InitializeComponent();
    }

    private void NotesView_Loaded(object sender, RoutedEventArgs e)
    {
        Header.Text = string.Format(Languages.Read(Languages.Type.View, "NotesView", "Header"), Global.Version);

        if (IsLoaded)
            return;

        var Parse = Endpoint.Read(Endpoint.Type.Version);

        foreach (string Item in Parse[Global.Version]["Notes"]) Notes.Text += $"{Item}\n";
        Notes.Text.Remove(Notes.Text.Length - 1);

        IsLoaded = true;
    }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        Memory.MainView.RemoveOverlay();
    }
}