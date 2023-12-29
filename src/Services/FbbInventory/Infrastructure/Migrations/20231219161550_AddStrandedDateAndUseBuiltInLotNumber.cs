using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStrandedDateAndUseBuiltInLotNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId_TimeEnteredStorage_TimeUnfulfillableSince_UnfulfillableCategory",
                table: "Lots");

            migrationBuilder.DropCheckConstraint(
                name: "CK_TimeUnfulfillableSince_After_TimeEnteredStorage",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "TotalUnits",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "UnitsPendingRemoval",
                table: "Lots",
                newName: "UnitsInRemoval");

            migrationBuilder.RenameColumn(
                name: "TimeUnfulfillableSince",
                table: "Lots",
                newName: "DateUnitsBecameUnfulfillable");

            migrationBuilder.RenameColumn(
                name: "TimeEnteredStorage",
                table: "Lots",
                newName: "DateUnitsEnteredStorage");

            migrationBuilder.AddColumn<bool>(
                name: "IsStranded",
                table: "ProductInventories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUnitsBecameStranded",
                table: "Lots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "Lots");

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "Lots",
                type: "nvarchar(450)",
                nullable: false,
                computedColumnSql: "CONCAT('LOT-', [Id])");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_LotNumber",
                table: "Lots",
                column: "LotNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductInventoryId_DateUnitsEnteredStorage_DateUnitsBecameStranded_DateUnitsBecameUnfulfillable",
                table: "Lots",
                columns: new[] { "ProductInventoryId", "DateUnitsEnteredStorage", "DateUnitsBecameStranded", "DateUnitsBecameUnfulfillable" },
                unique: true,
                filter: "[DateUnitsBecameStranded] IS NOT NULL AND [DateUnitsBecameUnfulfillable] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lots_LotNumber",
                table: "Lots");

            migrationBuilder.DropIndex(
                name: "IX_Lots_ProductInventoryId_DateUnitsEnteredStorage_DateUnitsBecameStranded_DateUnitsBecameUnfulfillable",
                table: "Lots");

            migrationBuilder.DropColumn(
                name: "IsStranded",
                table: "ProductInventories");

            migrationBuilder.DropColumn(
                name: "DateUnitsBecameStranded",
                table: "Lots");

            migrationBuilder.RenameColumn(
                name: "UnitsInRemoval",
                table: "Lots",
                newName: "UnitsPendingRemoval");

            migrationBuilder.RenameColumn(
                name: "DateUnitsEnteredStorage",
                table: "Lots",
                newName: "TimeEnteredStorage");

            migrationBuilder.RenameColumn(
                name: "DateUnitsBecameUnfulfillable",
                table: "Lots",
                newName: "TimeUnfulfillableSince");

            migrationBuilder.AddColumn<long>(
                name: "TotalUnits",
                table: "Lots",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "LotNumber",
                table: "Lots",
                type: "nvarchar(max)",
                nullable: false,
                computedColumnSql: "CONCAT('LOT-', [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComputedColumnSql: "CONCAT('LOT-', [Id])",
                oldStored: null);

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
        }
    }
}
