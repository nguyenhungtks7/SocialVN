using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Occupation { get; set; }
        public string? Location { get; set; }
        [NotMapped]
        public IFormFile? Avatar { get; set; }

        public string? AvatarPath { get; set; }
    }
}
