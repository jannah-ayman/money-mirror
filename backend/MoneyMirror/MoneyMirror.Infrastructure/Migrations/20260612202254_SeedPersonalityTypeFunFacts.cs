using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPersonalityTypeFunFacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PersonalityTypes",
                columns: new[] { "TypeID", "ChildName", "Desc", "FunFacts", "ParentName", "StaticRecommendation", "Traits" },
                values: new object[,]
                {
                    { 1, "Money Explorer", "We're still learning about your money personality! Keep logging expenses and taking quizzes.", "Every money expert started somewhere — you're just getting started!", "Pending Analysis", "[]", "[]" },
                    { 2, "Speedy Spender", "Quick purchases driven by excitement, low savings ratios.", "Did you know? Speedy Spenders are super fun and spontaneous — the trick is to pause for just one day before buying!", "Impulsive Spender", "[\"Wait 24 hours before buying\",\"Set a weekly spending limit\",\"Try a savings jar\"]", "[\"Buys quickly\",\"Gets excited about new things\",\"Struggles to save\"]" },
                    { 3, "Treasure Keeper", "High savings ratios and deliberate spending decisions.", "Did you know? Treasure Keepers are rare — only the wisest kids know how to grow their coins into something amazing!", "Prudent Saver", "[\"Set a savings goal each month\",\"Reward yourself occasionally\",\"Track your savings growth\"]", "[\"Thinks before buying\",\"Saves consistently\",\"Rarely regrets purchases\"]" },
                    { 4, "Dream Builder", "Steady goal contributions and balanced spending.", "Did you know? Dream Builders are natural achievers — every coin you save is one step closer to your dream!", "Goal-Oriented Planner", "[\"Break big goals into smaller steps\",\"Celebrate milestones\",\"Keep your goal visible\"]", "[\"Plans purchases ahead\",\"Contributes to goals regularly\",\"Balances fun and saving\"]" },
                    { 5, "Deal Detective", "Smart spending focused on value and deals.", "Did you know? Deal Detectives have a superpower — they can spot a great deal from a mile away!", "Bargain Hunter", "[\"Make a list before shopping\",\"Look for sales and discounts\",\"Avoid buying just because it's cheap\"]", "[\"Compares prices\",\"Loves a good deal\",\"Spends wisely\"]" }
                });
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
