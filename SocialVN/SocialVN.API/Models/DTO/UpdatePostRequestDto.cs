using SocialVN.API.Models.Domain;

namespace SocialVN.API.Models.DTO
{
    public class UpdatePostRequestDto :BaseDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
    }
}
