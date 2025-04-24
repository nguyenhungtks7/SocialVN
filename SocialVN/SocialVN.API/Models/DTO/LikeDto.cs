namespace SocialVN.API.Models.DTO
{
    public class LikeDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
    }
    
}
