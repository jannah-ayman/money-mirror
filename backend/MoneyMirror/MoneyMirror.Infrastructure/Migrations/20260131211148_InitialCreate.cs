using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AchievementTypes",
                columns: table => new
                {
                    AchievementTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IconURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.AchievementTypeID);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStats",
                columns: table => new
                {
                    StatsID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnimationURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStats", x => x.StatsID);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseCategories", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Moods",
                columns: table => new
                {
                    MoodID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Emoji = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moods", x => x.MoodID);
                });

            migrationBuilder.CreateTable(
                name: "Parents",
                columns: table => new
                {
                    ParentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsEmailConfirmed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    EmailConfirmationToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EmailConfirmationTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parents", x => x.ParentID);
                });

            migrationBuilder.CreateTable(
                name: "PersonalityTypes",
                columns: table => new
                {
                    TypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChildName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Traits = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    FunFacts = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StaticRecommendation = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalityTypes", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "StoryQuizTemplates",
                columns: table => new
                {
                    StoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Scenario = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnswerOptions = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    TargetAgeMin = table.Column<int>(type: "int", nullable: false),
                    TargetAgeMax = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryQuizTemplates", x => x.StoryID);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    ChildID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    ProfileCompletionStatus = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TypeID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.ChildID);
                    table.ForeignKey(
                        name: "FK_Children_PersonalityTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Advices",
                columns: table => new
                {
                    AdviceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    SourceTrigger = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advices", x => x.AdviceID);
                    table.ForeignKey(
                        name: "FK_Advices_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allowances",
                columns: table => new
                {
                    AllowanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SetDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    GivenDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allowances", x => x.AllowanceID);
                    table.ForeignKey(
                        name: "FK_Allowances_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allowances_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChildAchievements",
                columns: table => new
                {
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    AchievementTypeID = table.Column<int>(type: "int", nullable: false),
                    EarnedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildAchievements", x => new { x.ChildID, x.AchievementTypeID });
                    table.ForeignKey(
                        name: "FK_ChildAchievements_AchievementTypes_AchievementTypeID",
                        column: x => x.AchievementTypeID,
                        principalTable: "AchievementTypes",
                        principalColumn: "AchievementTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChildAchievements_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChildCharacterStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatsData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    TriggerEvent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StatsID = table.Column<int>(type: "int", nullable: false),
                    ChildID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildCharacterStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                        column: x => x.StatsID,
                        principalTable: "CharacterStats",
                        principalColumn: "StatsID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChildCharacterStats_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MoneyAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    MoodID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseID);
                    table.ForeignKey(
                        name: "FK_Expenses_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "ExpenseCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Moods_MoodID",
                        column: x => x.MoodID,
                        principalTable: "Moods",
                        principalColumn: "MoodID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InitialProfilingQuestionnaires",
                columns: table => new
                {
                    QuestionnaireID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question1Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Question2Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Question3Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Question4Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Question5Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Question6Response = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuestionResponse = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    CalculatedTypeID = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitialProfilingQuestionnaires", x => x.QuestionnaireID);
                    table.ForeignKey(
                        name: "FK_InitialProfilingQuestionnaires_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitialProfilingQuestionnaires_PersonalityTypes_CalculatedTypeID",
                        column: x => x.CalculatedTypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TargetType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DeliveryMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ParentID = table.Column<int>(type: "int", nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK_Notifications_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParentChildren",
                columns: table => new
                {
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentChildren", x => new { x.ParentID, x.ChildID });
                    table.ForeignKey(
                        name: "FK_ParentChildren_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParentChildren_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SelectedAnswerIndex = table.Column<int>(type: "int", nullable: false),
                    ScoreValue = table.Column<int>(type: "int", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    StoryID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizLogs", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_QuizLogs_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizLogs_PersonalityTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizLogs_StoryQuizTemplates_StoryID",
                        column: x => x.StoryID,
                        principalTable: "StoryQuizTemplates",
                        principalColumn: "StoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavingsGoals",
                columns: table => new
                {
                    GoalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TargetAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsChallenge = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    RewardValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavingsGoals", x => x.GoalID);
                    table.ForeignKey(
                        name: "FK_SavingsGoals_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavingsGoals_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advice_Child_IsRead",
                table: "Advices",
                columns: new[] { "ChildID", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_Allowance_Child_SetDate",
                table: "Allowances",
                columns: new[] { "ChildID", "SetDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Allowances_ParentID",
                table: "Allowances",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_ChildAchievements_AchievementTypeID",
                table: "ChildAchievements",
                column: "AchievementTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_Child_Trigger",
                table: "ChildCharacterStats",
                columns: new[] { "ChildID", "TriggerEvent" });

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID");

            migrationBuilder.CreateIndex(
                name: "IX_Child_LoginCode_Unique",
                table: "Children",
                column: "LoginCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Children_TypeID",
                table: "Children",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_Child_LogDate",
                table: "Expenses",
                columns: new[] { "ChildID", "LogDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Expense_LogDate",
                table: "Expenses",
                column: "LogDate");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryID",
                table: "Expenses",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_MoodID",
                table: "Expenses",
                column: "MoodID");

            migrationBuilder.CreateIndex(
                name: "IX_InitialProfiling_ChildID_Unique",
                table: "InitialProfilingQuestionnaires",
                column: "ChildID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InitialProfilingQuestionnaires_CalculatedTypeID",
                table: "InitialProfilingQuestionnaires",
                column: "CalculatedTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Child_IsRead",
                table: "Notifications",
                columns: new[] { "ChildID", "IsRead" },
                filter: "[ChildID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_Parent_IsRead",
                table: "Notifications",
                columns: new[] { "ParentID", "IsRead" },
                filter: "[ParentID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ParentChildren_ChildID",
                table: "ParentChildren",
                column: "ChildID");

            migrationBuilder.CreateIndex(
                name: "IX_Parent_Email_Unique",
                table: "Parents",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizLog_Child_CompletedDate",
                table: "QuizLogs",
                columns: new[] { "ChildID", "CompletedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_StoryID",
                table: "QuizLogs",
                column: "StoryID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_TypeID",
                table: "QuizLogs",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoal_Child_Status",
                table: "SavingsGoals",
                columns: new[] { "ChildID", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SavingsGoals_ParentID",
                table: "SavingsGoals",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_StoryQuiz_AgeRange",
                table: "StoryQuizTemplates",
                columns: new[] { "TargetAgeMin", "TargetAgeMax" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advices");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.DropTable(
                name: "ChildAchievements");

            migrationBuilder.DropTable(
                name: "ChildCharacterStats");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "InitialProfilingQuestionnaires");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ParentChildren");

            migrationBuilder.DropTable(
                name: "QuizLogs");

            migrationBuilder.DropTable(
                name: "SavingsGoals");

            migrationBuilder.DropTable(
                name: "AchievementTypes");

            migrationBuilder.DropTable(
                name: "CharacterStats");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Moods");

            migrationBuilder.DropTable(
                name: "StoryQuizTemplates");

            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "PersonalityTypes");
        }
    }
}
