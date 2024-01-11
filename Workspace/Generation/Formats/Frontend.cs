namespace LilySwapper.Workspace.Generation.Formats;

public class Frontend
{
    public Dictionary<string, Cosmetic> Cosmetics = new();
    public List<Option> Options = new();
    public JToken Empty { get; set; }
}