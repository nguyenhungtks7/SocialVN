using SocialVN.API.Models.Domain;

namespace SocialVN.API.Models.DTO
{
    public class CommentDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }

        // Thuộc tính điều hướng
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
