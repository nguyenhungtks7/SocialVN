namespace SocialVN.API.Models.Domain
{
    public class Comment : Base
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }

        // Thuộc tính điều hướng
        public Post Post { get; set; }
        public ApplicationUser User { get; set; }
    }
}
