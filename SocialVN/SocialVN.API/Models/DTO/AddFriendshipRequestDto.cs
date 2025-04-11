using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class AddFriendshipRequestDto : BaseDto
    {
        public Guid RequesterId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Status { get; set; }

    }
}
