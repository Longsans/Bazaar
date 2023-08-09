using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_ExternalId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                computedColumnSql: "CONCAT('ORDR-', [Id])",
                stored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ExternalId",
                table: "Orders",
                column: "ExternalId",
                unique: true);
        }
    }
}
