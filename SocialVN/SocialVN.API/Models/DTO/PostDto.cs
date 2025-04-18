using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class PostDto : BaseDto
    {
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public PostStatus Status { get; set; }
        public bool IsEdited { get; set; }

        // Thuộc tính điều hướng
        public UserDto? User { get; set; }
    }

}
