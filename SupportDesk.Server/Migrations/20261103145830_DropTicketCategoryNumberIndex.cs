using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Server.Migrations
{
    /// <inheritdoc />
    public partial class DropTicketCategoryNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_CategoryId_Number",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_CategoryId",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CategoryId_Number",
                table: "Tickets",
                columns: new[] { "CategoryId", "Number" },
                unique: true);
        }
    }
}
