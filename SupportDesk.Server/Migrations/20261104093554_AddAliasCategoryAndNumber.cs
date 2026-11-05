using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAliasCategoryAndNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TicketAliases_TicketId",
                table: "TicketAliases");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "TicketAliases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "TicketAliases",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql(@"
        UPDATE a
        SET a.CategoryId = c.Id,
            a.Number = CAST(SUBSTRING(a.TicketNumber, LEN(c.Prefix) + 2, 20) AS int)
        FROM TicketAliases a
        JOIN Categories c ON a.TicketNumber LIKE c.Prefix + '-%';
    ");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAliases_CategoryId",
                table: "TicketAliases",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAliases_TicketId_CategoryId",
                table: "TicketAliases",
                columns: new[] { "TicketId", "CategoryId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketAliases_Categories_CategoryId",
                table: "TicketAliases",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketAliases_Categories_CategoryId",
                table: "TicketAliases");

            migrationBuilder.DropIndex(
                name: "IX_TicketAliases_CategoryId",
                table: "TicketAliases");

            migrationBuilder.DropIndex(
                name: "IX_TicketAliases_TicketId_CategoryId",
                table: "TicketAliases");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "TicketAliases");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "TicketAliases");

            migrationBuilder.CreateIndex(
                name: "IX_TicketAliases_TicketId",
                table: "TicketAliases",
                column: "TicketId");
        }
    }
}
