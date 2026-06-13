using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonalityScoresToChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BargainHunterScore",
                table: "Children",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GoalOrientedPlannerScore",
                table: "Children",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ImpulsiveSpenderScore",
                table: "Children",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPersonalityUpdateDate",
                table: "Children",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrudentSaverScore",
                table: "Children",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BargainHunterScore",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "GoalOrientedPlannerScore",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "ImpulsiveSpenderScore",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "LastPersonalityUpdateDate",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "PrudentSaverScore",
                table: "Children");
        }
    }
}
