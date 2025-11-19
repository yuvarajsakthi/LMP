using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class Document : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DocumentRequirementType",
                table: "ApplicationDocumentLinks",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ApplicationDocumentLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VerificationNotes",
                table: "ApplicationDocumentLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerifiedAt",
                table: "ApplicationDocumentLinks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedBy",
                table: "ApplicationDocumentLinks",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationDocumentLinks",
                keyColumns: new[] { "DocumentId", "LoanApplicationBaseId" },
                keyValues: new object[] { 1, 1 },
                columns: new[] { "DocumentRequirementType", "Status", "VerificationNotes", "VerifiedAt", "VerifiedBy" },
                values: new object[] { 6, 0, null, null, null });

            migrationBuilder.UpdateData(
                table: "ApplicationDocumentLinks",
                keyColumns: new[] { "DocumentId", "LoanApplicationBaseId" },
                keyValues: new object[] { 2, 2 },
                columns: new[] { "DocumentRequirementType", "Status", "VerificationNotes", "VerifiedAt", "VerifiedBy" },
                values: new object[] { 6, 0, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ApplicationDocumentLinks");

            migrationBuilder.DropColumn(
                name: "VerificationNotes",
                table: "ApplicationDocumentLinks");

            migrationBuilder.DropColumn(
                name: "VerifiedAt",
                table: "ApplicationDocumentLinks");

            migrationBuilder.DropColumn(
                name: "VerifiedBy",
                table: "ApplicationDocumentLinks");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentRequirementType",
                table: "ApplicationDocumentLinks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "ApplicationDocumentLinks",
                keyColumns: new[] { "DocumentId", "LoanApplicationBaseId" },
                keyValues: new object[] { 1, 1 },
                column: "DocumentRequirementType",
                value: "AddressProof-UtilityBill");

            migrationBuilder.UpdateData(
                table: "ApplicationDocumentLinks",
                keyColumns: new[] { "DocumentId", "LoanApplicationBaseId" },
                keyValues: new object[] { 2, 2 },
                column: "DocumentRequirementType",
                value: "IncomeProof-PayStub");
        }
    }
}
