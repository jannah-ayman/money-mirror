using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoneyMirror.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTransactionCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailyHour",
                table: "Allowances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Allowances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecurring",
                table: "Allowances",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastCreditedDate",
                table: "Allowances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyDay",
                table: "Allowances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WeeklyDay",
                table: "Allowances",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

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
                name: "Transactions");

            migrationBuilder.DropColumn(
                name: "DailyHour",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "IsRecurring",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "LastCreditedDate",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "MonthlyDay",
                table: "Allowances");

            migrationBuilder.DropColumn(
                name: "WeeklyDay",
                table: "Allowances");
        }
    }
}
