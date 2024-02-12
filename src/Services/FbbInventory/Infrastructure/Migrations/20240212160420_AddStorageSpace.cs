using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "TotalStorageSpaceUsedM3",
                table: "SellerInventories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StorageHeightPerUnitCm",
                table: "ProductInventories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StorageLengthPerUnitCm",
                table: "ProductInventories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StorageWidthPerUnitCm",
                table: "ProductInventories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "TotalStorageSpaceCm3",
                table: "ProductInventories",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "StorageSpaceUsedCm3",
                table: "Lots",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalStorageSpaceUsedM3",
                table: "SellerInventories");

            migrationBuilder.DropColumn(
                name: "StorageHeightPerUnitCm",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "StorageLengthPerUnitCm",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "StorageWidthPerUnitCm",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "TotalStorageSpaceCm3",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "StorageSpaceUsedCm3",
                table: "Lots");
        }
    }
}
