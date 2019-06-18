using Microsoft.AspNetCore.Identity;
using Minerva.Web.Models.Discord;
using System.Collections.Generic;

namespace Minerva.Web.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public DiscordUser DiscordUser { get; set; }
        public IEnumerable<DiscordConnection> DiscordConnections { get; set; }
    }
}
