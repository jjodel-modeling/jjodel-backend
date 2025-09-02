using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jjodel_persistence.Migrations
{
    /// <inheritdoc />
    public partial class OriginalAuthor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalAuthorId",
                table: "Projects",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OriginalAuthorId",
                table: "Projects",
                column: "OriginalAuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AspNetUsers_OriginalAuthorId",
                table: "Projects",
                column: "OriginalAuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AspNetUsers_OriginalAuthorId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OriginalAuthorId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "OriginalAuthorId",
                table: "Projects");
        }
    }
}
