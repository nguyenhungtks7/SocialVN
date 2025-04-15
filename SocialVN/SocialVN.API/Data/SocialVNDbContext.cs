using System;
using Microsoft.EntityFrameworkCore;
using SocialVN.API.Models.Domain;

namespace SocialVN.API.Data
{
    public class SocialVNDbContext : DbContext
    {
        public SocialVNDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Report> Reports { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Cấu hình bảng User
            //modelBuilder.Entity<User>(entity =>
            //{
            //    entity.ToTable("Users");
            //    entity.HasKey(u => u.Id);
            //    entity.Property(u => u.Id)
            //        .HasComment("ID duy nhất của người dùng (UUID)")
            //        .HasColumnType("varchar(36)"); 
            //    entity.Property(u => u.Email).IsRequired().HasMaxLength(100).HasComment("Email của người dùng");
            //    entity.Property(u => u.PasswordHash).IsRequired().HasComment("Mật khẩu được mã hóa");
            //    entity.Property(u => u.Role).IsRequired().HasDefaultValue("user").HasComment("Vai trò của người dùng");
            //    entity.Property(u => u.FullName).HasMaxLength(200).HasComment("Họ và tên đầy đủ của người dùng");
            //    entity.Property(u => u.BirthDate).HasComment("Ngày tháng năm sinh của người dùng");
            //    entity.Property(u => u.Occupation).HasMaxLength(100).HasComment("Nghề nghiệp của người dùng");
            //    entity.Property(u => u.Location).HasMaxLength(200).HasComment("Địa chỉ nơi sinh sống của người dùng");
            //    entity.Property(u => u.Avatar).HasComment("Đường dẫn ảnh đại diện của người dùng");
            //    entity.Property(u => u.CreatedAt).IsRequired().HasComment("Thời gian tạo tài khoản");
            //    entity.Property(u => u.UpdatedAt).HasComment("Thời gian cập nhật tài khoản");
            //});

            //// Cấu hình bảng Post
            //modelBuilder.Entity<Post>(entity =>
            //{
            //    entity.ToTable("Posts");
            //    entity.HasKey(p => p.Id);
            //    entity.Property(p => p.Id)
            //        .HasComment("ID duy nhất của bài viết (UUID)")
            //        .HasColumnType("varchar(36)");
            //    entity.Property(p => p.UserId).IsRequired().HasComment("ID của người dùng tạo bài viết");
            //    entity.Property(p => p.Content).HasComment("Nội dung bài viết");
            //    entity.Property(p => p.Image).HasComment("Đường dẫn ảnh của bài viết (nếu có)");
            //    entity.Property(p => p.CreatedAt).IsRequired().HasComment("Thời gian tạo bài viết");
            //    entity.Property(p => p.UpdatedAt).HasComment("Thời gian cập nhật bài viết");

            //    entity.HasOne(p => p.User)
            //          .WithMany(u => u.Posts)
            //          .HasForeignKey(p => p.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);
            //});

            //// Cấu hình bảng Comment
            //modelBuilder.Entity<Comment>(entity =>
            //{
            //    entity.ToTable("Comments");
            //    entity.HasKey(c => c.Id);
            //    entity.Property(c => c.Id)
            //        .HasComment("ID duy nhất của comment (UUID)")
            //        .HasColumnType("varchar(36)");
            //    entity.Property(c => c.PostId).IsRequired().HasComment("ID bài viết mà comment thuộc về");
            //    entity.Property(c => c.UserId).IsRequired().HasComment("ID của người tạo comment");
            //    entity.Property(c => c.Content).IsRequired().HasComment("Nội dung comment");
            //    entity.Property(c => c.CreatedAt).IsRequired().HasComment("Thời gian tạo comment");
            //    entity.Property(c => c.UpdatedAt).HasComment("Thời gian cập nhật comment");

            //    // Cấu hình quan hệ với Post (OnDelete Cascade)
            //    entity.HasOne(c => c.Post)
            //          .WithMany(p => p.Comments)
            //          .HasForeignKey(c => c.PostId)
            //          .OnDelete(DeleteBehavior.Restrict); 

            //    // Cấu hình quan hệ với User (OnDelete No Action)
            //    entity.HasOne(c => c.User)
            //           .WithMany(u => u.Comments)  //=
            //           .HasForeignKey(c => c.UserId) 
            //           .OnDelete(DeleteBehavior.Restrict); 
            //});

            //// Cấu hình bảng Like
            //modelBuilder.Entity<Like>(entity =>
            //{
            //    entity.ToTable("Likes");
            //    entity.HasKey(l => l.Id);
            //    entity.Property(l => l.Id)
            //        .HasComment("ID duy nhất của lượt thích (UUID)")
            //        .HasColumnType("varchar(36)"); 
            //    entity.Property(l => l.PostId).IsRequired().HasComment("ID bài viết được thích");
            //    entity.Property(l => l.UserId).IsRequired().HasComment("ID của người thực hiện thích bài viết");
            //    entity.Property(l => l.CreatedAt).IsRequired().HasComment("Thời gian thực hiện thích bài viết");
            //    entity.Property(l => l.UpdatedAt).HasComment("Thời gian cập nhật lượt thích");

            //    entity.HasOne(l => l.Post)
            //          .WithMany(p => p.Likes)
            //          .HasForeignKey(l => l.PostId)
            //          .OnDelete(DeleteBehavior.Restrict);

            //    entity.HasOne(l => l.User)
            //    .WithMany(u => u.Likes)
            //      .HasForeignKey(l => l.UserId)
            //       .OnDelete(DeleteBehavior.Restrict);
            //});

            //// Cấu hình bảng Friendship
            //modelBuilder.Entity<Friendship>(entity =>
            //{
            //    entity.ToTable("Friendships");
            //    entity.HasKey(f => f.Id);

            //    entity.Property(f => f.Status)
            //          .IsRequired()
            //          .HasDefaultValue("pending")
            //          .HasMaxLength(50)
            //          .HasComment("Trạng thái của yêu cầu kết bạn (pending/accepted/declined)");

            //    entity.Property(f => f.CreatedAt)
            //          .IsRequired()
            //          .HasComment("Thời gian gửi yêu cầu");

            //    entity.Property(f => f.UpdatedAt)
            //          .HasComment("Thời gian cập nhật trạng thái yêu cầu");

            //    // Cấu hình quan hệ giữa User và Friendship
            //    entity.HasOne(f => f.Requester)
            //          .WithMany(u => u.FriendshipsAsRequester)
            //          .HasForeignKey(f => f.RequesterId)
            //          .OnDelete(DeleteBehavior.Restrict)
            //          .HasConstraintName("FK_Friendships_Requester");

            //    entity.HasOne(f => f.Receiver)
            //          .WithMany(u => u.FriendshipsAsReceiver)
            //          .HasForeignKey(f => f.ReceiverId)
            //          .OnDelete(DeleteBehavior.Restrict)
            //          .HasConstraintName("FK_Friendships_Receiver");
            //});

            //// Cấu hình bảng Report
            //modelBuilder.Entity<Report>(entity =>
            //{
            //    entity.ToTable("Reports");
            //    entity.HasKey(r => r.Id);
            //    entity.Property(r => r.Id)
            //        .HasComment("ID duy nhất của báo cáo (UUID)")
            //        .HasColumnType("varchar(36)"); // Chuyển đổi Guid sang varchar(36)
            //    entity.Property(r => r.UserId).IsRequired().HasComment("ID của người dùng thực hiện báo cáo");
            //    entity.Property(r => r.WeekStart).IsRequired().HasComment("Ngày bắt đầu của tuần được báo cáo");
            //    entity.Property(r => r.WeekEnd).IsRequired().HasComment("Ngày kết thúc của tuần được báo cáo");
            //    entity.Property(r => r.TotalPosts).IsRequired().HasComment("Tổng số bài viết trong tuần");
            //    entity.Property(r => r.NewFriends).IsRequired().HasComment("Số lượng bạn bè mới trong tuần");
            //    entity.Property(r => r.TotalLikes).IsRequired().HasComment("Tổng số lượt thích trong tuần");
            //    entity.Property(r => r.TotalComments).IsRequired().HasComment("Tổng số lượt comment trong tuần");
            //    entity.Property(r => r.CreatedAt).IsRequired().HasComment("Thời gian tạo báo cáo");
            //    entity.Property(r => r.UpdatedAt).HasComment("Thời gian cập nhật báo cáo");

            //    entity.HasOne(r => r.User)
            //          .WithMany(u => u.Reports)
            //          .HasForeignKey(r => r.UserId)
            //          .OnDelete(DeleteBehavior.Cascade);
            //});

        }
    }
}
