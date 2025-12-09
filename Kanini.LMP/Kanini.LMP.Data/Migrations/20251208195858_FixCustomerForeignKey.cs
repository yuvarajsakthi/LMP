using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCustomerForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanApplicationBases_Customers_CustomerRefCustomerId",
                table: "LoanApplicationBases");

            migrationBuilder.DropIndex(
                name: "IX_LoanApplicationBases_CustomerRefCustomerId",
                table: "LoanApplicationBases");

            migrationBuilder.DropColumn(
                name: "CustomerRefCustomerId",
                table: "LoanApplicationBases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerRefCustomerId",
                table: "LoanApplicationBases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationBases_CustomerRefCustomerId",
                table: "LoanApplicationBases",
                column: "CustomerRefCustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanApplicationBases_Customers_CustomerRefCustomerId",
                table: "LoanApplicationBases",
                column: "CustomerRefCustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
