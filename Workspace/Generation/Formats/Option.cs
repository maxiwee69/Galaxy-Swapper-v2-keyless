namespace LilySwapper.Workspace.Generation.Formats;

public class Option : ICloneable
{
    public Cosmetic Cosmetic;
    public List<Downloadable> Downloadables = new();
    public List<Asset> Exports = new();
    public string Icon;
    public string ID;
    public bool IsPlugin;
    public string Message;
    public string Name;
    public bool Nsfw;
    public string OptionMessage;
    public string OverrideIcon;
    public JToken Parse;
    public List<Social> Socials = new();
    public bool UEFNFormat;
    public string UEFNTag;
    public bool UseMainUEFN;

    public object Clone()
    {
        return new Option { Name = Name, ID = ID, Icon = Icon, OptionMessage = OptionMessage, Parse = Parse };
    }

    public object Clone(bool all)
    {
        return new Option
        {
            Name = Name, ID = ID, Icon = Icon, OverrideIcon = OverrideIcon, Message = Message,
            OptionMessage = OptionMessage, Nsfw = Nsfw, UseMainUEFN = UseMainUEFN, UEFNFormat = UEFNFormat,
            IsPlugin = IsPlugin, UEFNTag = UEFNTag, Cosmetic = Cosmetic, Parse = Parse, Downloadables = Downloadables,
            Socials = Socials, Exports = Exports
        };
    }
}