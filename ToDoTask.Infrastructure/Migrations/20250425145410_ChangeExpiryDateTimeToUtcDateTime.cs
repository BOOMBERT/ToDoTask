using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeExpiryDateTimeToUtcDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDateTime",
                table: "ToDoItems",
                newName: "ExpiryDateTimeUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpiryDateTimeUtc",
                table: "ToDoItems",
                newName: "ExpiryDateTime");
        }
    }
}
