using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFieldNamesAndTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CatalogItems",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CatalogItems",
                newName: "ProductDescription");

            migrationBuilder.AlterColumn<long>(
                name: "RestockThreshold",
                table: "CatalogItems",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "MaxStockThreshold",
                table: "CatalogItems",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "AvailableStock",
                table: "CatalogItems",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "CatalogItems",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductDescription",
                table: "CatalogItems",
                newName: "Description");

            migrationBuilder.AlterColumn<int>(
                name: "RestockThreshold",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "MaxStockThreshold",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "AvailableStock",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
