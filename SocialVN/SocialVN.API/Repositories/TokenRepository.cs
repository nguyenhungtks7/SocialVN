using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SocialVN.API.Models.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialVN.API.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private IConfiguration configuration { get; }

        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public string CreateJWTToken(ApplicationUser user, List<string> roles)
        {
            // Create claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email), // Claim Email
                new Claim(ClaimTypes.NameIdentifier, user.Id) // Thêm Claim NameIdentifier với user.Id
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
             );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
