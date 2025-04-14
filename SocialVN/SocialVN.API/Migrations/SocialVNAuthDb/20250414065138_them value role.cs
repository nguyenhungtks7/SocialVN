using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SocialVN.API.Migrations.SocialVNAuthDb
{
    /// <inheritdoc />
    public partial class themvaluerole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "14e17e30-ec09-4fc3-bc9f-628628c5e38e", "14e17e30-ec09-4fc3-bc9f-628628c5e38e", "Admin", "ADMIN" },
                    { "61189a47-33d0-4006-a157-cc300bb55121", "61189a47-33d0-4006-a157-cc300bb55121", "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "14e17e30-ec09-4fc3-bc9f-628628c5e38e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "61189a47-33d0-4006-a157-cc300bb55121");
        }
    }
}
