﻿namespace LilySwapper.Workspace.Usercontrols.Overlays;

/// <summary>
///     Interaction logic for LanguageSelectionView.xaml
/// </summary>
public partial class LanguageSelectionView : UserControl
{
    public LanguageSelectionView(SettingsView settingsview)
    {
        InitializeComponent();
        SettingsView = settingsview;
    }

    private SettingsView SettingsView { get; }

    private void Close_Click(object sender, MouseButtonEventArgs e)
    {
        Memory.MainView.RemoveOverlay();
    }

    private void LanguageSelectionView_Loaded(object sender, RoutedEventArgs e)
    {
        var Parse = Endpoint.Read(Endpoint.Type.Languages);
        foreach (var Option in Parse["Languages"])
        {
            var NewOption = new Button
            {
                Content = Option["Name"].Value<string>(), Style = (Style)FindResource("LanguageSelection"), Width = 530,
                Height = 30, Margin = new Thickness(5)
            };
            NewOption.Click += (sender, e) =>
            {
                Settings.Edit(Settings.Type.Language, Option["CodeName"].Value<string>());
                SettingsView.SettingsView_Loaded(SettingsView, null!);
                Memory.MainView.RemoveOverlay();
            };
            Language_Items.Children.Add(NewOption);
        }
    }
}