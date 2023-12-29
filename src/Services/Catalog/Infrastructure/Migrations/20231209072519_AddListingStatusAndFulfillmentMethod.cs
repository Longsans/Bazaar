using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingStatusAndFulfillmentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "IsFulfilledByBazaar",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "IsOfficiallyListed",
                table: "CatalogItems");

            migrationBuilder.AddColumn<int>(
                name: "FulfillmentMethod",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ListingStatus",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FulfillmentMethod",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "ListingStatus",
                table: "CatalogItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
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
    }
}
