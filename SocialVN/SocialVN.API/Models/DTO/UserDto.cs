using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class UserDto 
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public string? LivingPlace { get; set; } // Thêm nơi sống
        public string? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string AvatarPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }


}

