namespace SocialVN.API.Models.Domain
{
    public class ApplicationUser
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public string AvatarUrl { get; set; }
    }
}
