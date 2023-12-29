using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductInventoryToPickupProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "Units",
                table: "ReturnQuantities",
                newName: "Quantity");

            migrationBuilder.CreateTable(
                name: "PickupProductStocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfUnits = table.Column<long>(type: "bigint", nullable: false),
                    PickupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupProductStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupProductStocks_InventoryPickups_PickupId",
                        column: x => x.PickupId,
                        principalTable: "InventoryPickups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupProductStocks_PickupId",
                table: "PickupProductStocks",
                column: "PickupId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupProductStocks");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ReturnQuantities",
                newName: "Units");

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupId = table.Column<int>(type: "int", nullable: false),
                    NumberOfUnits = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_InventoryPickups_PickupId",
                        column: x => x.PickupId,
                        principalTable: "InventoryPickups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_PickupId",
                table: "InventoryItems",
                column: "PickupId");
        }
    }
}
