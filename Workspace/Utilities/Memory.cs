using LilySwapper.Workspace.Generation;
using LilySwapper.Workspace.Usercontrols;
using LilySwapper.Workspace.Usercontrols.Overlays;
using LilySwapper.Workspace.Views;

namespace LilySwapper.Workspace.Utilities;

public static class Memory
{
    public static DashboardView DashboardView = DashboardView ?? new DashboardView();
    public static SettingsView SettingsView = SettingsView ?? new SettingsView();
    public static NotesView NotesView = NotesView ?? new NotesView();
    public static NoOptionsView NoOptionsView = NoOptionsView ?? new NoOptionsView();
    public static PluginsView PluginsView = PluginsView ?? new PluginsView();
    public static MiscView MiscView = MiscView ?? new MiscView();
    public static FovView FovView = FovView ?? new FovView();
    public static LobbyView LobbyView = LobbyView ?? new LobbyView();

    private static CosmeticsView Characters = Characters ?? new CosmeticsView(Generate.Type.Characters);

    private static CosmeticsView Backpacks = Backpacks ?? new CosmeticsView(Generate.Type.Backpacks);

    private static CosmeticsView Pickaxes = Pickaxes ?? new CosmeticsView(Generate.Type.Pickaxes);

    private static CosmeticsView Dances = Dances ?? new CosmeticsView(Generate.Type.Dances);

    private static CosmeticsView Gliders = Gliders ?? new CosmeticsView(Generate.Type.Gliders);

    private static CosmeticsView Weapons = Weapons ?? new CosmeticsView(Generate.Type.Weapons);
    public static MainView MainView { get; set; } = default!;

    public static CosmeticsView LoadCharacters(TextBox SearchBar)
    {
        Characters.SetSearchBar(SearchBar);
        return Characters;
    }

    public static CosmeticsView LoadBackpacks(TextBox SearchBar)
    {
        Backpacks.SetSearchBar(SearchBar);
        return Backpacks;
    }

    public static CosmeticsView LoadPickaxes(TextBox SearchBar)
    {
        Pickaxes.SetSearchBar(SearchBar);
        return Pickaxes;
    }

    public static CosmeticsView LoadDances(TextBox SearchBar)
    {
        Dances.SetSearchBar(SearchBar);
        return Dances;
    }

    public static CosmeticsView LoadGliders(TextBox SearchBar)
    {
        Gliders.SetSearchBar(SearchBar);
        return Gliders;
    }

    public static CosmeticsView LoadWeapons(TextBox SearchBar)
    {
        Weapons.SetSearchBar(SearchBar);
        return Weapons;
    }

    public static void Clear(bool All = true)
    {
        Characters = new CosmeticsView(Generate.Type.Characters);
        Backpacks = new CosmeticsView(Generate.Type.Backpacks);
        Pickaxes = new CosmeticsView(Generate.Type.Pickaxes);
        Dances = new CosmeticsView(Generate.Type.Dances);
        Weapons = new CosmeticsView(Generate.Type.Weapons);
        Gliders = new CosmeticsView(Generate.Type.Gliders);

        if (All)
        {
            DashboardView = new DashboardView();
            SettingsView = new SettingsView();
            NotesView = new NotesView();
            NoOptionsView = new NoOptionsView();
        }
    }
}