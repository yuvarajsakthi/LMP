using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocumentStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentUploads");

            migrationBuilder.AddColumn<byte[]>(
                name: "DocumentData",
                table: "LoanApplicationBases",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "DocumentName",
                table: "LoanApplicationBases",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentData",
                table: "LoanApplicationBases");

            migrationBuilder.DropColumn(
                name: "DocumentName",
                table: "LoanApplicationBases");

            migrationBuilder.CreateTable(
                name: "DocumentUploads",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    DocumentData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUploads", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads",
                column: "LoanApplicationBaseId");
        }
    }
}
