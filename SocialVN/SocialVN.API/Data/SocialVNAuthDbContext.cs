using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Data
{
    public class SocialVNAuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public SocialVNAuthDbContext(DbContextOptions<SocialVNAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Thêm các cấu hình tùy chỉnh nếu cần
            var readerRoleId = "14e17e30-ec09-4fc3-bc9f-628628c5e38e";
            var writerRoleId = "61189a47-33d0-4006-a157-cc300bb55121";
            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    ConcurrencyStamp = readerRoleId,
                    Name ="Admin",
                    NormalizedName="Admin".ToUpper(),
                }
                ,
                new IdentityRole
                {
                    Id = writerRoleId,
                    ConcurrencyStamp= writerRoleId,
                    Name ="User",
                    NormalizedName = "User".ToUpper(),
                }
            };
            // Thêm roles vào ModelBuilder
            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
