using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{
    public class Like : Base
    {
        public Guid LikeId { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        // Thuộc tính điều hướng
        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}
