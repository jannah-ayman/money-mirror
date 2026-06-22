using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddParentRoleToParentChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "ParentChildren",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ParentChildren");
        }
    }
}
