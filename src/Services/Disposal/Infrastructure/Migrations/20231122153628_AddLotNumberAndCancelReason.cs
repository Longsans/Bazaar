using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disposal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLotNumberAndCancelReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DisposeQuantities_ProductStockQuantityId_DisposalOrderId",
                table: "DisposeQuantities");

            migrationBuilder.DropColumn(
                name: "ProductStockQuantityId",
                table: "DisposeQuantities");

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "DisposeQuantities",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancelReason",
                table: "DisposalOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisposeQuantities_LotNumber_DisposalOrderId",
                table: "DisposeQuantities",
                columns: new[] { "LotNumber", "DisposalOrderId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DisposeQuantities_LotNumber_DisposalOrderId",
                table: "DisposeQuantities");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "DisposeQuantities");

            migrationBuilder.DropColumn(
                name: "CancelReason",
                table: "DisposalOrders");

            migrationBuilder.AddColumn<int>(
                name: "ProductStockQuantityId",
                table: "DisposeQuantities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DisposeQuantities_ProductStockQuantityId_DisposalOrderId",
                table: "DisposeQuantities",
                columns: new[] { "ProductStockQuantityId", "DisposalOrderId" },
                unique: true);
        }
    }
}
