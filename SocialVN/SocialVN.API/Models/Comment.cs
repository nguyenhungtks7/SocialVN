namespace SocialVN.API.Models
{
    public class Comment: Base
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
