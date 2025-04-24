using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{
    public class Like : Base
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; }

        // Thuộc tính điều hướng
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("PostId")]
        public Post Post { get; set; }
    }
}
