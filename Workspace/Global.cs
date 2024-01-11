namespace LilySwapper.Workspace;

public static class Global
{
    public const string Version = "1.31";
    public const string ApiVersion = "1.23";
    public static string Discord = "https://pornhub.com";
    public static string Website = "https://maxiwee.de";
    public static string Download = "https://github.com/maxiwee69/Galaxy-Swapper-v2-keyless";
    public static string Key = "´https://pornhub.com";

    public class CustomException : Exception
    {
        public CustomException(string message) : base(message)
        {
        }

        public CustomException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class FortniteDirectoryEmptyException : Exception
    {
        public FortniteDirectoryEmptyException(string message) : base(message)
        {
        }

        public FortniteDirectoryEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}