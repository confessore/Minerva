using Minerva.Discord.Enums;
using Minerva.Discord.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Minerva.Discord.Services
{
    public class RiotService
    {
        HttpClient HttpClient;
        readonly string Key;

        public RiotService()
        {
            HttpClient = new HttpClient();
            Key = Environment.GetEnvironmentVariable("Riot");
        }

        readonly string protocol = "https://";
        readonly string address = ".api.riotgames.com/";
        readonly string summonersByName = "lol/summoner/v4/summoners/by-name/";
        readonly string leaguesBySummoner = "lol/league/v4/entries/by-summoner/";
        readonly string api = "?api_key=";

        async Task<Summoner> GetSummonerByNameAsync(RiotRegion region, string name)
        {
            var response = await HttpClient.GetAsync(protocol + region + address + summonersByName + name + api + Key);
            return JsonConvert.DeserializeObject<Summoner>(await response.Content.ReadAsStringAsync());
        }

        async Task<League[]> GetLeaguesBySummonerAsync(RiotRegion region, Summoner summoner)
        {
            var response = await HttpClient.GetAsync(protocol + region + address + leaguesBySummoner + summoner.Id + api + Key);
            return JsonConvert.DeserializeObject<League[]>(await response.Content.ReadAsStringAsync());
        }

        public async Task<League[]> GetLeaguesByNameAsync(RiotRegion region, string name) =>
            await GetLeaguesBySummonerAsync(region, await GetSummonerByNameAsync(region, name));

        public string GetBestSoloRank(League[] leagues) =>
            leagues.Any() ? leagues.Where(x => x.QueueType.ToLower() == "ranked_solo_5x5").FirstOrDefault().Tier ?? string.Empty : string.Empty;

        public RiotRegion Region(string guildName)
        {
            if (guildName.Contains("EUW")) return RiotRegion.euw1;
            else return RiotRegion.na1;
        }
    }
}
