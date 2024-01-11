namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CSettingsControl.xaml
/// </summary>
public partial class CSettingsControl : UserControl
{
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(CSettingsControl));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(CSettingsControl));

    public CSettingsControl()
    {
        InitializeComponent();
        //Additional = new ObservableCollection<UIElement>();
    }

    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}