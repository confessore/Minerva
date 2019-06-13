using Discord.WebSocket;
using Minerva.Discord.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minerva.Discord.Services
{
    public class CoordinatorService
    {
        readonly DiscordSocketClient client;

        public CoordinatorService(DiscordSocketClient client)
        {
            this.client = client;
        }

        Dictionary<SocketGuildUser, (SocketGuild guild, SocketRole tier, SocketRole lane)> DuoCoordinator = new Dictionary<SocketGuildUser, (SocketGuild, SocketRole, SocketRole)>();
        Dictionary<SocketGuildUser, (bool add, SocketGuild guild, SocketRole tier, SocketRole lane)> DuoQueue = new Dictionary<SocketGuildUser, (bool, SocketGuild, SocketRole, SocketRole)>();

        Dictionary<SocketGuildUser, (SocketGuild guild, SocketRole tier, SocketRole lane)> FlexCoordinator = new Dictionary<SocketGuildUser, (SocketGuild, SocketRole, SocketRole)>();
        Dictionary<SocketGuildUser, (bool add, SocketGuild guild, SocketRole tier, SocketRole lane)> FlexQueue = new Dictionary<SocketGuildUser, (bool, SocketGuild, SocketRole, SocketRole)>();

        public async Task StartCoordinatorTasks()
        {
            await StartDuoCoordinatorTask();
            await StartFlexCoordinatorTask();
        }

        Task StartDuoCoordinatorTask()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateDuoCoordinator(DuoQueue);
                    foreach (var guild in DuoCoordinator.GroupBy(x => x.Value.guild))
                    {
                        foreach (var tier in guild.GroupBy(x => x.Value.tier))
                        {
                            if (tier.Count() > 1)
                            {
                                var duo = new List<KeyValuePair<SocketGuildUser, (SocketGuild guild, SocketRole tier, SocketRole lane)>>();
                                duo.AddRange(tier.Where(x => !duo.Contains(x) && !duo.Any(y => y.Value.lane == x.Value.lane)));
                                if (duo.Count() > 1)
                                {
                                    foreach (var user in duo)
                                    {
                                        await AddToDuoQueue(user.Key, false);
                                        var dm = await client.GetUser(user.Key.Id).GetOrCreateDMChannelAsync();
                                        await dm.SendMessageAsync($"your **{user.Value.guild.Name.ToUpper()}** match is ready! here are the names of the players, their tiers and their lanes:");
                                        foreach (var u in duo)
                                            await dm.SendMessageAsync($"**{u.Key.Nickname ?? u.Key.Username}**    **{u.Value.tier.Name.ToUpper()}**    **{u.Value.lane.Name.ToUpper()}**");
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return Task.CompletedTask;
        }

        Task StartFlexCoordinatorTask()
        {
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await UpdateFlexCoordinator(DuoQueue);
                    foreach (var guild in DuoCoordinator.GroupBy(x => x.Value.guild))
                    {
                        foreach (var tier in guild.GroupBy(x => x.Value.tier))
                        {
                            if (tier.Count() > 4)
                            {
                                var flex = new List<KeyValuePair<SocketGuildUser, (SocketGuild guild, SocketRole tier, SocketRole lane)>>();
                                flex.AddRange(tier.Where(x => !flex.Contains(x) && !flex.Any(y => y.Value.lane == x.Value.lane)));
                                if (flex.Count() > 4)
                                {
                                    foreach (var user in flex)
                                    {
                                        await AddToFlexQueue(user.Key, false);
                                        var dm = await client.GetUser(user.Key.Id).GetOrCreateDMChannelAsync();
                                        await dm.SendMessageAsync($"your **{user.Value.guild.Name.ToUpper()}** match is ready! here are the names of the players, their tiers and their lanes:");
                                        foreach (var u in flex)
                                            await dm.SendMessageAsync($"**{u.Key.Nickname ?? u.Key.Username}**    **{u.Value.tier.Name.ToUpper()}**    **{u.Value.lane.Name.ToUpper()}**");
                                    }
                                }
                            }
                        }
                    }
                }
            });
            return Task.CompletedTask;
        }

        public int DuoCount => DuoCoordinator.Count;

        public int FlexCount => FlexCoordinator.Count;

        public Task UpdateDuoCoordinator(Dictionary<SocketGuildUser, (bool add, SocketGuild guild, SocketRole tier, SocketRole lane)> queue)
        {
            foreach (var pair in queue.ToArray())
            {
                if (pair.Value.add)
                {
                    if (!DuoCoordinator.Keys.Contains(pair.Key))
                        DuoCoordinator.Add(pair.Key, (pair.Value.guild, pair.Value.tier, pair.Value.lane));
                }
                else if (DuoCoordinator.Keys.Contains(pair.Key))
                    DuoCoordinator.Remove(pair.Key);
                RemoveFromDuoQueue(pair.Key);
            }
            return Task.CompletedTask;
        }

        public Task UpdateFlexCoordinator(Dictionary<SocketGuildUser, (bool add, SocketGuild guild, SocketRole tier, SocketRole lane)> queue)
        {
            foreach (var pair in queue.ToArray())
            {
                if (pair.Value.add)
                {
                    if (!FlexCoordinator.Keys.Contains(pair.Key))
                        FlexCoordinator.Add(pair.Key, (pair.Value.guild, pair.Value.tier, pair.Value.lane));
                }
                else if (FlexCoordinator.Keys.Contains(pair.Key))
                    FlexCoordinator.Remove(pair.Key);
                RemoveFromFlexQueue(pair.Key);
            }
            return Task.CompletedTask;
        }

        public Task AddToDuoQueue(SocketGuildUser user, bool add)
        {
            if (!DuoQueue.Keys.Contains(user))
                DuoQueue.Add(user, (add, user.Guild, GetTier(user), GetLane(user)));
            return Task.CompletedTask;
        }

        public Task AddToFlexQueue(SocketGuildUser user, bool add)
        {
            if (!FlexQueue.Keys.Contains(user))
                FlexQueue.Add(user, (add, user.Guild, GetTier(user), GetLane(user)));
            return Task.CompletedTask;
        }

        Task RemoveFromDuoQueue(SocketGuildUser user)
        {
            if (DuoQueue.Keys.Contains(user))
                DuoQueue.Remove(user);
            return Task.CompletedTask;
        }

        Task RemoveFromFlexQueue(SocketGuildUser user)
        {
            if (DuoQueue.Keys.Contains(user))
                DuoQueue.Remove(user);
            return Task.CompletedTask;
        }

        public bool HasTier(SocketGuildUser user) =>
            user.Roles.Any(x => Enum.GetNames(typeof(DiscordTierRole)).Contains(x.Name.ToLower()));

        public SocketRole GetTier(SocketGuildUser user) =>
            user.Roles.Where(x => Enum.GetNames(typeof(DiscordTierRole)).Contains(x.Name.ToLower())).FirstOrDefault();

        public bool HasLane(SocketGuildUser user) =>
            user.Roles.Any(x => Enum.GetNames(typeof(DiscordLaneRole)).Contains(x.Name.ToLower()));

        public SocketRole GetLane(SocketGuildUser user) =>
            user.Roles.Where(x => Enum.GetNames(typeof(DiscordLaneRole)).Contains(x.Name.ToLower())).FirstOrDefault();
    }
}
