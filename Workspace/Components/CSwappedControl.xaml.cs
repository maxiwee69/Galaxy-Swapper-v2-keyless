namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CSwappedControl.xaml
/// </summary>
public partial class CSwappedControl : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CSwappedControl));

    public static readonly DependencyProperty OverrideIconProperty =
        DependencyProperty.Register("OverrideIcon", typeof(ImageSource), typeof(CSwappedControl));

    public CSwappedControl()
    {
        InitializeComponent();
    }

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public ImageSource OverrideIcon
    {
        get => (ImageSource)GetValue(OverrideIconProperty);
        set => SetValue(OverrideIconProperty, value);
    }
}