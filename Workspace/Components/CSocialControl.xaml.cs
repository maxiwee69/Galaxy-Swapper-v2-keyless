namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CSocialControl.xaml
/// </summary>
public partial class CSocialControl : UserControl
{
    private readonly Social Social = null!;

    public CSocialControl(Social social)
    {
        InitializeComponent();
        Logo.LoadImage(social.Icon);
        Logo.ToolTip = social.Header;
        Social = social;
    }

    private void root_MouseEnter(object sender, MouseEventArgs e)
    {
        Margin = new Thickness(2.5, 0, 2.5, 0);
        Height += 5;
        Width += 5;
    }

    private void root_MouseLeave(object sender, MouseEventArgs e)
    {
        Margin = new Thickness(5, 0, 5, 0);
        Height -= 5;
        Width -= 5;
    }

    private void root_Click(object sender, MouseButtonEventArgs e)
    {
        if (string.IsNullOrEmpty(Social.URL))
            Message.Display(Languages.Read(Languages.Type.Header, "Info"), Social.Header);
        else if (Message.Display(Languages.Read(Languages.Type.Header, "Warning"),
                     string.Format(Languages.Read(Languages.Type.Message, "OpenUrl"), Social.URL),
                     MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes) Social.URL.UrlStart();
    }
}