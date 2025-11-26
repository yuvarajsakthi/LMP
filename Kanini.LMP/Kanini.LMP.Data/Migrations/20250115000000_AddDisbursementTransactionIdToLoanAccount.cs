using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisbursementTransactionIdToLoanAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisbursementTransactionId",
                table: "LoanAccounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisbursementTransactionId",
                table: "LoanAccounts");
        }
    }
}