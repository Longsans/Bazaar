using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transport.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScheduledAt",
                table: "InventoryPickups",
                newName: "TimeScheduledAt");

            migrationBuilder.RenameColumn(
                name: "ScheduledAtDate",
                table: "Deliveries",
                newName: "TimeScheduledAt");

            migrationBuilder.RenameColumn(
                name: "ExpectedDeliveryDate",
                table: "Deliveries",
                newName: "EstimatedDeliveryTime");

            migrationBuilder.CreateTable(
                name: "InventoryReturns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimatedDeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CancelReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryOwnerId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryReturns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturnQuantities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Units = table.Column<long>(type: "bigint", nullable: false),
                    ReturnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnQuantities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnQuantities_InventoryReturns_ReturnId",
                        column: x => x.ReturnId,
                        principalTable: "InventoryReturns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnQuantities_ReturnId",
                table: "ReturnQuantities",
                column: "ReturnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReturnQuantities");

            migrationBuilder.DropTable(
                name: "InventoryReturns");

            migrationBuilder.RenameColumn(
                name: "TimeScheduledAt",
                table: "InventoryPickups",
                newName: "ScheduledAt");

            migrationBuilder.RenameColumn(
                name: "TimeScheduledAt",
                table: "Deliveries",
                newName: "ScheduledAtDate");

            migrationBuilder.RenameColumn(
                name: "EstimatedDeliveryTime",
                table: "Deliveries",
                newName: "ExpectedDeliveryDate");
        }
    }
}
