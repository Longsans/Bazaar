using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockQuantityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisposalOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "UnfulfillableSince",
                table: "ProductInventories");

            migrationBuilder.RenameColumn(
                name: "UnitsInStock",
                table: "ProductInventories",
                newName: "UnfulfillableUnitsPendingRemoval");

            migrationBuilder.AddColumn<long>(
                name: "FulfillableUnitsInStock",
                table: "ProductInventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FulfillableUnitsPendingRemoval",
                table: "ProductInventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TotalUnits",
                table: "ProductInventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "UnfulfillableUnitsInStock",
                table: "ProductInventories",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "FulfillableProductQuantities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEnteredStorage = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitsInStock = table.Column<long>(type: "bigint", nullable: false),
                    UnitsPendingRemoval = table.Column<long>(type: "bigint", nullable: false),
                    TotalUnits = table.Column<long>(type: "bigint", nullable: false),
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FulfillableProductQuantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FulfillableProductQuantities_ProductInventories_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnfulfillableProductQuantities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnfulfillableCategory = table.Column<int>(type: "int", nullable: false),
                    DateUnfulfillableSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitsInStock = table.Column<long>(type: "bigint", nullable: false),
                    UnitsPendingRemoval = table.Column<long>(type: "bigint", nullable: false),
                    TotalUnits = table.Column<long>(type: "bigint", nullable: false),
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnfulfillableProductQuantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnfulfillableProductQuantities_ProductInventories_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FulfillableProductQuantities_ProductInventoryId_DateEnteredStorage",
                table: "FulfillableProductQuantities",
                columns: new[] { "ProductInventoryId", "DateEnteredStorage" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnfulfillableProductQuantities_ProductInventoryId_DateUnfulfillableSince_UnfulfillableCategory",
                table: "UnfulfillableProductQuantities",
                columns: new[] { "ProductInventoryId", "DateUnfulfillableSince", "UnfulfillableCategory" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FulfillableProductQuantities");

            migrationBuilder.DropTable(
                name: "UnfulfillableProductQuantities");

            migrationBuilder.DropColumn(
                name: "FulfillableUnitsInStock",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "FulfillableUnitsPendingRemoval",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "TotalUnits",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "UnfulfillableUnitsInStock",
                table: "ProductInventories");

            migrationBuilder.RenameColumn(
                name: "UnfulfillableUnitsPendingRemoval",
                table: "ProductInventories",
                newName: "UnitsInStock");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductInventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UnfulfillableSince",
                table: "ProductInventories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DisposalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductInventoryId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UnitsToDispose = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisposalOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisposalOrders_ProductInventories_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisposalOrders_ProductInventoryId",
                table: "DisposalOrders",
                column: "ProductInventoryId");
        }
    }
}
