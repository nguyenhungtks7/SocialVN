using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class FriendshipDto : BaseDto
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Status { get; set; }

        // Thuộc tính điều hướng
        public User Requester { get; set; }
        public User Receiver { get; set; }
    }
}
