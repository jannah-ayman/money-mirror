using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToAchievementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AchievementTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 1,
                column: "Description",
                value: "Answered your first quiz question!");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 2,
                column: "Description",
                value: "Answered 10 quiz questions.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 3,
                column: "Description",
                value: "Answered 20 quiz questions.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 4,
                column: "Description",
                value: "Answered 50 quiz questions!");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 5,
                column: "Description",
                value: "Completed your first savings goal!");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 6,
                column: "Description",
                value: "Completed 3 savings goals.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 7,
                column: "Description",
                value: "Completed 5 savings goals.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 8,
                column: "Description",
                value: "Completed 10 savings goals!");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 9,
                column: "Description",
                value: "Logged your first expense.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 10,
                column: "Description",
                value: "Logged 20 expenses.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 11,
                column: "Description",
                value: "Logged 40 expenses.");

            migrationBuilder.UpdateData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 12,
                column: "Description",
                value: "Logged 100 expenses!");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AchievementTypes");
        }
    }
}
