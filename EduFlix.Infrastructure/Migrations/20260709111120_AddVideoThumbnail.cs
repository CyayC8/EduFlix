using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduFlix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoThumbnail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailBlobName",
                table: "Videos",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailBlobName",
                table: "Videos");
        }
    }
}
