using System;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Models;

namespace SocialVN.API.Data
{
    public class SocialVNDbContext : DbContext
    {
        public SocialVNDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }
        DbSet<Comment> comments;
        DbSet<User> users;
        DbSet<Friendship> friendships;
        DbSet<Like> like;
        DbSet<Post> posts;
        DbSet<Report> reports;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình bảng User
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).HasComment("ID duy nhất của người dùng (UUID)");
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100).HasComment("Email của người dùng");
                entity.Property(u => u.PasswordHash).IsRequired().HasComment("Mật khẩu được mã hóa");
                entity.Property(u => u.Role).IsRequired().HasDefaultValue("user").HasComment("Vai trò của người dùng");
                entity.Property(u => u.FullName).HasMaxLength(200).HasComment("Họ và tên đầy đủ của người dùng");
                entity.Property(u => u.BirthDate).HasComment("Ngày tháng năm sinh của người dùng");
                entity.Property(u => u.Occupation).HasMaxLength(100).HasComment("Nghề nghiệp của người dùng");
                entity.Property(u => u.Location).HasMaxLength(200).HasComment("Địa chỉ nơi sinh sống của người dùng");
                entity.Property(u => u.Avatar).HasComment("Đường dẫn ảnh đại diện của người dùng");
                entity.Property(u => u.CreatedAt).IsRequired().HasComment("Thời gian tạo tài khoản");
                entity.Property(u => u.UpdatedAt).HasComment("Thời gian cập nhật tài khoản");
            });


            // Cấu hình bảng Post
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Posts");
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id).HasComment("ID duy nhất của bài viết (UUID)");
                entity.Property(p => p.UserId).IsRequired().HasComment("ID của người dùng tạo bài viết");
                entity.Property(p => p.Content).HasComment("Nội dung bài viết");
                entity.Property(p => p.Image).HasComment("Đường dẫn ảnh của bài viết (nếu có)");
                entity.Property(p => p.CreatedAt).IsRequired().HasComment("Thời gian tạo bài viết");
                entity.Property(p => p.UpdatedAt).HasComment("Thời gian cập nhật bài viết");

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Posts)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Cấu hình bảng Comment
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("Comments");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasComment("ID duy nhất của comment (UUID)");
                entity.Property(c => c.PostId).IsRequired().HasComment("ID bài viết mà comment thuộc về");
                entity.Property(c => c.UserId).IsRequired().HasComment("ID của người tạo comment");
                entity.Property(c => c.Content).IsRequired().HasComment("Nội dung comment");
                entity.Property(c => c.CreatedAt).IsRequired().HasComment("Thời gian tạo comment");
                entity.Property(c => c.UpdatedAt).HasComment("Thời gian cập nhật comment");

                entity.HasOne(c => c.Post)
                      .WithMany(p => p.Comments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany()
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Cấu hình bảng Like
            modelBuilder.Entity<Like>(entity =>
            {
                entity.ToTable("Likes");
                entity.HasKey(l => l.LikeId);
                entity.Property(l => l.LikeId).HasComment("ID duy nhất của lượt thích (UUID)");
                entity.Property(l => l.PostId).IsRequired().HasComment("ID bài viết được thích");
                entity.Property(l => l.UserId).IsRequired().HasComment("ID của người thực hiện thích bài viết");
                entity.Property(l => l.CreatedAt).IsRequired().HasComment("Thời gian thực hiện thích bài viết");
                entity.Property(l => l.UpdatedAt).HasComment("Thời gian cập nhật lượt thích");

                entity.HasOne(l => l.Post)
                      .WithMany(p => p.Likes)
                      .HasForeignKey(l => l.PostId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(l => l.User)
                      .WithMany()
                      .HasForeignKey(l => l.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Cấu hình bảng Friendship
            // Cấu hình bảng Friendship
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.Id).HasComment("ID duy nhất của yêu cầu kết bạn (UUID)");
                entity.Property(f => f.RequesterId).IsRequired().HasComment("ID của người gửi yêu cầu");
                entity.Property(f => f.ReceiverId).IsRequired().HasComment("ID của người nhận yêu cầu");
                entity.Property(f => f.Status).IsRequired().HasDefaultValue("pending").HasComment("Trạng thái của yêu cầu (pending/accepted/declined)");
                entity.Property(f => f.CreatedAt).IsRequired().HasComment("Thời gian gửi yêu cầu");
                entity.Property(f => f.UpdatedAt).HasComment("Thời gian cập nhật trạng thái yêu cầu");

                entity.HasOne(f => f.Requester)
                      .WithMany()
                      .HasForeignKey(f => f.RequesterId)
                      .OnDelete(DeleteBehavior.NoAction); // Thay đổi từ Cascade sang NoAction

                entity.HasOne(f => f.Receiver)
                      .WithMany()
                      .HasForeignKey(f => f.ReceiverId)
                      .OnDelete(DeleteBehavior.NoAction); // Thay đổi từ Cascade sang NoAction
            });



            // Cấu hình bảng Report
            modelBuilder.Entity<Report>(entity =>
            {
                entity.ToTable("Reports");
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Id).HasComment("ID duy nhất của báo cáo (UUID)");
                entity.Property(r => r.UserId).IsRequired().HasComment("ID của người dùng thực hiện báo cáo");
                entity.Property(r => r.WeekStart).IsRequired().HasComment("Ngày bắt đầu của tuần được báo cáo");
                entity.Property(r => r.WeekEnd).IsRequired().HasComment("Ngày kết thúc của tuần được báo cáo");
                entity.Property(r => r.TotalPosts).IsRequired().HasComment("Tổng số bài viết trong tuần");
                entity.Property(r => r.NewFriends).IsRequired().HasComment("Số lượng bạn bè mới trong tuần");
                entity.Property(r => r.TotalLikes).IsRequired().HasComment("Tổng số lượt thích trong tuần");
                entity.Property(r => r.TotalComments).IsRequired().HasComment("Tổng số lượt comment trong tuần");
                entity.Property(r => r.CreatedAt).IsRequired().HasComment("Thời gian tạo báo cáo");
                entity.Property(r => r.UpdatedAt).HasComment("Thời gian cập nhật báo cáo");

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Reports)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}
