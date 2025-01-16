namespace SocialVN.API.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public string Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Thuộc tính điều hướng
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Friendship> FriendshipsAsRequester { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsAsReceiver { get; set; } = new List<Friendship>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
