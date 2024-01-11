using LilySwapper.Workspace.Usercontrols.Overlays;
using Newtonsoft.Json;
using RestSharp;

namespace LilySwapper.Workspace.Verify.EpicGames;

public class LiveManifest
{
    private const string Domain =
        "https://launcher-public-service-prod06.ol.epicgames.com/launcher/api/public/assets/v2/platform/Windows/namespace/fn/catalogItem/4fe75bbc5a674f4f9b356b5c90567da5/app/Fortnite/label/Live";

    public JObject Parse = null!;

    public bool Download(VerifyView verifyView, string access_token)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using (var client = new RestClient())
        {
            var request = new RestRequest(new Uri(Domain));
            request.AddHeader("Authorization", "Bearer " + access_token);

            verifyView.Output($"Sending {request.Method} request to {Domain}", VerifyView.Type.Info);

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK) return false;

            Parse = JsonConvert.DeserializeObject<JObject>(response.Content);
            verifyView.Output(
                $"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}",
                VerifyView.Type.Info);

            return true;
        }
    }
}