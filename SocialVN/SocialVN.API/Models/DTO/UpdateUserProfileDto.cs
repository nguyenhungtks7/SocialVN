using SocialVN.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class UpdateUserProfileDto
    {
        public string? FullName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string? Occupation { get; set; }

        public string? Location { get; set; }
        public string?  LivingPlace { get; set; } // Thêm nơi sống
        public GenderEnum? Gender { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; } // Thêm số điện thoại
        // Avatar là file ảnh tải lên, không cần validate ở đây mà sẽ xử lý ở controller
        public IFormFile? Avatar { get; set; }

    }
}
