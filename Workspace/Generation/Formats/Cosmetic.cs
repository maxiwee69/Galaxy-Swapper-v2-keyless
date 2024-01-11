namespace LilySwapper.Workspace.Generation.Formats;

public class Cosmetic
{
    public List<Downloadable> Downloadables = new();
    public string Icon;
    public string ID;
    public string Message;
    public string Name;
    public bool Nsfw = false;
    public List<Option> Options = new();
    public string OverrideFrontend;
    public JToken Parse;
    public List<Social> Socials = new();
    public int Stats = 0;
    public Generate.Type Type;
    public string UEFNTag;
    public bool UseMainUEFN = false;
}