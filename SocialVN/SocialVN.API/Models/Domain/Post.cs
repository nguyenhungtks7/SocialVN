using SocialVN.API.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        public PostStatus Status { get; set; }
        [NotMapped]
        public bool IsEdited => UpdatedAt > CreatedAt;
        // Thuộc tính điều hướng
        [JsonIgnore]
        public ApplicationUser User { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Like> Likes { get; set; }
    }
}
