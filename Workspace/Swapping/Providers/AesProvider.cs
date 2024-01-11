using LilySwapper.Workspace.CProvider.Encryption;
using Newtonsoft.Json;
using RestSharp;

namespace LilySwapper.Workspace.Swapping.Providers;

public static class AesProvider
{
    private const string Domain = "https://galaxyswapperv2.com/API/Fortnite/Aes.json";
    public static Dictionary<FGuid, FAesKey> Keys = new();

    public static void Initialize()
    {
        if (Keys.Count != 0)
        {
            Log.Warning("Keys were already initialized");
            return;
        }

        using (RestClient client = new())
        {
            var request = new RestRequest(new Uri(Domain));

            Log.Information($"Sending {request.Method} request to {Domain}");

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error(
                    $"Failed to request aes keys from: {Domain} Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                return;
            }

            var parse = JsonConvert.DeserializeObject<JObject>(response.Content);

            if (parse["dynamicKeys"] is not null)
                foreach (var dynamickey in parse["dynamicKeys"])
                {
                    var pakGuid = new FGuid(dynamickey["guid"].Value<string>());
                    if (!dynamickey["key"].KeyIsNullOrEmpty() && !Keys.ContainsKey(pakGuid))
                    {
                        var key = Format(dynamickey["key"].Value<string>());
                        Keys.Add(pakGuid, new FAesKey(key));

                        Log.Information($"Added {pakGuid} to keys array with value {key} as dynamic key");
                    }
                }
            else Log.Warning("dynamicKeys array is null! No dynamic keys will be loaded");
        }
    }

    private static string Format(string key)
    {
        return !key.StartsWith("0x") ? "0x" + key : key;
    }
}