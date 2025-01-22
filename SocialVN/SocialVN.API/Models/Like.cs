namespace SocialVN.API.Models
{
    public class Like : Base
    {
        public Guid LikeId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
       
        // Thuộc tính điều hướng
        public Post Post { get; set; }
        public User User { get; set; }
    }
}
