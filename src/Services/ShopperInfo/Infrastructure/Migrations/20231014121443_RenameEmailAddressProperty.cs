using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopperInfo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmailAddressProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Shoppers",
                newName: "EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Shoppers",
                newName: "Email");
        }
    }
}
