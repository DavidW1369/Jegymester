using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Jegymester.Migrations
{
    /// <inheritdoc />
    public partial class initcorr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancellable",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancellable",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
