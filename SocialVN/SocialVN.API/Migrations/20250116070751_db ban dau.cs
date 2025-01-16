using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialVN.API.Migrations
{
    /// <inheritdoc />
    public partial class dbbandau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID duy nhất của người dùng (UUID)"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Email của người dùng"),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Mật khẩu được mã hóa"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "user", comment: "Vai trò của người dùng"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Họ và tên đầy đủ của người dùng"),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Ngày tháng năm sinh của người dùng"),
                    Occupation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "Nghề nghiệp của người dùng"),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Địa chỉ nơi sinh sống của người dùng"),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Đường dẫn ảnh đại diện của người dùng"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian tạo tài khoản"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật tài khoản")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "pending", comment: "Trạng thái của yêu cầu kết bạn (pending/accepted/declined)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian gửi yêu cầu"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật trạng thái yêu cầu")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Friendships_Receiver",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Friendships_Requester",
                        column: x => x.RequesterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID duy nhất của bài viết (UUID)"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID của người dùng tạo bài viết"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Nội dung bài viết"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Đường dẫn ảnh của bài viết (nếu có)"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian tạo bài viết"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật bài viết")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID duy nhất của báo cáo (UUID)"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID của người dùng thực hiện báo cáo"),
                    WeekStart = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Ngày bắt đầu của tuần được báo cáo"),
                    WeekEnd = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Ngày kết thúc của tuần được báo cáo"),
                    TotalPosts = table.Column<int>(type: "int", nullable: false, comment: "Tổng số bài viết trong tuần"),
                    NewFriends = table.Column<int>(type: "int", nullable: false, comment: "Số lượng bạn bè mới trong tuần"),
                    TotalLikes = table.Column<int>(type: "int", nullable: false, comment: "Tổng số lượt thích trong tuần"),
                    TotalComments = table.Column<int>(type: "int", nullable: false, comment: "Tổng số lượt comment trong tuần"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian tạo báo cáo"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật báo cáo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID duy nhất của comment (UUID)"),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID bài viết mà comment thuộc về"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID của người tạo comment"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Nội dung comment"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian tạo comment"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật comment")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    LikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID duy nhất của lượt thích (UUID)"),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID bài viết được thích"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "ID của người thực hiện thích bài viết"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian thực hiện thích bài viết"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Thời gian cập nhật lượt thích"),
                    UserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.LikeId);
                    table.ForeignKey(
                        name: "FK_Likes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ReceiverId",
                table: "Friendships",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_RequesterId",
                table: "Friendships",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_PostId",
                table: "Likes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId1",
                table: "Likes",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId",
                table: "Posts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
