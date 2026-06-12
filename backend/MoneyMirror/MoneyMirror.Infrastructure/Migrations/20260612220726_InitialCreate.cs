using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

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
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Threshold = table.Column<int>(type: "int", nullable: false),
                    IconURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchievementTypes", x => x.AchievementTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DefaultImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterID);
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
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPermanentlyDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PermanentDeletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailConfirmationCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailChangeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailChangeCodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    QuestionText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TargetAgeMin = table.Column<int>(type: "int", nullable: false),
                    TargetAgeMax = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoryQuizTemplates", x => x.StoryID);
                });

            migrationBuilder.CreateTable(
                name: "CharacterStates",
                columns: table => new
                {
                    StateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScreenContext = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CharacterID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterStates", x => x.StateID);
                    table.ForeignKey(
                        name: "FK_CharacterStates_Characters_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Characters",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Children",
                columns: table => new
                {
                    ChildID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CurrentBalance = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.00m),
                    IsPersonalityFinalized = table.Column<bool>(type: "bit", nullable: false),
                    QuizCount = table.Column<int>(type: "int", nullable: false),
                    GoalCount = table.Column<int>(type: "int", nullable: false),
                    ExpenseCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TypeID = table.Column<int>(type: "int", nullable: true),
                    CharacterID = table.Column<int>(type: "int", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Children", x => x.ChildID);
                    table.ForeignKey(
                        name: "FK_Children_Characters_CharacterID",
                        column: x => x.CharacterID,
                        principalTable: "Characters",
                        principalColumn: "CharacterID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Children_PersonalityTypes_TypeID",
                        column: x => x.TypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.SetNull);
                });

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
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    DailyHour = table.Column<int>(type: "int", nullable: true),
                    WeeklyDay = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    MonthlyDay = table.Column<int>(type: "int", nullable: true),
                    LastCreditedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                name: "Expenses",
                columns: table => new
                {
                    ExpenseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
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
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChildAgeGroup = table.Column<int>(type: "int", nullable: false),
                    HasAllowance = table.Column<int>(type: "int", nullable: false),
                    SpendingPace = table.Column<int>(type: "int", nullable: false),
                    SpendingCategories = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    OutOfMoneyBehavior = table.Column<int>(type: "int", nullable: false),
                    TriesToSave = table.Column<int>(type: "int", nullable: false),
                    MoneyMindset = table.Column<int>(type: "int", nullable: false),
                    FeelingAfterSpending = table.Column<int>(type: "int", nullable: false),
                    FeelingWhenSavingGrows = table.Column<int>(type: "int", nullable: false),
                    ReactionTo100 = table.Column<int>(type: "int", nullable: false),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    CalculatedTypeID = table.Column<int>(type: "int", nullable: true)
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
                name: "SavingsGoals",
                columns: table => new
                {
                    GoalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
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

            migrationBuilder.CreateTable(
                name: "QuizLogs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    StoryID = table.Column<int>(type: "int", nullable: false),
                    AnswerID = table.Column<int>(type: "int", nullable: false),
                    PersonalityTypeTypeID = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_QuizLogs_PersonalityTypes_PersonalityTypeTypeID",
                        column: x => x.PersonalityTypeTypeID,
                        principalTable: "PersonalityTypes",
                        principalColumn: "TypeID");
                    table.ForeignKey(
                        name: "FK_QuizLogs_QuizAnswers_AnswerID",
                        column: x => x.AnswerID,
                        principalTable: "QuizAnswers",
                        principalColumn: "AnswerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizLogs_StoryQuizTemplates_StoryID",
                        column: x => x.StoryID,
                        principalTable: "StoryQuizTemplates",
                        principalColumn: "StoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BalanceAfter = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true),
                    AllowanceID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.TransactionID);
                    table.ForeignKey(
                        name: "FK_Transactions_Allowances_AllowanceID",
                        column: x => x.AllowanceID,
                        principalTable: "Allowances",
                        principalColumn: "AllowanceID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Children_ChildID",
                        column: x => x.ChildID,
                        principalTable: "Children",
                        principalColumn: "ChildID");
                    table.ForeignKey(
                        name: "FK_Transactions_Parents_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Parents",
                        principalColumn: "ParentID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AchievementTypes",
                columns: new[] { "AchievementTypeID", "Category", "Description", "IconURL", "Name", "Threshold" },
                values: new object[,]
                {
                    { 1, "Quiz", "Answered your first quiz question!", "/images/badges/first-step.png", "First Step", 1 },
                    { 2, "Quiz", "Answered 10 quiz questions.", "/images/badges/quiz-explorer.png", "Quiz Explorer", 10 },
                    { 3, "Quiz", "Answered 20 quiz questions.", "/images/badges/quiz-master.png", "Quiz Master", 20 },
                    { 4, "Quiz", "Answered 50 quiz questions!", "/images/badges/quiz-legend.png", "Quiz Legend", 50 },
                    { 5, "Goal", "Completed your first savings goal!", "/images/badges/goal-getter.png", "Goal Getter", 1 },
                    { 6, "Goal", "Completed 3 savings goals.", "/images/badges/determined.png", "Determined", 3 },
                    { 7, "Goal", "Completed 5 savings goals.", "/images/badges/achiever.png", "Achiever", 5 },
                    { 8, "Goal", "Completed 10 savings goals!", "/images/badges/champion.png", "Champion", 10 },
                    { 9, "Expense", "Logged your first expense.", "/images/badges/first-purchase.png", "First Purchase", 1 },
                    { 10, "Expense", "Logged 20 expenses.", "/images/badges/expense-tracker.png", "Expense Tracker", 20 },
                    { 11, "Expense", "Logged 40 expenses.", "/images/badges/money-logger.png", "Money Logger", 40 },
                    { 12, "Expense", "Logged 100 expenses!", "/images/badges/financial-pro.png", "Financial Pro", 100 }
                });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "CharacterID", "DefaultImageUrl", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "/images/characters/nova/profile.png", "Cool astronaut who loves street style and music", "Nova" },
                    { 2, "/images/characters/cosmo/profile.png", "Ninja superhero astronaut always ready for action", "Cosmo" },
                    { 3, "/images/characters/luna/profile.png", "Graceful ballerina astronaut in a pink skirt", "Luna" },
                    { 4, "/images/characters/stella/profile.png", "Laid back astronaut in a hoodie who loves bubblegum", "Stella" }
                });

            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "CategoryID", "Name" },
                values: new object[,]
                {
                    { 1, "Snacks / Food" },
                    { 2, "Games / Toys" },
                    { 3, "Gifts" },
                    { 4, "School Supplies" },
                    { 5, "Other" }
                });

            migrationBuilder.InsertData(
                table: "Moods",
                columns: new[] { "MoodID", "Description" },
                values: new object[,]
                {
                    { 1, "Happy" },
                    { 2, "Sad" },
                    { 3, "Neutral" },
                    { 4, "Excited" },
                    { 5, "Regretful" },
                    { 6, "Cool" },
                    { 7, "Thoughtful" }
                });

            migrationBuilder.InsertData(
                table: "PersonalityTypes",
                columns: new[] { "TypeID", "ChildName", "Desc", "FunFacts", "ParentName", "StaticRecommendation", "Traits" },
                values: new object[,]
                {
                    { 1, "Money Explorer", "We're still getting to know your child's financial personality. As they use the app and log expenses, we'll build a complete picture of their money habits and provide personalized guidance.", "Every money expert started somewhere — you're just getting started!", "Pending Analysis", "[\"Keep encouraging your child to log their expenses regularly\", \"Guide your child to try saving for a specific goal\", \"Have your child complete the story quizzes to help us better understand their money personality\"]", "[\"Discovering spending patterns\", \"Building financial profile\", \"Learning money habits\"]" },
                    { 2, "Speedy Spender", "Quick purchases driven by excitement, low savings ratios.", "Did you know? Speedy Spenders are super fun and spontaneous — the trick is to pause for just one day before buying!", "Impulsive Spender", "[\"Encourage a 24-hour waiting rule before they make non-essential purchases.\", \"Introduce a visual savings jar or progress bar so they can see their money build up.\", \"Work together to create a simple shopping list before visiting stores or online apps.\", \"Suggest setting aside a flat 20% of their allowance instantly into savings before spending any.\"]", "[\"Buys quickly\",\"Gets excited about new things\",\"Struggles to save\"]" },
                    { 3, "Treasure Keeper", "High savings ratios and deliberate spending decisions.", "Did you know? Treasure Keepers are rare — only the wisest kids know how to grow their coins into something amazing!", "Prudent Saver", "[\"Help them set exciting, long-term savings goals so they don't hold onto money out of fear.\", \"Give them permission to enjoy some 'fun spending' to avoid eventual saving burnout.\", \"Introduce basic age-appropriate concepts of investing or earning interest on accumulated funds.\"]", "[\"Thinks before buying\",\"Saves consistently\",\"Rarely regrets purchases\"]" },
                    { 4, "Dream Builder", "Balanced approach to spending and saving, with steady goal contributions. Plans purchases carefully.", "Did you know? Dream Builders are natural achievers — every coin you save is one step closer to your dream!", "Goal-Oriented Planner", "[\"Help them break down very large, daunting savings goals into smaller, reachable milestones.\", \"Celebrate or match their savings when they cross a major milestone to reward consistency.\", \"Keep their targeted goals visually prominent in conversation to sustain their natural planning habits.\"]", "[\"Creates clear savings goals\", \"Balances fun spending with saving\", \"Tracks progress regularly\", \"Plans purchases in advance\", \"Stays motivated by dreams\"]" },
                    { 5, "Deal Detective", "Emphasizes value and deals, strategic spending. Loves finding the best prices and getting good value.", "Did you know? Deal Detectives have a superpower — they can spot a great deal from a mile away!", "Bargain Hunter", "[\"Remind them to make a strict shopping list so they don't buy things simply because they are 'on sale'.\", \"Challenge them to find coupon codes or comparison shop to engage their detective strengths productively.\", \"Teach them about quality vs. price so they understand that cheap doesn't always mean high value.\"]", "[\"Compares prices before buying\", \"Loves finding good deals\", \"Waits for sales\", \"Values getting the most for money\", \"Shares deals with others\"]" }
                });

            migrationBuilder.InsertData(
                table: "CharacterStates",
                columns: new[] { "StateID", "CharacterID", "ImageUrl", "Message", "ScreenContext" },
                values: new object[,]
                {
                    { 6, 1, "/images/characters/nova/expenses.png", "Let's see what you copped this week, astronaut.", "Expenses" },
                    { 7, 2, "/images/characters/cosmo/expenses.png", "Time to investigate your spending like a true hero.", "Expenses" },
                    { 8, 3, "/images/characters/luna/expenses.png", "Every purchase tells a story. Let's review yours.", "Expenses" },
                    { 9, 4, "/images/characters/stella/expenses.png", "No stress, let's just vibe and see what you bought.", "Expenses" },
                    { 10, 1, "/images/characters/nova/goals.png", "Your bank account looking real nice right now.", "Goals" },
                    { 11, 2, "/images/characters/cosmo/goals.png", "Your savings power is charging up fast.", "Goals" },
                    { 12, 3, "/images/characters/luna/goals.png", "Your savings are dancing beautifully toward your dreams.", "Goals" },
                    { 14, 4, "/images/characters/stella/goals.png", "Pretty cool how your money's stacking up like that.", "Goals" },
                    { 15, 1, "/images/characters/nova/profile.png", "Yo, what's good? Time to check those space credits.", "Profile" },
                    { 16, 2, "/images/characters/cosmo/profile.png", "Looking strong, money warrior. Keep training.", "Profile" },
                    { 17, 3, "/images/characters/luna/profile.png", "Welcome back, little star. Shall we begin?", "Profile" },
                    { 18, 4, "/images/characters/stella/profile.png", "Hey there, space buddy. Just chilling and checking in.", "Profile" },
                    { 19, 1, "/images/characters/nova/badges.png", "Another badge? You're on fire with this.", "Badges" },
                    { 20, 2, "/images/characters/cosmo/badges.png", "Another victory unlocked. You're unstoppable.", "Badges" },
                    { 23, 3, "/images/characters/luna/badges.png", "Each achievement is like a perfect spin.", "Badges" },
                    { 29, 4, "/images/characters/stella/badges.png", "Nice, another one. You're doing your thing.", "Badges" },
                    { 32, 1, "/images/characters/nova/quiz.png", "Aight, let's test that money brain of yours.", "Quiz" },
                    { 33, 2, "/images/characters/cosmo/quiz.png", "Think fast, space cadet. Show me your skills.", "Quiz" },
                    { 34, 3, "/images/characters/luna/quiz.png", "Let's gracefully glide through these questions together.", "Quiz" },
                    { 35, 4, "/images/characters/stella/quiz.png", "Take it easy, no rush. You got this.", "Quiz" }
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
                name: "IX_Character_Name_Unique",
                table: "Characters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterState_Character_Screen_Unique",
                table: "CharacterStates",
                columns: new[] { "CharacterID", "ScreenContext" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChildAchievements_AchievementTypeID",
                table: "ChildAchievements",
                column: "AchievementTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Child_LoginCode_Unique",
                table: "Children",
                column: "LoginCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Children_CharacterID",
                table: "Children",
                column: "CharacterID");

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
                name: "IX_QuizAnswers_PersonalityTypeID",
                table: "QuizAnswers",
                column: "PersonalityTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_StoryID",
                table: "QuizAnswers",
                column: "StoryID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLog_Child_CompletedDate",
                table: "QuizLogs",
                columns: new[] { "ChildID", "CompletedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizLog_Child_Story_Unique",
                table: "QuizLogs",
                columns: new[] { "ChildID", "StoryID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_AnswerID",
                table: "QuizLogs",
                column: "AnswerID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_PersonalityTypeTypeID",
                table: "QuizLogs",
                column: "PersonalityTypeTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizLogs_StoryID",
                table: "QuizLogs",
                column: "StoryID");

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

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Child_TransactionDate",
                table: "Transactions",
                columns: new[] { "ChildID", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Type_TransactionDate",
                table: "Transactions",
                columns: new[] { "Type", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AllowanceID",
                table: "Transactions",
                column: "AllowanceID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ParentID",
                table: "Transactions",
                column: "ParentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advices");

            migrationBuilder.DropTable(
                name: "CharacterStates");

            migrationBuilder.DropTable(
                name: "ChildAchievements");

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
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AchievementTypes");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "Moods");

            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropTable(
                name: "Allowances");

            migrationBuilder.DropTable(
                name: "StoryQuizTemplates");

            migrationBuilder.DropTable(
                name: "Children");

            migrationBuilder.DropTable(
                name: "Parents");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "PersonalityTypes");
        }
    }
}
