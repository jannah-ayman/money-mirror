using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSurveyQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ChildGender",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "MoneyPriority",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "PrimarySpendingCategory",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ReactionToExpensiveItem",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ReactionToMoreAllowance",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SatisfactionPreference",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SavingFailureReason",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SavingGoal",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SavingPercentage",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SavingSuccessRate",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SpendingAffectsSaving",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.RenameColumn(
                name: "TalksAboutMoney",
                table: "InitialProfilingQuestionnaires",
                newName: "TriesToSave");

            migrationBuilder.RenameColumn(
                name: "SpendingPlanning",
                table: "InitialProfilingQuestionnaires",
                newName: "HasAllowance");

            migrationBuilder.AddColumn<string>(
                name: "SpendingCategories",
                table: "InitialProfilingQuestionnaires",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpendingCategories",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.RenameColumn(
                name: "TriesToSave",
                table: "InitialProfilingQuestionnaires",
                newName: "TalksAboutMoney");

            migrationBuilder.RenameColumn(
                name: "HasAllowance",
                table: "InitialProfilingQuestionnaires",
                newName: "SpendingPlanning");

            migrationBuilder.AddColumn<decimal>(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildGender",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MoneyPriority",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrimarySpendingCategory",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReactionToExpensiveItem",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReactionToMoreAllowance",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SatisfactionPreference",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavingFailureReason",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavingGoal",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavingPercentage",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SavingSuccessRate",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpendingAffectsSaving",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
