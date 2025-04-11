using SocialVN.API.Models.Domain;

namespace SocialVN.API.Models.DTO
{
    public class AddCommentRequestDto :BaseDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }

    }
}
