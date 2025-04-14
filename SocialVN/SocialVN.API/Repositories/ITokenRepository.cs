using Microsoft.AspNetCore.Identity;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser userm, List<string> roles);
    }
}
