using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuantitiesAndAddLots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "UnfulfillableUnitsPendingRemoval",
                table: "ProductInventories");

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false, computedColumnSql: "CONCAT(IIF([Fulfillability] = 'Fulfillable', 'FUFL-', 'UNFL-'), [Id])", stored: true),
                    UnitsInStock = table.Column<long>(type: "bigint", nullable: false),
                    UnitsPendingRemoval = table.Column<long>(type: "bigint", nullable: false),
                    TotalUnits = table.Column<long>(type: "bigint", nullable: false),
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false),
                    Fulfillability = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateEnteredStorage = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductInventoryId1 = table.Column<int>(type: "int", nullable: true),
                    UnfulfillableCategory = table.Column<int>(type: "int", nullable: true),
                    DateUnfulfillableSince = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProductInventoryId2 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_ProductInventories_ProductInventoryId",
                        column: x => x.ProductInventoryId,
                        principalTable: "ProductInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lots_ProductInventories_ProductInventoryId1",
                        column: x => x.ProductInventoryId1,
                        principalTable: "ProductInventories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lots_ProductInventories_ProductInventoryId2",
                        column: x => x.ProductInventoryId2,
                        principalTable: "ProductInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId",
                table: "Lots",
                column: "ProductInventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId_DateEnteredStorage",
                table: "Lots",
                columns: new[] { "ProductInventoryId", "DateEnteredStorage" },
                unique: true,
                filter: "[DateEnteredStorage] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId_DateUnfulfillableSince_UnfulfillableCategory",
                table: "Lots",
                columns: new[] { "ProductInventoryId", "DateUnfulfillableSince", "UnfulfillableCategory" },
                unique: true,
                filter: "[DateUnfulfillableSince] IS NOT NULL AND [UnfulfillableCategory] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId1",
                table: "Lots",
                column: "ProductInventoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId2",
                table: "Lots",
                column: "ProductInventoryId2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lots");

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

            migrationBuilder.AddColumn<long>(
                name: "UnfulfillableUnitsPendingRemoval",
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
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false),
                    DateEnteredStorage = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalUnits = table.Column<long>(type: "bigint", nullable: false),
                    UnitsInStock = table.Column<long>(type: "bigint", nullable: false),
                    UnitsPendingRemoval = table.Column<long>(type: "bigint", nullable: false)
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
                    ProductInventoryId = table.Column<int>(type: "int", nullable: false),
                    DateUnfulfillableSince = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalUnits = table.Column<long>(type: "bigint", nullable: false),
                    UnfulfillableCategory = table.Column<int>(type: "int", nullable: false),
                    UnitsInStock = table.Column<long>(type: "bigint", nullable: false),
                    UnitsPendingRemoval = table.Column<long>(type: "bigint", nullable: false)
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
    }
}
