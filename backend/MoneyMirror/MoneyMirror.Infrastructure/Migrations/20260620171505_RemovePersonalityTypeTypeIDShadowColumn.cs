using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePersonalityTypeTypeIDShadowColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_PersonalityTypeTypeID",
                table: "QuizLogs");

            migrationBuilder.DropIndex(
                name: "IX_QuizLogs_PersonalityTypeTypeID",
                table: "QuizLogs");

            migrationBuilder.DropColumn(
                name: "PersonalityTypeTypeID",
                table: "QuizLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonalityTypeTypeID",
                table: "QuizLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_PersonalityTypeTypeID",
                table: "QuizLogs",
                column: "PersonalityTypeTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_PersonalityTypeTypeID",
                table: "QuizLogs",
                column: "PersonalityTypeTypeID",
                principalTable: "PersonalityTypes",
                principalColumn: "TypeID");
        }
    }
}
