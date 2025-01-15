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
        public ICollection<Post> Posts{ get; set; }
        public ICollection<Friendship> Friendships { get; set; } 
        public ICollection<Report> Reports { get; set; }
    }
}
