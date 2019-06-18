using Microsoft.AspNetCore.Identity;
using Minerva.Web.Models.Discord;
using Minerva.Web.Models.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minerva.Web.Services.Interfaces
{
    public interface IDiscordService
    {
        Task<IEnumerable<DiscordConnection>> GetUserConnectionsAsync(UserManager<ApplicationUser> userManager, ApplicationUser user);
        Task<DiscordUser> GetUserProfileAsync(UserManager<ApplicationUser> userManager, ApplicationUser user);
    }
}
