﻿using DiscordRPC;
using Button = DiscordRPC.Button;

namespace LilySwapper.Workspace.Utilities;

public static class Presence
{
    private static RichPresence RichPresence;
    public static DiscordRpcClient Client;
    public static User User;

    public static void Initialize()
    {
        var Parse = Endpoint.Read(Endpoint.Type.Presence);

        Client = new DiscordRpcClient(Parse["ApplicationID"].Value<string>());
        Client.Initialize();
        Client.OnReady += (sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.User.Username) &&
                !string.IsNullOrEmpty(e.User.GetAvatarURL(User.AvatarFormat.PNG))) User = e.User;
        };

        RichPresence = new RichPresence
        {
            Details = "Dashboard",
            State = Parse["State"].Value<string>(),
            Timestamps = Timestamps.Now,
            Buttons = Parse["Buttons"].Select(Button => new Button
                { Label = Button["Label"].Value<string>(), Url = Button["URL"].Value<string>() }).ToArray(),
            Assets = new Assets
            {
                LargeImageKey = Parse["LargeImageKey"].Value<string>(),
                LargeImageText = Parse["LargeImageText"].Value<string>(),
                SmallImageKey = Parse["SmallImageKey"].Value<string>(),
                SmallImageText = Parse["SmallImageText"].Value<string>()
            }
        };

        if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
            Client.SetPresence(RichPresence);
    }

    public static void Update(string Details)
    {
        if (Settings.Read(Settings.Type.RichPresence).Value<bool>())
            Client.UpdateDetails(Details);
    }
}