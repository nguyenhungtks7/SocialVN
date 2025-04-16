using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialVN.API.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {

        public string? FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Occupation { get; set; }
        public string? Location { get; set; }
        public string? LivingPlace { get; set; } 
        public string? Gender { get; set; }
        [NotMapped]
        public IFormFile? Avatar { get; set; }

        public string? AvatarPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Thuộc tính điều hướng
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Friendship> FriendshipsAsRequester { get; set; } = new List<Friendship>();
        public ICollection<Friendship> FriendshipsAsReceiver { get; set; } = new List<Friendship>();
        public ICollection<Report> Reports { get; set; } = new List<Report>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
