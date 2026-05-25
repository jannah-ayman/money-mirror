using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAchievements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Criteria",
                table: "AchievementTypes");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseCount",
                table: "Children",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalCount",
                table: "Children",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuizCount",
                table: "Children",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "IconURL",
                table: "AchievementTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AchievementTypes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Threshold",
                table: "AchievementTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AchievementTypes",
                columns: new[] { "AchievementTypeID", "Category", "IconURL", "Name", "Threshold" },
                values: new object[,]
                {
                    { 1, "Quiz", "/images/badges/first-step.png", "First Step", 1 },
                    { 2, "Quiz", "/images/badges/quiz-explorer.png", "Quiz Explorer", 10 },
                    { 3, "Quiz", "/images/badges/quiz-master.png", "Quiz Master", 20 },
                    { 4, "Quiz", "/images/badges/quiz-legend.png", "Quiz Legend", 50 },
                    { 5, "Goal", "/images/badges/goal-getter.png", "Goal Getter", 1 },
                    { 6, "Goal", "/images/badges/determined.png", "Determined", 3 },
                    { 7, "Goal", "/images/badges/achiever.png", "Achiever", 5 },
                    { 8, "Goal", "/images/badges/champion.png", "Champion", 10 },
                    { 9, "Expense", "/images/badges/first-purchase.png", "First Purchase", 1 },
                    { 10, "Expense", "/images/badges/expense-tracker.png", "Expense Tracker", 20 },
                    { 11, "Expense", "/images/badges/money-logger.png", "Money Logger", 40 },
                    { 12, "Expense", "/images/badges/financial-pro.png", "Financial Pro", 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AchievementTypes",
                keyColumn: "AchievementTypeID",
                keyValue: 12);

            migrationBuilder.DropColumn(
                name: "ExpenseCount",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "GoalCount",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "QuizCount",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AchievementTypes");

            migrationBuilder.DropColumn(
                name: "Threshold",
                table: "AchievementTypes");

            migrationBuilder.AlterColumn<string>(
                name: "IconURL",
                table: "AchievementTypes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Criteria",
                table: "AchievementTypes",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }
    }
}
