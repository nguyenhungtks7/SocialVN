using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class FriendshipDto 
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid RequesterId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        [RegularExpression("^(Pending|Accepted|Rejected)$", ErrorMessage = "Trạng thái không hợp lệ.")]
        public string Status { get; set; }

        // Thuộc tính điều hướng


        public ApplicationUser Requester { get; set; }
        public ApplicationUser Receiver { get; set; }
    }
}
