using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizLogAddQuizAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_TypeID",
                table: "QuizLogs");

            migrationBuilder.DropColumn(
                name: "AnswerOptions",
                table: "StoryQuizTemplates");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "QuizLogs");

            migrationBuilder.DropColumn(
                name: "ScoreValue",
                table: "QuizLogs");

            migrationBuilder.DropColumn(
                name: "SelectedAnswerIndex",
                table: "QuizLogs");

            migrationBuilder.RenameColumn(
                name: "TypeID",
                table: "QuizLogs",
                newName: "AnswerID");

            migrationBuilder.RenameIndex(
                name: "IX_QuizLogs_TypeID",
                table: "QuizLogs",
                newName: "IX_QuizLogs_AnswerID");

            migrationBuilder.AddColumn<int>(
                name: "PersonalityTypeTypeID",
                table: "QuizLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    AnswerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FeedbackMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StoryID = table.Column<int>(type: "int", nullable: false),
                    PersonalityTypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.AnswerID);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_PersonalityTypes_PersonalityTypeID",
                        column: x => x.PersonalityTypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_StoryQuizTemplates_StoryID",
                        column: x => x.StoryID,
                        principalTable: "StoryQuizTemplates",
                        principalColumn: "StoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizLog_Child_Story_Unique",
                table: "QuizLogs",
                columns: new[] { "ChildID", "StoryID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_PersonalityTypeTypeID",
                table: "QuizLogs",
                column: "PersonalityTypeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_PersonalityTypeID",
                table: "QuizAnswers",
                column: "PersonalityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_StoryID",
                table: "QuizAnswers",
                column: "StoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_PersonalityTypeTypeID",
                table: "QuizLogs",
                column: "PersonalityTypeTypeID",
                principalTable: "PersonalityTypes",
                principalColumn: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLogs_QuizAnswers_AnswerID",
                table: "QuizLogs",
                column: "AnswerID",
                principalTable: "QuizAnswers",
                principalColumn: "AnswerID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_PersonalityTypeTypeID",
                table: "QuizLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizLogs_QuizAnswers_AnswerID",
                table: "QuizLogs");

            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuizLog_Child_Story_Unique",
                table: "QuizLogs");

            migrationBuilder.DropIndex(
                name: "IX_QuizLogs_PersonalityTypeTypeID",
                table: "QuizLogs");

            migrationBuilder.DropColumn(
                name: "PersonalityTypeTypeID",
                table: "QuizLogs");

            migrationBuilder.RenameColumn(
                name: "AnswerID",
                table: "QuizLogs",
                newName: "TypeID");

            migrationBuilder.RenameIndex(
                name: "IX_QuizLogs_AnswerID",
                table: "QuizLogs",
                newName: "IX_QuizLogs_TypeID");

            migrationBuilder.AddColumn<string>(
                name: "AnswerOptions",
                table: "StoryQuizTemplates",
                type: "NVARCHAR(MAX)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "QuizLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScoreValue",
                table: "QuizLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectedAnswerIndex",
                table: "QuizLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_QuizLogs_PersonalityTypes_TypeID",
                table: "QuizLogs",
                column: "TypeID",
                principalTable: "PersonalityTypes",
                principalColumn: "TypeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
