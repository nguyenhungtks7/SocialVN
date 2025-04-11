using SocialVN.API.Models.Domain;

namespace SocialVN.API.Models.DTO
{
    public class PostDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }

        // Thuộc tính điều hướng
        public User User { get; set; }
    }
}
