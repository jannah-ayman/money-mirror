using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarUrlAndSelectedCharacterToChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Children",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCharacter",
                table: "Children",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "SelectedCharacter",
                table: "Children");
        }
    }
}
