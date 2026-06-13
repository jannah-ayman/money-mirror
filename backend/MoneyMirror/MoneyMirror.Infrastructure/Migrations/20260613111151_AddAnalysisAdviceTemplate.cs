using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalysisAdviceTemplate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnalysisAdviceTemplates",
                columns: table => new
                {
                    AdviceTemplateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ActionSteps = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TriggerKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisAdviceTemplates", x => x.AdviceTemplateID);
                });

            migrationBuilder.InsertData(
                table: "AnalysisAdviceTemplates",
                columns: new[] { "AdviceTemplateID", "ActionSteps", "Description", "Priority", "Title", "TriggerKey", "Type" },
                values: new object[,]
                {
                    { 1, "[\"Introduce a 24-hour waiting rule before any purchase over 20 EGP\",\"Talk to your child about how their mood affects their decisions\",\"Create a wish list to revisit purchases when they feel calmer\"]", "Your child spends a large portion of their money when in a specific emotional state, which may indicate mood-driven impulse purchases.", 1, "Emotional Spending Alert", "HIGH_MOOD_SPENDING", "Alert" },
                    { 2, "[\"Set an automatic savings rule: put aside 20% of allowance immediately\",\"Use a visible savings goal to motivate consistent saving\",\"Discuss what they are saving toward to build purpose\"]", "Your child is saving very little of their allowance, spending most of it before the next payment cycle.", 1, "Low Savings Rate", "LOW_SAVINGS_RATIO", "Alert" },
                    { 3, "[\"Help your child plan their spending at the start of each allowance cycle\",\"Set a weekly spending limit together\",\"Review purchases together at the end of each week\"]", "Your child is making purchases very frequently relative to their allowance cycle, which may lead to running out of money early.", 2, "High Spending Frequency", "HIGH_SPENDING_FREQUENCY", "Alert" },
                    { 4, "[\"Encourage your child to explore other categories like books or school supplies\",\"Set a soft limit on spending in their dominant category per cycle\",\"Talk about balancing needs vs. wants\"]", "Your child is spending the majority of their money on a single category, showing limited variety in their purchasing decisions.", 2, "Category Over-Focus", "IMPULSIVE_CATEGORY_FOCUS", "Alert" },
                    { 5, "[\"Sit together and pick one thing your child wants to save for\",\"Set a realistic target amount and timeline\",\"Check in on progress weekly to keep motivation high\"]", "Your child currently has no active savings goals, which removes a key motivation to save and plan ahead.", 2, "No Active Savings Goals", "NO_ACTIVE_GOALS", "Alert" },
                    { 6, "[\"Discuss the concept of making money last through the full cycle\",\"Try splitting the allowance into spending and saving portions immediately\",\"Review what large purchases were made and whether they were planned\"]", "Your child's balance has dropped significantly since their last allowance, suggesting rapid spending after receiving money.", 1, "Balance Draining Fast", "LOW_BALANCE_DRAIN", "Alert" },
                    { 7, "[\"Celebrate this achievement with non-monetary praise\",\"Help set a new, slightly more ambitious goal\",\"Share their success to reinforce the behavior\"]", "Your child has been completing savings goals consistently, showing strong financial discipline and follow-through.", 1, "Goal Achiever", "GOAL_STREAK", "Strength" },
                    { 8, "[\"Praise their consistency to reinforce the habit\",\"Review logged expenses together to deepen their understanding\",\"Use the data to help them spot their own patterns\"]", "Your child has been logging their expenses regularly, building a strong habit of financial awareness.", 2, "Consistent Tracker", "CONSISTENT_LOGGING", "Strength" },
                    { 9, "[\"Acknowledge this balance as a sign of developing impulse control\",\"Continue open conversations about how feelings relate to money decisions\",\"Encourage them to keep reflecting on their mood when spending\"]", "Your child's spending is distributed across different moods without any single emotional state dominating their purchases.", 3, "Emotionally Balanced Spender", "BALANCED_MOOD_SPENDING", "Strength" },
                    { 10, "[\"Highlight how close they are to their goal to keep momentum\",\"Discuss what they will do once they reach it\",\"Suggest adding a stretch goal for extra motivation\"]", "Your child has made meaningful progress toward at least one savings goal, showing patience and planning ability.", 2, "Steady Saver", "SAVING_PROGRESS", "Strength" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnalysisAdviceTemplates");
        }
    }
}
