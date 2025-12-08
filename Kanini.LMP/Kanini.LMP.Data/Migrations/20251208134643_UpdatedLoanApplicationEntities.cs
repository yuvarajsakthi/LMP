using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedLoanApplicationEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentUploads_Customers_CustomerId",
                table: "DocumentUploads");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalDetails_Customers_CustomerId",
                table: "PersonalDetails");

            migrationBuilder.DropIndex(
                name: "IX_PersonalDetails_CustomerId",
                table: "PersonalDetails");

            migrationBuilder.DropIndex(
                name: "IX_DocumentUploads_CustomerId",
                table: "DocumentUploads");

            migrationBuilder.DropIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "PersonalDetails");

            migrationBuilder.DropColumn(
                name: "LoanApplicationId",
                table: "LoanDetails");

            migrationBuilder.DropColumn(
                name: "RequestedLoanAmount",
                table: "LoanApplicationBases");

            migrationBuilder.DropColumn(
                name: "TenureMonths",
                table: "LoanApplicationBases");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "DocumentUploads");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "FamilyEmergencyDetails",
                newName: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyEmergencyDetails_CustomerId",
                table: "FamilyEmergencyDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads",
                column: "LoanApplicationBaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_FamilyEmergencyDetails_Customers_CustomerId",
                table: "FamilyEmergencyDetails",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FamilyEmergencyDetails_Customers_CustomerId",
                table: "FamilyEmergencyDetails");

            migrationBuilder.DropIndex(
                name: "IX_FamilyEmergencyDetails_CustomerId",
                table: "FamilyEmergencyDetails");

            migrationBuilder.DropIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "FamilyEmergencyDetails",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "PersonalDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LoanApplicationId",
                table: "LoanDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestedLoanAmount",
                table: "LoanApplicationBases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TenureMonths",
                table: "LoanApplicationBases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "DocumentUploads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_CustomerId",
                table: "PersonalDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_CustomerId",
                table: "DocumentUploads",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentUploads_Customers_CustomerId",
                table: "DocumentUploads",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalDetails_Customers_CustomerId",
                table: "PersonalDetails",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
