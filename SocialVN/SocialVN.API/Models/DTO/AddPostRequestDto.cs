namespace SocialVN.API.Models.DTO
{
    public class AddPostRequestDto :BaseDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
    }
}
