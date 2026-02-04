using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedCharacter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildCharacterStats");

            migrationBuilder.DropTable(
                name: "CharacterStats");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "SelectedCharacter",
                table: "Children");

            migrationBuilder.AddColumn<int>(
                name: "CharacterID",
                table: "Children",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Children_CharacterID",
                table: "Children",
                column: "CharacterID");

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
                name: "FK_Children_Characters_CharacterID",
                table: "Children");

            migrationBuilder.DropTable(
                name: "CharacterStates");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Children_CharacterID",
                table: "Children");

            migrationBuilder.DropColumn(
                name: "CharacterID",
                table: "Children");

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
                name: "ChildCharacterStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildID = table.Column<int>(type: "int", nullable: false),
                    StatsID = table.Column<int>(type: "int", nullable: false),
                    StatsData = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    TriggerEvent = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_Child_Trigger",
                table: "ChildCharacterStats",
                columns: new[] { "ChildID", "TriggerEvent" });

            migrationBuilder.CreateIndex(
                name: "IX_ChildCharacterStats_StatsID",
                table: "ChildCharacterStats",
                column: "StatsID");
        }
    }
}
