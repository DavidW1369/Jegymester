using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jegymester.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdFromTO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketOrders_Users_UserId",
                table: "TicketOrders");

            migrationBuilder.DropIndex(
                name: "IX_TicketOrders_UserId",
                table: "TicketOrders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketOrders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TicketOrders",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketOrders_UserId",
                table: "TicketOrders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketOrders_Users_UserId",
                table: "TicketOrders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
