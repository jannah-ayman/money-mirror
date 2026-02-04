using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_ChildCharacterStats_ChildID_DisplayedAt",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_ChildCharacterStats_TriggerEvent",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStats_CharacterID_State",
                table: "CharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_Characters_DisplayOrder",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "SelectedCharacter",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Characters");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_CharacterType",
                table: "Characters",
                newName: "IX_Character_CharacterType_Unique");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Characters",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "CharacterID", "BasePath", "CharacterType", "Description", "DisplayName", "DisplayOrder", "IsActive" },
                values: new object[,]
                {
                    { 1, "/characters/nova", "Nova", "Energetic and loves adventures! Nova is always excited to help you reach your goals! 🚀", "Nova the Explorer", 1, true },
                    { 2, "/characters/luna", "Luna", "Calm and thoughtful! Luna helps you make smart decisions about your money. 🌙", "Luna the Thinker", 2, true },
                    { 3, "/characters/cosmo", "Cosmo", "Curious and playful! Cosmo loves learning new things about saving and spending! ⭐", "Cosmo the Curious", 3, true },
                    { 4, "/characters/aura", "Aura", "Wise and encouraging! Aura believes in you and celebrates every achievement! ✨", "Aura the Wise", 4, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterStats_CharacterID",
                table: "CharacterStats",
                column: "CharacterID");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID",
                principalTable: "CharacterStats",
                principalColumn: "StatsID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats");

            migrationBuilder.DropIndex(
                name: "IX_CharacterStats_CharacterID",
                table: "CharacterStats");

            migrationBuilder.DeleteData(
                table: "Characters",
                keyColumn: "CharacterID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Characters",
                keyColumn: "CharacterID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Characters",
                keyColumn: "CharacterID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Characters",
                keyColumn: "CharacterID",
                keyValue: 4);

            migrationBuilder.RenameIndex(
                name: "IX_Character_CharacterType_Unique",
                table: "Characters",
                newName: "IX_Characters_CharacterType");

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                table: "Children",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SelectedCharacter",
                table: "Children",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Characters",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Characters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "IX_Characters_DisplayOrder",
                table: "Characters",
                column: "DisplayOrder");

            migrationBuilder.AddForeignKey(
                name: "FK_ChildCharacterStats_CharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID",
                principalTable: "CharacterStats",
                principalColumn: "StatsID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
