using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Basket.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "BuyerBaskets",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Total",
                table: "BuyerBaskets");
        }
    }
}
