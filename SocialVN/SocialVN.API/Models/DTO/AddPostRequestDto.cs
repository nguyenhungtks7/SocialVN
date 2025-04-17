namespace SocialVN.API.Models.DTO
{
    public class AddPostRequestDto
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
