namespace SocialVN.API.Models
{
    public class Like
    {
        public Guid LikeId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Thuộc tính điều hướng
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
