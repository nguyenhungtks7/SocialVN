using SocialVN.API.Models.Domain;

namespace SocialVN.API.Models.DTO
{
    public class AddCommentRequestDto
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }

    }
}
