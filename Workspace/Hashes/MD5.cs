namespace LilySwapper.Workspace.Hashes;

public static class MD5
{
    public static byte[] Hash(byte[] buffer)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            return md5.ComputeHash(buffer);
        }
    }

    public static byte[] Hash(string content)
    {
        return Hash(Encoding.ASCII.GetBytes(content));
    }
}