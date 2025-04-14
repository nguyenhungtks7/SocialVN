using Microsoft.AspNetCore.Identity;

namespace SocialVN.API.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Occupation { get; set; }
        public string? Location { get; set; }
        public string? Avatar { get; set; }
    }
}
