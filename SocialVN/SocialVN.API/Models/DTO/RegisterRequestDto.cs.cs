using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username là bắt buộc")]
        [EmailAddress(ErrorMessage = "Username phải là email hợp lệ")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        [MinLength(6, ErrorMessage = "Password phải có ít nhất 6 ký tự")]
        public string Password { get; set; }

        //public List<string>? Roles { get; set; }
    }
}
