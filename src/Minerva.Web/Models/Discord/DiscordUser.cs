namespace Minerva.Web.Models.Discord
{
    public class DiscordUser
    {
        public string Username { get; set; }
        public string Locale { get; set; }
        public bool MFA_Enabled { get; set; }
        public string Flags { get; set; }
        public string Avatar { get; set; }
        public int Discriminator { get; set; }
        public long Id { get; set; }
    }
}
