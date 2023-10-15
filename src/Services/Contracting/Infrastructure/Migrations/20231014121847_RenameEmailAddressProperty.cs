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
                table: "Partners",
                newName: "EmailAddress");

            migrationBuilder.RenameIndex(
                name: "IX_Partners_Email",
                table: "Partners",
                newName: "IX_Partners_EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Partners",
                newName: "Email");

            migrationBuilder.RenameIndex(
                name: "IX_Partners_EmailAddress",
                table: "Partners",
                newName: "IX_Partners_Email");
        }
    }
}
