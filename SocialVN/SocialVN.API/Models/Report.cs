namespace SocialVN.API.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalPosts { get; set; }
        public int NewFriends { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Thuộc tính điều hướng
        public User User { get; set; }
    }
}
