using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question1Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "Question2Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "Question3Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "Question4Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "Question5Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "Question6Response",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "QuestionResponse",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.AlterColumn<int>(
                name: "CalculatedTypeID",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires",
                type: "DECIMAL(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "ChildAgeGroup",
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
                name: "EmotionalSpendingScore",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FeelingAfterSpending",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FeelingWhenSavingGrows",
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

            migrationBuilder.AddColumn<int>(
                name: "MoneyMindset",
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
                name: "OutOfMoneyBehavior",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReactionTo100",
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
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SavingPercentage",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SavingSuccessRate",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpendingAffectsSaving",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SpendingCategoriesJson",
                table: "InitialProfilingQuestionnaires",
                type: "NVARCHAR(500)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SpendingPace",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpendingPlanning",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TalksAboutMoney",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowanceAmount",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "AllowanceFrequency",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "AverageSpendingAmount",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "BargainHuntingScore",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ChildAgeGroup",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ChildGender",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "EmotionalSpendingScore",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "FeelingAfterSpending",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "FeelingWhenSavingGrows",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "HasAllowance",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "MoneyMindset",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "MoneyPriority",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "OutOfMoneyBehavior",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "ReactionTo100",
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

            migrationBuilder.DropColumn(
                name: "SpendingCategoriesJson",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SpendingPace",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "SpendingPlanning",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "TalksAboutMoney",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "TracksExpenses",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.DropColumn(
                name: "TriesToSave",
                table: "InitialProfilingQuestionnaires");

            migrationBuilder.AlterColumn<int>(
                name: "CalculatedTypeID",
                table: "InitialProfilingQuestionnaires",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Question1Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question2Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question3Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question4Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question5Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Question6Response",
                table: "InitialProfilingQuestionnaires",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "QuestionResponse",
                table: "InitialProfilingQuestionnaires",
                type: "NVARCHAR(MAX)",
                nullable: true);
        }
    }
}
