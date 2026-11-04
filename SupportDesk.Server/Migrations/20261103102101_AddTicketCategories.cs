using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    LastNumber = table.Column<int>(type: "int", nullable: false),
                    DefaultPriority = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });
            // --- Data: default category + renumber existing tickets (GEN-1, GEN-2… by creation order) ---
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Name", "Prefix", "LastNumber", "DefaultPriority", "Color", "IsActive", "IsDefault", "CreatedAt" },
                values: new object[] { "General", "GEN", 0, "Medium", "#607d8b", true, true, DateTime.UtcNow });

            migrationBuilder.Sql(@"
                WITH ordered AS (
                    SELECT Id, ROW_NUMBER() OVER (ORDER BY CreatedAt, Id) AS rn
                    FROM Tickets
                )
                UPDATE t
                SET t.Number = o.rn,
                    t.TicketNumber = CONCAT('GEN-', o.rn)
                FROM Tickets t
                JOIN ordered o ON o.Id = t.Id;

                UPDATE Categories
                SET LastNumber = (SELECT COUNT(*) FROM Tickets)
                WHERE Prefix = 'GEN';
            ");
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId_Number",
                table: "Tickets",
                columns: new[] { "CategoryId", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketNumber",
                table: "Tickets",
                column: "TicketNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Prefix",
                table: "Categories",
                column: "Prefix",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CategoryId_Number",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_TicketNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "TicketNumber",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
