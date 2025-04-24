using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class AddLikeRequestDto 
    {
        [Required(ErrorMessage = "PostId không được để trống.")]
        public Guid PostId { get; set; }

    }
}
