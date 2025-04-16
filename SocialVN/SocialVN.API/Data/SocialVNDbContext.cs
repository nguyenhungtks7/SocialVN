using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Data
{
    public class SocialVNDbContext : IdentityDbContext<ApplicationUser>
    {
        public SocialVNDbContext(DbContextOptions<SocialVNDbContext> options) : base(options)
        {
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Report> Reports { get; set; }

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

            // Configuration for Post entity
            builder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .HasColumnType("char(36)")
                      .HasComment("ID duy nhất của bài viết (UUID)");
                entity.Property(p => p.UserId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người dùng tạo bài viết");
                entity.Property(p => p.Content)
                      .HasComment("Nội dung bài viết");
                entity.Property(p => p.Image)
                      .HasComment("Đường dẫn ảnh của bài viết (nếu có)");
                entity.Property(p => p.CreatedAt)
                      .IsRequired()
                      .HasComment("Thời gian tạo bài viết");
                entity.Property(p => p.UpdatedAt)
                      .HasComment("Thời gian cập nhật bài viết");

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuration for Comment entity
            builder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                      .HasColumnType("char(36)")
                      .HasComment("ID duy nhất của comment (UUID)");
                entity.Property(c => c.PostId)
                      .IsRequired()
                      .HasColumnType("char(36)")
                      .HasComment("ID bài viết mà comment thuộc về");
                entity.Property(c => c.UserId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người tạo comment");
                entity.Property(c => c.Content)
                      .IsRequired()
                      .HasComment("Nội dung comment");
                entity.Property(c => c.CreatedAt)
                      .IsRequired()
                      .HasComment("Thời gian tạo comment");
                entity.Property(c => c.UpdatedAt)
                      .HasComment("Thời gian cập nhật comment");

                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuration for Like entity
            builder.Entity<Like>(entity =>
            {
                entity.ToTable("Likes");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Id)
                      .HasColumnType("char(36)")
                      .HasComment("ID duy nhất của lượt thích (UUID)");
                entity.Property(l => l.PostId)
                      .IsRequired()
                      .HasColumnType("char(36)")
                      .HasComment("ID bài viết được thích");
                entity.Property(l => l.UserId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người thực hiện thích bài viết");
                entity.Property(l => l.CreatedAt)
                      .IsRequired()
                      .HasComment("Thời gian thực hiện thích bài viết");
                entity.Property(l => l.UpdatedAt)
                      .HasComment("Thời gian cập nhật lượt thích");

                entity.HasOne(l => l.Post)
                      .WithMany(p => p.Likes)
                      .HasForeignKey(l => l.PostId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuration for Friendship entity
            builder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id)
                      .HasColumnType("char(36)")
                      .HasComment("ID duy nhất của yêu cầu kết bạn (UUID)");
                entity.Property(f => f.RequesterId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người gửi yêu cầu kết bạn");
                entity.Property(f => f.ReceiverId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người nhận yêu cầu kết bạn");
                entity.Property(f => f.Status)
                      .IsRequired()
                      .HasDefaultValue("pending")
                      .HasMaxLength(50)
                      .HasComment("Trạng thái của yêu cầu kết bạn (pending/accepted/declined)");
                entity.Property(f => f.CreatedAt)
                      .IsRequired()
                      .HasComment("Thời gian gửi yêu cầu");
                entity.Property(f => f.UpdatedAt)
                      .HasComment("Thời gian cập nhật trạng thái yêu cầu");

                entity.HasOne(f => f.Requester)
                      .WithMany(u => u.FriendshipsAsRequester)
                      .HasForeignKey(f => f.RequesterId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Friendships_Requester");

                entity.HasOne(f => f.Receiver)
                      .WithMany(u => u.FriendshipsAsReceiver)
                      .HasForeignKey(f => f.ReceiverId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_Friendships_Receiver");
            });

            // Configuration for Report entity
            builder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id)
                      .HasColumnType("char(36)")
                      .HasComment("ID duy nhất của báo cáo (UUID)");
                entity.Property(r => r.UserId)
                      .IsRequired()
                      .HasColumnType("varchar(36)") // Đổi từ char(36) sang varchar(36)
                      .HasComment("ID của người dùng thực hiện báo cáo");
                entity.Property(r => r.WeekStart)
                      .IsRequired()
                      .HasComment("Ngày bắt đầu của tuần được báo cáo");
                entity.Property(r => r.WeekEnd)
                      .IsRequired()
                      .HasComment("Ngày kết thúc của tuần được báo cáo");
                entity.Property(r => r.TotalPosts)
                      .IsRequired()
                      .HasComment("Tổng số bài viết trong tuần");
                entity.Property(r => r.NewFriends)
                      .IsRequired()
                      .HasComment("Số lượng bạn bè mới trong tuần");
                entity.Property(r => r.TotalLikes)
                      .IsRequired()
                      .HasComment("Tổng số lượt thích trong tuần");
                entity.Property(r => r.TotalComments)
                      .IsRequired()
                      .HasComment("Tổng số lượt comment trong tuần");
                entity.Property(r => r.CreatedAt)
                      .IsRequired()
                      .HasComment("Thời gian tạo báo cáo");
                entity.Property(r => r.UpdatedAt)
                      .HasComment("Thời gian cập nhật báo cáo");

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reports)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }    
    }
}
