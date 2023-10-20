using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazaar.Contracting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPartnerEmailUniqueAndSellingPlanNonZeroChecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Partners",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SellingPlan_PerSaleOrMonthlyFeePositive",
                table: "SellingPlans",
                sql: "[PerSaleFee] > 0 OR [MonthlyFee] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SellingPlan_RegularFeePositive",
                table: "SellingPlans",
                sql: "[RegularPerSaleFeePercent] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_Partners_Email",
                table: "Partners",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SellingPlan_PerSaleOrMonthlyFeePositive",
                table: "SellingPlans");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SellingPlan_RegularFeePositive",
                table: "SellingPlans");

            migrationBuilder.DropIndex(
                name: "IX_Partners_Email",
                table: "Partners");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
