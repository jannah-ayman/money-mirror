using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCharacterModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CharacterStats");

            migrationBuilder.AddColumn<int>(
                name: "CharacterID",
                table: "Children",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DisplayedAt",
                table: "ChildCharacterStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DurationSeconds",
                table: "ChildCharacterStats",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WasInteracted",
                table: "ChildCharacterStats",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CharacterID",
                table: "CharacterStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CharacterStats",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "CharacterStats",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "CharacterStats",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UsageContext",
                table: "CharacterStats",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CharacterType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BasePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Children_CharacterID",
                table: "Children",
                column: "CharacterID");

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_ChildID_DisplayedAt",
                table: "ChildCharacterStats",
                columns: new[] { "ChildID", "DisplayedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_TriggerEvent",
                table: "ChildCharacterStats",
                column: "TriggerEvent");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStats_CharacterID_State",
                table: "CharacterStats",
                columns: new[] { "CharacterID", "State" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterType",
                table: "Characters",
                column: "CharacterType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_DisplayOrder",
                table: "Characters",
                column: "DisplayOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_CharacterStats_Characters_CharacterID",
                table: "CharacterStats",
                column: "CharacterID",
                principalTable: "Characters",
                principalColumn: "CharacterID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID",
                principalTable: "CharacterStats",
                principalColumn: "StatsID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Children_Characters_CharacterID",
                table: "Children",
                column: "CharacterID",
                principalTable: "Characters",
                principalColumn: "CharacterID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CharacterStats_Characters_CharacterID",
                table: "CharacterStats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats");

            migrationBuilder.DropForeignKey(
                name: "FK_Children_Characters_CharacterID",
                table: "Children");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Children_CharacterID",
                table: "Children");

            migrationBuilder.DropIndex(
                name: "IX_ChildCharacterStats_ChildID_DisplayedAt",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_ChildCharacterStats_TriggerEvent",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStats_CharacterID_State",
                table: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "CharacterID",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "DisplayedAt",
                table: "ChildCharacterStats");

            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "ChildCharacterStats");

            migrationBuilder.DropColumn(
                name: "WasInteracted",
                table: "ChildCharacterStats");

            migrationBuilder.DropColumn(
                name: "CharacterID",
                table: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "State",
                table: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "UsageContext",
                table: "CharacterStats");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CharacterStats",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID",
                principalTable: "CharacterStats",
                principalColumn: "StatsID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
