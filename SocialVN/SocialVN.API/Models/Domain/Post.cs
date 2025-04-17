using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{
    public class Post : Base
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public bool IsEdited => UpdatedAt > CreatedAt;
        // Thuộc tính điều hướng
        public ApplicationUser User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
