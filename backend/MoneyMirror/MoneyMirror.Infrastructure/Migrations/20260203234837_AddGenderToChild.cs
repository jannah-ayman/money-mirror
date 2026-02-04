using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderToChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Children",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Children");
        }
    }
}
