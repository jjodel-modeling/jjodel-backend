using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jjodel_persistence.Migrations
{
    /// <inheritdoc />
    public partial class smallupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContextJson",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "DState",
                table: "ClientLogs");

            migrationBuilder.RenameColumn(
                name: "TransientJson",
                table: "ClientLogs",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ClientLogs",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "StackTrace",
                table: "ClientLogs",
                newName: "ReactMsg");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Browser",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrowserMajorVersion",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BrowserVersion",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Cookies",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Error",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Mobile",
                table: "ClientLogs",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Os",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OsVersion",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Screen",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "ClientLogs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Browser",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "BrowserMajorVersion",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "BrowserVersion",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "Cookies",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "Error",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "Os",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "OsVersion",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "Screen",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "ClientLogs");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "ClientLogs",
                newName: "TransientJson");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "ClientLogs",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "ReactMsg",
                table: "ClientLogs",
                newName: "StackTrace");

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                table: "ClientLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "ClientLogs",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ContextJson",
                table: "ClientLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DState",
                table: "ClientLogs",
                type: "text",
                nullable: true);
        }
    }
}
