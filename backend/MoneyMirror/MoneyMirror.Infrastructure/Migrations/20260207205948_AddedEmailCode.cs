using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "PasswordResetToken",
                table: "Parents");

            migrationBuilder.RenameColumn(
                name: "PasswordResetTokenExpiry",
                table: "Parents",
                newName: "PasswordResetCodeExpiry");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmationTokenExpiry",
                table: "Parents",
                newName: "EmailConfirmationCodeExpiry");

            migrationBuilder.RenameColumn(
                name: "EmailChangeTokenExpiry",
                table: "Parents",
                newName: "EmailChangeCodeExpiry");

            migrationBuilder.RenameColumn(
                name: "EmailChangeToken",
                table: "Parents",
                newName: "PasswordResetCode");

            migrationBuilder.AddColumn<string>(
                name: "EmailChangeCode",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationCode",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailChangeCode",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationCode",
                table: "Parents");

            migrationBuilder.RenameColumn(
                name: "PasswordResetCodeExpiry",
                table: "Parents",
                newName: "PasswordResetTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "PasswordResetCode",
                table: "Parents",
                newName: "EmailChangeToken");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmationCodeExpiry",
                table: "Parents",
                newName: "EmailConfirmationTokenExpiry");

            migrationBuilder.RenameColumn(
                name: "EmailChangeCodeExpiry",
                table: "Parents",
                newName: "EmailChangeTokenExpiry");

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Parents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetToken",
                table: "Parents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
