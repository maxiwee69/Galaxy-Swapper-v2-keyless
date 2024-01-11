namespace LilySwapper.Workspace.Utilities;

public static class Languages
{
    public enum Type
    {
        View,
        Header,
        Message
    }

    private static JToken Parse { get; set; } = default!;

    private static JToken GetPath(Type type)
    {
        return type switch
        {
            Type.View => Parse["Views"],
            Type.Header => Parse["Headers"],
            Type.Message => Parse["Messages"],
            _ => throw new Exception($"Failed to find {type}")
        };
    }

    public static string Read(Type Type, params string[] Keys)
    {
        Parse ??= Endpoint.Read(Endpoint.Type.Languages);
        var Path = Keys.Aggregate(GetPath(Type), (current, key) => current[key]);
        var selected = Settings.Read(Settings.Type.Language).Value<string>();
        return Path[selected].KeyIsNullOrEmpty() ? Path["EN"].Value<string>() : Path[selected].Value<string>();
    }

    public static string[] ReadSolutions(Type type, params string[] keys)
    {
        Parse ??= Endpoint.Read(Endpoint.Type.Languages);
        var jToken = keys.Aggregate(GetPath(type), (current, key) => current[key])["Solutions"];
        var language = Settings.Read(Settings.Type.Language).Value<string>();
        return jToken[language].KeyIsNullOrEmpty()
            ? ((JArray)jToken["EN"]).ToObject<string[]>()
            : ((JArray)jToken[language]).ToObject<string[]>();
    }
}