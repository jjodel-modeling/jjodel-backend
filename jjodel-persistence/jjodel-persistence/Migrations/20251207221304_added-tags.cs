using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jjodel_persistence.Migrations
{
    /// <inheritdoc />
    public partial class addedtags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TagId",
                table: "Projects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Creation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_TagId",
                table: "Projects",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AuthorId",
                table: "Tags",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Tags_TagId",
                table: "Projects",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Tags_TagId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Projects_TagId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Projects");
        }
    }
}
