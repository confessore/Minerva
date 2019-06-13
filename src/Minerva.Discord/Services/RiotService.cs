using Minerva.Discord.Enums;
using Minerva.Discord.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Minerva.Discord.Services
{
    public class RiotService
    {
        readonly string key;

        public RiotService()
        {
            key = Environment.GetEnvironmentVariable("Riot");
        }

        readonly string protocol = "https://";
        readonly string address = ".api.riotgames.com/";
        readonly string summonersByName = "lol/summoner/v4/summoners/by-name/";
        readonly string leaguesBySummoner = "lol/league/v4/entries/by-summoner/";
        readonly string api = "?api_key=";

        async Task<Summoner> GetSummonerByNameAsync(RiotRegion region, string name) =>
            JsonConvert.DeserializeObject<Summoner>(await GetUrlAsync(protocol + region + address + summonersByName + name + api + key).ConfigureAwait(false));

        async Task<League[]> GetLeaguesBySummonerAsync(RiotRegion region, Summoner summoner) =>
            JsonConvert.DeserializeObject<League[]>(await GetUrlAsync(protocol + region + address + leaguesBySummoner + summoner.Id + api + key).ConfigureAwait(false));

        public async Task<League[]> GetLeaguesByNameAsync(RiotRegion region, string name) =>
            await GetLeaguesBySummonerAsync(region, await GetSummonerByNameAsync(region, name));

        public string GetBestSoloRank(League[] leagues) =>
            leagues.Any() ? leagues.Where(x => x.QueueType.ToLower() == "ranked_solo_5x5").FirstOrDefault().Tier ?? string.Empty : string.Empty;

        public RiotRegion Region(string guildName)
        {
            if (guildName.Contains("EUW")) return RiotRegion.euw1;
            else return RiotRegion.na1;
        }

        async Task<string> GetUrlAsync(string url)
        {
            var request = WebRequest.Create(url);
            using (var response = await request.GetResponseAsync())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return await reader.ReadToEndAsync();
        }
    }
}
