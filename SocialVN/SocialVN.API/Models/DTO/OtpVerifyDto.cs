using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class OtpVerifyDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email phải hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP là bắt buộc")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có 6 chữ số")]
        public string OTP { get; set; }
    }
}
