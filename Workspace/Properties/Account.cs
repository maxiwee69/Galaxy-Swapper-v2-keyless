using LilySwapper.Workspace.Hashes;
using Newtonsoft.Json;
using RestSharp;

namespace LilySwapper.Workspace.Properties;

public static class Account
{
    private const string Domain = "https://galaxyswapperv2.com/Key/Valid.php";
    public static readonly string Path = $"{App.Config}\\Account.dat";

    public static bool Valid()
    {
        return true;
    }

    public static bool Create(int days)
    {
        try
        {
            if (File.Exists(Path))
                File.Delete(Path);

            var writer = new Writer(new byte[60000]);
            var username = Encoding.ASCII.GetBytes(Environment.UserName);

            writer.Write(CityHash.Hash(username));
            writer.Write(username.Length);
            writer.WriteBytes(username);

            writer.Write(days);

            var dateTime = DateTime.Now;
            for (var i = 0; i < days; i++)
            {
                var date = Encoding.ASCII.GetBytes(dateTime.AddDays(i).ToString("dd/MM/yyyy"));
                writer.Write(CityHash.Hash(date));
                writer.Write(date.Length);
                writer.WriteBytes(date);
            }

            File.WriteAllBytes(Path, writer.ToByteArray(writer.Position));
            Log.Information($"Wrote to: {Path} with {days} days");
            return true;
        }
        catch (Exception e)
        {
            Log.Fatal("Application caught a unexpected error while creating account data");
            Log.Fatal(e.ToString());
            return false;
        }
    }

    public static bool Activate(string Activation)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using (var client = new RestClient())
        {
            var request = new RestRequest(new Uri(Domain));
            request.AddHeader("version", Global.Version);
            request.AddHeader("apiversion", Global.Version);
            request.AddHeader("activation", Activation);
            request.AddHeader("auth", "galaxyswapperv2");

            Log.Information($"Sending {request.Method} request to {Domain}");
            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Fatal(
                    $"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                Message.DisplaySTA("Error", "Webclient caught a exception while downloading response from Endpoint.",
                    solutions: new[]
                        { "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN" },
                    exit: true);
            }

            Log.Information(
                $"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");

            var parse = JsonConvert.DeserializeObject<JObject>(response.Content);
            switch (parse["status"].Value<int>())
            {
                case 200:
                    if (!Create(parse["days"].Value<int>()))
                        return false;

                    Message.Display(Languages.Read(Languages.Type.Header, "Info"),
                        string.Format(Languages.Read(Languages.Type.Message, "LoginSuccess"),
                            parse["days"].Value<int>()));
                    return true;
                case 409:
                    Message.Display(Languages.Read(Languages.Type.Header, "Warning"),
                        Languages.Read(Languages.Type.Message, "LoginInvalid"));
                    return false;
                default:
                    Message.Display(Languages.Read(Languages.Type.Header, "Error"), parse["message"].Value<string>(),
                        solutions: new[]
                        {
                            "Disable Windows Defender Firewall", "Disable any anti-virus softwares", "Turn on a VPN"
                        });
                    return false;
            }
        }
    }
}