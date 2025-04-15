namespace SocialVN.API.Models.Domain
{
    public class Post : Base
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }

        // Thuộc tính điều hướng
        public ApplicationUser User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
