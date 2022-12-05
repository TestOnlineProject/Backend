using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestOnline.Migrations
{
    /// <inheritdoc />
    public partial class finalmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExamUsers_UserId",
                table: "ExamUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ExamUsers_UserId_ExamId",
                table: "ExamUsers",
                columns: new[] { "UserId", "ExamId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExamUsers_UserId_ExamId",
                table: "ExamUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ExamUsers_UserId",
                table: "ExamUsers",
                column: "UserId");
        }
    }
}
