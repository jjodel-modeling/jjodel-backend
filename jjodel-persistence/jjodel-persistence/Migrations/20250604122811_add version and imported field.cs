using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jjodel_persistence.Migrations
{
    /// <inheritdoc />
    public partial class addversionandimportedfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Imported",
                table: "Projects",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Projects",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imported",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Projects");
        }
    }
}
