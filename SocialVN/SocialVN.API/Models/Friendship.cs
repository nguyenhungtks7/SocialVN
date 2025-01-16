namespace SocialVN.API.Models
{
    public class Friendship
    {
        public Guid Id { get; set; }
        public Guid RequesterId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Thuộc tính điều hướng
        public User Requester { get; set; }
        public User Receiver { get; set; }
    }
}
