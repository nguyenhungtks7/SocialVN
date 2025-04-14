using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email phải là email hợp lệ")]
        public string Email { get; set; }
    }
}
