using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazaar.Contracting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrectEndDateAllowNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Contract_StartBeforeEndDate",
                table: "Contracts");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Contract_StartBeforeEndDate",
                table: "Contracts",
                sql: "[EndDate] IS NULL OR [StartDate] <= [EndDate]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Contract_StartBeforeEndDate",
                table: "Contracts");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Contract_StartBeforeEndDate",
                table: "Contracts",
                sql: "[StartDate] <= [EndDate]");
        }
    }
}
