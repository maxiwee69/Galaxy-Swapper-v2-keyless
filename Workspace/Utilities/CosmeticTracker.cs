﻿using RestSharp;

namespace LilySwapper.Workspace.Utilities;

public static class CosmeticTracker
{
    private const string Domain = "https://galaxyswapperv2.com/API/CosmeticStats/Upload.php";

    public static void Update(string name, string id, string type)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        if (Presence.User is null) return;

        using (var client = new RestClient())
        {
            var request = new RestRequest(new Uri(Domain));

            request.AddHeader("name", name);
            request.AddHeader("id", id);
            request.AddHeader("type", type);
            request.AddHeader("username", Presence.User.Username);
            request.AddHeader("discordid", Presence.User.ID);
            request.AddHeader("type", type);

            Log.Information($"Sending request to {Domain}");

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                Log.Error(
                    $"Failed to download response from endpoint! Expected: {HttpStatusCode.OK} Received: {response.StatusCode}");
                return;
            }

            Log.Information(
                $"Finished {request.Method} request in {stopwatch.GetElaspedAndStop().ToString("mm':'ss")} received {response.Content.Length}");
        }
    }
}