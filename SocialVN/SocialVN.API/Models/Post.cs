namespace SocialVN.API.Models
{
    public class Post:Base
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } 
        public string Image { get; set; }
       
        // Thuộc tính điều hướng
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
