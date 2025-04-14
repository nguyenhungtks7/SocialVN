using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        [EmailAddress(ErrorMessage = "Username phải là email hợp lệ")]
        public string Usename { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
