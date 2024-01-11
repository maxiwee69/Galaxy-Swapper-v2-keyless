using Colors = LilySwapper.Workspace.Properties.Colors;

namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CTabControl.xaml
/// </summary>
public partial class CTabControl : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CTabControl));

    public static readonly DependencyProperty IconClickedProperty =
        DependencyProperty.Register("IconClicked", typeof(ImageSource), typeof(CTabControl));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Brush), typeof(CTabControl),
            new PropertyMetadata(Colors.External_Brush));

    public static readonly DependencyProperty PresenceProperty =
        DependencyProperty.Register("Presence", typeof(string), typeof(CNewsControl));

    public CTabControl()
    {
        InitializeComponent();
    }

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ImageSource IconClicked
    {
        get => (ImageSource)GetValue(IconClickedProperty);
        set => SetValue(IconClickedProperty, value);
    }

    public Brush Color
    {
        get => (Brush)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public string Presence
    {
        get => (string)GetValue(PresenceProperty);
        set => SetValue(PresenceProperty, value);
    }

    public void Tab_Default()
    {
        Clicked.Visibility = Visibility.Hidden;
        Default.Visibility = Visibility.Visible;
        Color = Colors.External_Brush;
    }

    public void Tab_Clicked()
    {
        Default.Visibility = Visibility.Hidden;
        Clicked.Visibility = Visibility.Visible;
        Color = Colors.Blue_Brush;
        Utilities.Presence.Update(Presence);
    }

    private void CTabControl_MouseEnter(object sender, MouseEventArgs e)
    {
        Width = 42;
        Height = 42;
        Margin = new Thickness(7);
    }

    private void CTabControl_MouseLeave(object sender, MouseEventArgs e)
    {
        Width = 40;
        Height = 40;
        Margin = new Thickness(8);
    }
}