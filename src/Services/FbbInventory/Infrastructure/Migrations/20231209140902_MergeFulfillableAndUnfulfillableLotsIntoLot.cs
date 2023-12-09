using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MergeFulfillableAndUnfulfillableLotsIntoLot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId1",
                table: "Lots");

            migrationBuilder.DropForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId2",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId_DateEnteredStorage",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId_DateUnfulfillableSince_UnfulfillableCategory",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId1",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId2",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "DateEnteredStorage",
                table: "Lots");

            migrationBuilder.AlterColumn<string>(
                name: "LotNumber",
                table: "Lots",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT('LOT-', [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "CONCAT(IIF([Fulfillability] = 'Fulfillable', 'FUFL-', 'UNFL-'), [Id])",
                oldStored: true);

            migrationBuilder.DropColumn(
                name: "Fulfillability",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "ProductInventoryId1",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "ProductInventoryId2",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "DateUnfulfillableSince",
                table: "Lots",
                newName: "TimeUnfulfillableSince");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnteredStorage",
                table: "Lots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId_TimeEnteredStorage_TimeUnfulfillableSince_UnfulfillableCategory",
                table: "Lots",
                columns: new[] { "ProductInventoryId", "TimeEnteredStorage", "TimeUnfulfillableSince", "UnfulfillableCategory" },
                unique: true,
                filter: "[TimeUnfulfillableSince] IS NOT NULL AND [UnfulfillableCategory] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_TimeUnfulfillableSince_After_TimeEnteredStorage",
                table: "Lots",
                sql: "[TimeUnfulfillableSince] IS NULL OR [TimeUnfulfillableSince] >= [TimeEnteredStorage]");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId",
                table: "Lots",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId_TimeEnteredStorage_TimeUnfulfillableSince_UnfulfillableCategory",
                table: "Lots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeUnfulfillableSince_After_TimeEnteredStorage",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "TimeEnteredStorage",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "TimeUnfulfillableSince",
                table: "Lots",
                newName: "DateUnfulfillableSince");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEnteredStorage",
                table: "Lots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fulfillability",
                table: "Lots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProductInventoryId1",
                table: "Lots",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductInventoryId2",
                table: "Lots",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LotNumber",
                table: "Lots",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT(IIF([Fulfillability] = 'Fulfillable', 'FUFL-', 'UNFL-'), [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComputedColumnSql: "CONCAT('LOT-', [Id])",
                oldStored: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId",
                table: "Lots",
                column: "ProductInventoryId",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId1",
                table: "Lots",
                column: "ProductInventoryId1",
                principalTable: "ProductInventories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lots_ProductInventories_ProductInventoryId2",
                table: "Lots",
                column: "ProductInventoryId2",
                principalTable: "ProductInventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
