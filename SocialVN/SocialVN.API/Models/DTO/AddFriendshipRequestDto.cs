using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class AddFriendshipRequestDto : BaseDto
    {
        [Required(ErrorMessage = "ReceiverId không được để trống.")]
        public Guid ReceiverId { get; set; }

    }
}
