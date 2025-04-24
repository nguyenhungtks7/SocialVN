using SocialVN.API.Enums;
using SocialVN.API.Models.Domain;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.DTO
{
    public class PostDto : BaseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public PostStatus Status { get; set; }
        public bool IsEdited { get; set; }
        // Thông tin người dùng
        public string UserName { get; set; }
        public string? AvatarPath { get; set; }

        // Thống kê
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }

        // Thuộc tính điều hướng

    }

}
