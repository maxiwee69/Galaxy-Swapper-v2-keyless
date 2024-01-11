using Colors = LilySwapper.Workspace.Properties.Colors;

namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CMinimizeControl.xaml
/// </summary>
public partial class CMinimizeControl : UserControl
{
    public CMinimizeControl()
    {
        InitializeComponent();
    }

    private void Minimize_Hover(object sender, MouseEventArgs e)
    {
        Minimize.Background = Colors.ControlHover_Brush;
    }

    private void Minimize_Leave(object sender, MouseEventArgs e)
    {
        Minimize.Background = Brushes.Transparent;
    }
}