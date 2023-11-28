using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazaar.Contracting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemodelClientSellingPlanRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts");

            migrationBuilder.AddColumn<int>(
                name: "SellingPlanId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                computedColumnSql: "CONCAT('CLNT-', [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComputedColumnSql: "CONCAT('PNER-', [Id])",
                oldStored: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_SellingPlanId",
                table: "Clients",
                column: "SellingPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_SellingPlans_SellingPlanId",
                table: "Clients",
                column: "SellingPlanId",
                principalTable: "SellingPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_SellingPlans_SellingPlanId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Clients_SellingPlanId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "SellingPlanId",
                table: "Clients");

            migrationBuilder.AlterColumn<string>(
                name: "ExternalId",
                table: "Clients",
                type: "nvarchar(450)",
                nullable: false,
                computedColumnSql: "CONCAT('PNER-', [Id])",
                stored: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComputedColumnSql: "CONCAT('CLNT-', [Id])",
                oldStored: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_Clients_ClientId",
                table: "Contracts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
