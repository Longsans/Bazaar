using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStorageSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ProductHeightCm",
                table: "CatalogItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ProductLengthCm",
                table: "CatalogItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ProductWidthCm",
                table: "CatalogItems",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductHeightCm",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "ProductLengthCm",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "ProductWidthCm",
                table: "CatalogItems");
        }
    }
}
