using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minerva.Discord.Services
{
    public class ChannelService
    {
        readonly DiscordSocketClient client;

        public ChannelService(DiscordSocketClient client)
        {
            this.client = client;
        }

        public async Task GenerateChannelsAsync()
        {
            await GenerateTextChannelsAsync();
            await GenerateVoiceChannelsAsync();
        }

        async Task GenerateTextChannelsAsync()
        {
            await RemoveCategoryChannelsAsync();
            foreach (var guild in client.Guilds)
            {
                foreach (var pair in TextChannels)
                {
                    if (!TextChannelExists(guild, pair.Key))
                    {
                        var tmp = await guild.CreateTextChannelAsync(pair.Key);
                        foreach (var role in guild.Roles)
                        {
                            if (role.IsEveryone)
                                await tmp.AddPermissionOverwriteAsync(role, pair.Value.everyone);
                            else
                                await tmp.AddPermissionOverwriteAsync(role, pair.Value.member);
                        }
                    }
                }
            }
        }

        async Task GenerateVoiceChannelsAsync()
        {
            await RemoveCategoryChannelsAsync();
            foreach (var guild in client.Guilds)
            {
                foreach (var pair in VoiceChannels)
                {
                    if (!VoiceChannelExists(guild, pair.Key))
                    {
                        var tmp = await guild.CreateVoiceChannelAsync(pair.Key);
                        foreach (var role in guild.Roles)
                        {
                            if (role.IsEveryone)
                                await tmp.AddPermissionOverwriteAsync(role, pair.Value.everyone);
                            else
                                await tmp.AddPermissionOverwriteAsync(role, pair.Value.member);
                        }
                    }
                }
            }
        }

        async Task RemoveCategoryChannelsAsync()
        {
            foreach (var guild in client.Guilds)
                foreach (var cc in guild.CategoryChannels)
                    await cc.DeleteAsync();
        }

        bool TextChannelExists(SocketGuild guild, string channel) =>
            guild.TextChannels.Any(x => x.Name.ToLower() == channel.ToLower());

        bool VoiceChannelExists(SocketGuild guild, string channel) =>
            guild.VoiceChannels.Any(x => x.Name.ToLower() == channel.ToLower());

        public Task GetValues()
        {
            foreach (var guild in client.Guilds)
                foreach (var channel in guild.TextChannels)
                    foreach (var ow in channel.PermissionOverwrites)
                        Console.WriteLine($"{guild}    {channel}    {ow}    {ow.Permissions}    {ow.Permissions.AllowValue}    {ow.Permissions.DenyValue}");
            return Task.CompletedTask;
        }

        Dictionary<string, (OverwritePermissions everyone, OverwritePermissions member)> TextChannels =>
            new Dictionary<string, (OverwritePermissions, OverwritePermissions)>()
            {
                { "welcome", (new OverwritePermissions(66560, 2048), new OverwritePermissions(0, 1024)) },
                { "guides", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "lfg", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "general", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "offtopic", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "help", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 2048)) }
            };

        Dictionary<string, (OverwritePermissions everyone, OverwritePermissions member)> VoiceChannels =>
            new Dictionary<string, (OverwritePermissions, OverwritePermissions)>()
            {
                { "General", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "DuoQ-1", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "DuoQ-2", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "DuoQ-3", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "FlexQ-1", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "FlexQ-2", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) },
                { "Music", (new OverwritePermissions(0, 1024), new OverwritePermissions(1024, 0)) }
            };
    }
}
