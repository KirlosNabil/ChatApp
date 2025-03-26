using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class addedDateToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Notifications",
                newName: "Content");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Notifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Notifications",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notifications",
                newName: "content");
        }
    }
}
