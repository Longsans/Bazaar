using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Migrations
{
    /// <inheritdoc />
    public partial class CorrectProductIdDbType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "CatalogItems",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT('PROD-', [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "'PROD-' + [Id]",
                oldStored: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProductId",
                table: "CatalogItems",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "'PROD-' + [Id]",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "CONCAT('PROD-', [Id])",
                oldStored: true);
        }
    }
}
