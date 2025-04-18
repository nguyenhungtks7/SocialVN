using SocialVN.API.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SocialVN.API.Models.Domain
{

    public class Friendship : Base
    {
        public Guid Id { get; set; }
        public string RequesterId { get; set; }
        public string ReceiverId { get; set; }
        public string Status { get; set; }
        [NotMapped]
        public FriendshipStatus StatusEnum
        {
            get => Enum.TryParse<FriendshipStatus>(Status, out var result) ? result : FriendshipStatus.Pending;
            set => Status = value.ToString();
        }

        // Thuộc tính điều hướng
        [JsonIgnore]
        public ApplicationUser Requester { get; set; }
        [JsonIgnore]
        public ApplicationUser Receiver { get; set; }
    }
  
}
