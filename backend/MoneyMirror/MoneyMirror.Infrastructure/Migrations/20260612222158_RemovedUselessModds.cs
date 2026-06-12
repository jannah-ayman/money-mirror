using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUselessModds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Moods",
                keyColumn: "MoodID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Moods",
                keyColumn: "MoodID",
                keyValue: 7);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Moods",
                columns: new[] { "MoodID", "Description" },
                values: new object[,]
                {
                    { 6, "Cool" },
                    { 7, "Thoughtful" }
                });
        }
    }
}
