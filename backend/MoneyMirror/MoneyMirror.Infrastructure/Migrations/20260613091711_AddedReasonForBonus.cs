using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedReasonForBonus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Allowances",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Allowances");
        }
    }
}
