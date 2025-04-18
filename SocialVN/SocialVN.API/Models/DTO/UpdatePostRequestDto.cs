using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace SocialVN.API.Models.DTO
{
    public class UpdatePostRequestDto
    {
        [Required(ErrorMessage = "Nội dung là bắt buộc.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Nội dung phải có độ dài từ 10 đến 1000 ký tự.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Hình ảnh là bắt buộc.")]
        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
        [EnumDataType(typeof(PostStatus), ErrorMessage = "Giá trị trạng thái không hợp lệ.")]
        public PostStatus Status
        {
            get; set;
        }
    }
}
