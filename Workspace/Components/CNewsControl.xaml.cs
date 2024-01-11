namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CNewsControl.xaml
/// </summary>
public partial class CNewsControl : UserControl
{
    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register("Header", typeof(string), typeof(CNewsControl));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register("Description", typeof(string), typeof(CNewsControl));

    public static readonly DependencyProperty URLProperty =
        DependencyProperty.Register("URL", typeof(string), typeof(CNewsControl));

    public static readonly DependencyProperty NewsProperty =
        DependencyProperty.Register("News", typeof(ImageSource), typeof(CNewsControl));

    public CNewsControl()
    {
        InitializeComponent();
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

    public string URL
    {
        get => (string)GetValue(URLProperty);
        set => SetValue(URLProperty, value);
    }

    public ImageSource News
    {
        get => (ImageSource)GetValue(NewsProperty);
        set => SetValue(NewsProperty, value);
    }

    private void News_Click(object sender, MouseButtonEventArgs e)
    {
        if (!string.IsNullOrEmpty(URL))
            URL.UrlStart();
    }
}