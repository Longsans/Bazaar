using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBoolColumnsForIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxStockThreshold",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "RestockThreshold",
                table: "CatalogItems");

            migrationBuilder.AddColumn<bool>(
                name: "HasOrdersInProgress",
                table: "CatalogItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFulfilledByBazaar",
                table: "CatalogItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfficiallyListed",
                table: "CatalogItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasOrdersInProgress",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "IsFulfilledByBazaar",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "IsOfficiallyListed",
                table: "CatalogItems");

            migrationBuilder.AddColumn<long>(
                name: "MaxStockThreshold",
                table: "CatalogItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "RestockThreshold",
                table: "CatalogItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
