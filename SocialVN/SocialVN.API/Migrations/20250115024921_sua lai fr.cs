using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialVN.API.Migrations
{
    /// <inheritdoc />
    public partial class sualaifr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_RequesterId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_UserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Friendships");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "pending",
                comment: "Trạng thái của yêu cầu kết bạn (pending/accepted/declined)",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "pending",
                oldComment: "Trạng thái của yêu cầu (pending/accepted/declined)");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "ID của người gửi yêu cầu");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "ID của người nhận yêu cầu");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "ID duy nhất của yêu cầu kết bạn (UUID)");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_RequesterId",
                table: "Friendships",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_Users_RequesterId",
                table: "Friendships");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Friendships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "pending",
                comment: "Trạng thái của yêu cầu (pending/accepted/declined)",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "pending",
                oldComment: "Trạng thái của yêu cầu kết bạn (pending/accepted/declined)");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequesterId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                comment: "ID của người gửi yêu cầu",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReceiverId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                comment: "ID của người nhận yêu cầu",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: false,
                comment: "ID duy nhất của yêu cầu kết bạn (UUID)",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Friendships",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_ReceiverId",
                table: "Friendships",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_RequesterId",
                table: "Friendships",
                column: "RequesterId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_Users_UserId",
                table: "Friendships",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
