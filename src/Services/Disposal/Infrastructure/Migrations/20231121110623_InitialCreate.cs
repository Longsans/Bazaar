using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Disposal.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DisposalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByBazaar = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposalOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DisposeQuantities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductStockQuantityId = table.Column<int>(type: "int", nullable: false),
                    UnitsToDispose = table.Column<long>(type: "bigint", nullable: false),
                    InventoryOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisposalOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposeQuantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisposeQuantities_DisposalOrders_DisposalOrderId",
                        column: x => x.DisposalOrderId,
                        principalTable: "DisposalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisposeQuantities_DisposalOrderId",
                table: "DisposeQuantities",
                column: "DisposalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DisposeQuantities_ProductStockQuantityId_DisposalOrderId",
                table: "DisposeQuantities",
                columns: new[] { "ProductStockQuantityId", "DisposalOrderId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisposeQuantities");

            migrationBuilder.DropTable(
                name: "DisposalOrders");
        }
    }
}
