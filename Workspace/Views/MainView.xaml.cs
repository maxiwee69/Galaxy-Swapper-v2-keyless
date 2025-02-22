﻿using LilySwapper.Workspace.Components;
using LilySwapper.Workspace.Usercontrols;

namespace LilySwapper.Workspace.Views;

/// <summary>
///     Interaction logic for MainView.xaml
/// </summary>
public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
        Memory.MainView = this;
    }

    private CTabControl LastTabBorder { get; set; } = default!;

    private UserControl LastOverlay { get; set; } = default!;

    private void MainView_Loaded(object sender, RoutedEventArgs e)
    {
        Main.Visibility = Visibility.Hidden;
        SetOverlay(new SplashView());
    }

    public void Tab_Click(object sender, MouseButtonEventArgs e)
    {
        var Sender = (CTabControl)sender;

        LastTabBorder?.Tab_Default();
        LastTabBorder = Sender;
        Sender.Tab_Clicked();

        var tabActions = new Dictionary<string, Func<UserControl>>
        {
            { "Dashboard", () => Memory.DashboardView },
            { "Characters", () => Memory.LoadCharacters(SearchBar.Searchbar) },
            { "Backpacks", () => Memory.LoadBackpacks(SearchBar.Searchbar) },
            { "Pickaxes", () => Memory.LoadPickaxes(SearchBar.Searchbar) },
            { "Dances", () => Memory.LoadDances(SearchBar.Searchbar) },
            { "Gliders", () => Memory.LoadGliders(SearchBar.Searchbar) },
            { "Weapons", () => Memory.LoadWeapons(SearchBar.Searchbar) },
            { "Misc", () => Memory.MiscView },
            { "Settings", () => Memory.SettingsView },
            { "Plugins", () => Memory.PluginsView }
        };

        if (!tabActions.TryGetValue(Sender.Name, out var action))
            return;

        var newTab = action();
        var isSearchBarVisible = Sender.Name != "Dashboard" && Sender.Name != "Misc" && Sender.Name != "Settings" &&
                                 Sender.Name != "Plugins";

        SearchBar.Visibility = isSearchBarVisible ? Visibility.Visible : Visibility.Hidden;

        if (TabHolder.Child != null)
            TabHolder.Child = null;

        TabHolder.Child = newTab;

        Interface.SetElementAnimations(new Interface.BaseAnim
        {
            Element = newTab, Property = new PropertyPath(OpacityProperty),
            ElementAnim = new DoubleAnimation { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) }
        }).Begin();
    }

    public void SetOverlay(UserControl Overlay)
    {
        if (LastOverlay != null)
            Base.Children.Remove(LastOverlay);

        LastOverlay = Overlay;
        Main.IsEnabled = false;

        var Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim
        {
            Element = TabHolder, Property = new PropertyPath(OpacityProperty),
            ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 400) }
        });
        Storyboard.Completed += delegate
        {
            Interface.SetBlur(Main);
            Base.Children.Add(Overlay);
            Interface.SetElementAnimations(new Interface.BaseAnim
            {
                Element = Overlay, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) }
            }).Begin();
        };
        Storyboard.Begin();
    }

    public void RemoveOverlay()
    {
        if (LastOverlay is null)
            return;

        Main.Effect = null;

        var Storyboard = Interface.SetElementAnimations(new Interface.BaseAnim
        {
            Element = LastOverlay, Property = new PropertyPath(OpacityProperty),
            ElementAnim = new DoubleAnimation { From = 1, To = 0, Duration = new TimeSpan(0, 0, 0, 0, 400) }
        });
        Storyboard.Completed += delegate
        {
            Main.IsEnabled = true;
            Base.Children.Remove(LastOverlay);
            LastOverlay = null!;
            Interface.SetElementAnimations(new Interface.BaseAnim
            {
                Element = TabHolder, Property = new PropertyPath(OpacityProperty),
                ElementAnim = new DoubleAnimation { From = 0, To = 1, Duration = new TimeSpan(0, 0, 0, 0, 400) }
            }).Begin();
        };
        Storyboard.Begin();
    }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        Close();
    }

    private void Minimize_Click(object sender, MouseButtonEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void Drag_Click(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}