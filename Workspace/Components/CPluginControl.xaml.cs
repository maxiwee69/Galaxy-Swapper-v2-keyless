using LilySwapper.Workspace.Plugins;
using LilySwapper.Workspace.Usercontrols;

namespace LilySwapper.Workspace.Components;

/// <summary>
///     Interaction logic for CPluginControl.xaml
/// </summary>
public partial class CPluginControl : UserControl
{
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register("Icon", typeof(ImageSource), typeof(CPluginControl));

    private bool IsReImporting;

    public CPluginControl(PluginsView pluginsview, PluginData plugindata, string removetip = "Remove",
        string reimporttip = "Reimport")
    {
        InitializeComponent();
        PluginsView = pluginsview;
        PluginData = plugindata;
        Remove.ToolTip = removetip;
        Import.ToolTip = reimporttip;

        if (!File.Exists(plugindata.Import))
            Import.Visibility = Visibility.Hidden;
    }

    private Storyboard Storyboard { get; set; } = default!;
    private PluginsView PluginsView { get; } = default!;
    private PluginData PluginData { get; } = default!;

    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private void root_MouseEnter(object sender, MouseEventArgs e)
    {
        if (Storyboard != null)
            Storyboard.Stop();

        Storyboard = Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = Remove, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 200) }
            },
            new Interface.BaseAnim
            {
                Element = Import, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 200) }
            });
        Storyboard.Begin();

        Margin = new Thickness(5);
        Height += 10;
        Width += 10;
    }

    private void root_MouseLeave(object sender, MouseEventArgs e)
    {
        if (Storyboard != null)
            Storyboard.Stop();

        Storyboard = Interface.SetElementAnimations(
            new Interface.BaseAnim
            {
                Element = Remove, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 200) }
            },
            new Interface.BaseAnim
            {
                Element = Import, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 200) }
            });
        Storyboard.Begin();

        Margin = new Thickness(10);
        Height -= 10;
        Width -= 10;
    }

    private void Remove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (File.Exists(PluginData.Path))
            File.Delete(PluginData.Path);

        PluginsView.Refresh();
    }

    private void ReImport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsReImporting) return;

        IsReImporting = true;
        Import.IsEnabled = false;

        if (PluginData.Import is not null && File.Exists(PluginData.Import))
        {
            var fileInfo = new FileInfo(PluginData.Import);
            if (Validate.IsValid(fileInfo, out var parse))
            {
                Plugin.Import(fileInfo, parse);
            }
            else
            {
                IsReImporting = false;
                Import.IsEnabled = true;
                return;
            }
        }

        if (File.Exists(PluginData.Path)) File.Delete(PluginData.Path);

        PluginsView.Refresh();
    }
}