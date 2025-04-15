namespace SocialVN.API.Models.Domain
{
    public class Report : Base
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public int TotalPosts { get; set; }
        public int NewFriends { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }

        // Thuộc tính điều hướng
        public ApplicationUser User { get; set; }
    }
}
