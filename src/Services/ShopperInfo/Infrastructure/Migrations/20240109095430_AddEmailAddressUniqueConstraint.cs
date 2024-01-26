using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopperInfo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAddressUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "Shoppers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Shoppers_EmailAddress",
                table: "Shoppers",
                column: "EmailAddress",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Shoppers_EmailAddress",
                table: "Shoppers");

            migrationBuilder.AlterColumn<string>(
                name: "EmailAddress",
                table: "Shoppers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
