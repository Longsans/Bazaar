using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bazaar.Contracting.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameEmailAddressProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Clients",
                newName: "EmailAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                newName: "IX_Clients_EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Clients",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Clients_EmailAddress",
                table: "Clients",
                newName: "IX_Clients_Email");
        }
    }
}
