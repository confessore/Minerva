namespace Minerva.Web.Models.Discord
{
    public class DiscordConnection
    {
        public bool Verified { get; set; }
        public string Name { get; set; }
        public bool Show_Activity { get; set; }
        public bool Friend_Sync { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public int Visibility { get; set; }
    }
}
