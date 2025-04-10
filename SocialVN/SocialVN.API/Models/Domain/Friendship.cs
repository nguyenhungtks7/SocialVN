using SocialVN.API.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{

    public class Friendship : Base
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public FriendshipStatus StatusEnum
        {
            get => Enum.TryParse<FriendshipStatus>(Status, out var result) ? result : FriendshipStatus.Pending;
            set => Status = value.ToString();
        }

        // Thuộc tính điều hướng
        public User Requester { get; set; }
        public User Receiver { get; set; }
    }
  
}
