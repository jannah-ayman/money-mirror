using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedChildModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageSpendingAmount",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "BargainHuntingScore",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "HasAllowance",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SpendingCategoriesJson",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "TracksExpenses",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "TriesToSave",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ProfileCompletionStatus",
                table: "Children");

            migrationBuilder.RenameColumn(
                name: "EmotionalSpendingScore",
                table: "InitialProfilingQuestionnaires",
                newName: "PrimarySpendingCategory");

            migrationBuilder.AlterColumn<int>(
                name: "SavingSuccessRate",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SavingPercentage",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SavingGoal",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(10,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPersonalityFinalized",
                table: "Children",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LName",
                table: "Children",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPersonalityFinalized",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "LName",
                table: "Children");

            migrationBuilder.RenameColumn(
                name: "PrimarySpendingCategory",
                table: "InitialProfilingQuestionnaires",
                newName: "EmotionalSpendingScore");

            migrationBuilder.AlterColumn<int>(
                name: "SavingSuccessRate",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SavingPercentage",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SavingGoal",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires",
                type: "DECIMAL(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageSpendingAmount",
                table: "InitialProfilingQuestionnaires",
                type: "DECIMAL(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BargainHuntingScore",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasAllowance",
                table: "InitialProfilingQuestionnaires",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SpendingCategoriesJson",
                table: "InitialProfilingQuestionnaires",
                type: "NVARCHAR(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "TracksExpenses",
                table: "InitialProfilingQuestionnaires",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TriesToSave",
                table: "InitialProfilingQuestionnaires",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ProfileCompletionStatus",
                table: "Children",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
