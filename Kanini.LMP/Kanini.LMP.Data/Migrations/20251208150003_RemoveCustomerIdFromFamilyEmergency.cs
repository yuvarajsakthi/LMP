using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomerIdFromFamilyEmergency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyEmergencyDetails_Customers_CustomerId",
                table: "FamilyEmergencyDetails");

            migrationBuilder.DropIndex(
                name: "IX_FamilyEmergencyDetails_CustomerId",
                table: "FamilyEmergencyDetails");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "FamilyEmergencyDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "FamilyEmergencyDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FamilyEmergencyDetails_CustomerId",
                table: "FamilyEmergencyDetails",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyEmergencyDetails_Customers_CustomerId",
                table: "FamilyEmergencyDetails",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
