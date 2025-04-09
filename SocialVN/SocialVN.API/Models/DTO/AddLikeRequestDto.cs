namespace SocialVN.API.Models.DTO
{
    public class AddLikeRequestDto
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
}
