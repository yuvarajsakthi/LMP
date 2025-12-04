using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserAndLoanApplicationFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationDocumentLinks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationDocumentLinks",
                columns: table => new
                {
                    ApplicationDocumentLinkId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    DocumentRequirementType = table.Column<int>(type: "int", nullable: false),
                    LinkedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    VerificationNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocumentLinks", x => x.ApplicationDocumentLinkId);
                    table.ForeignKey(
                        name: "FK_ApplicationDocumentLinks_DocumentUploads_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "DocumentUploads",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationDocumentLinks_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocumentLinks_DocumentId",
                table: "ApplicationDocumentLinks",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocumentLinks_LoanApplicationBaseId",
                table: "ApplicationDocumentLinks",
                column: "LoanApplicationBaseId");
        }
    }
}
