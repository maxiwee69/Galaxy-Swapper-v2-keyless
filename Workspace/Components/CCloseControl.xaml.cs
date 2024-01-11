using Colors = LilySwapper.Workspace.Properties.Colors;

namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CCloseControl.xaml
/// </summary>
public partial class CCloseControl : UserControl
{
    public CCloseControl()
    {
        InitializeComponent();
    }

    private void Close_Hover(object sender, MouseEventArgs e)
    {
        Close.Background = Colors.Red;
    }

    private void Close_Leave(object sender, MouseEventArgs e)
    {
        Close.Background = Brushes.Transparent;
    }
}