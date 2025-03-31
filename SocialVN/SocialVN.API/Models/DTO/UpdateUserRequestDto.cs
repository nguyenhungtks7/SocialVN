namespace SocialVN.API.Models.DTO
{
    public class UpdateUserRequestDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Occupation { get; set; }
        public string Location { get; set; }
        public string Avatar { get; set; }
    }
}
