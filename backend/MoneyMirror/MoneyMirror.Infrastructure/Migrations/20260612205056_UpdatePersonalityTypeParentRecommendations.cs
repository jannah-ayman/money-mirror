using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonalityTypeParentRecommendations : Migration
    {
        /// <inheritdoc />
        /// <inheritdoc />
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Tell SQL Server to stop enforcing foreign key checks on tables referencing PersonalityTypes
            migrationBuilder.Sql("ALTER TABLE [InitialProfilingQuestionnaires] NOCHECK CONSTRAINT FK_InitialProfilingQuestionnaires_PersonalityTypes_CalculatedTypeID;");
            migrationBuilder.Sql("ALTER TABLE [Children] NOCHECK CONSTRAINT FK_Children_PersonalityTypes_TypeID;");
            migrationBuilder.Sql("ALTER TABLE [QuizAnswers] NOCHECK CONSTRAINT FK_QuizAnswers_PersonalityTypes_PersonalityTypeID;");

            // 2. Clear out the conflicting seed rows safely
            migrationBuilder.Sql("DELETE FROM [PersonalityTypes] WHERE [TypeID] IN (1, 2, 3, 4, 5);");

            // 3. Your existing EF core insert block runs here smoothly
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

            // 4. Turn foreign key constraint checks back on immediately
            migrationBuilder.Sql("ALTER TABLE [InitialProfilingQuestionnaires] CHECK CONSTRAINT FK_InitialProfilingQuestionnaires_PersonalityTypes_CalculatedTypeID;");
            migrationBuilder.Sql("ALTER TABLE [Children] CHECK CONSTRAINT FK_Children_PersonalityTypes_TypeID;");
            migrationBuilder.Sql("ALTER TABLE [QuizAnswers] CHECK CONSTRAINT FK_QuizAnswers_PersonalityTypes_PersonalityTypeID;");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PersonalityTypes",
                keyColumn: "TypeID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PersonalityTypes",
                keyColumn: "TypeID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PersonalityTypes",
                keyColumn: "TypeID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PersonalityTypes",
                keyColumn: "TypeID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PersonalityTypes",
                keyColumn: "TypeID",
                keyValue: 5);
        }
    }
}
