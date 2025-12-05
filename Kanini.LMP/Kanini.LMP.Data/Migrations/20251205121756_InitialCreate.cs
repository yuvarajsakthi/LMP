using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kanini.LMP.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faqs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faqs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoanProducts",
                columns: table => new
                {
                    LoanProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanType = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanProducts", x => x.LoanProductId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnnualIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EligibilityScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ProfileImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HomeOwnershipStatus = table.Column<int>(type: "int", nullable: true),
                    AadhaarNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                    table.ForeignKey(
                        name: "FK_Customers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    NotificationMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "LoanApplicationBases",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    LoanProductType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmissionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplicationBases", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_LoanApplicationBases_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AddressInformations",
                columns: table => new
                {
                    AddressInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    PresentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "varchar(50)", nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MobileNumber1 = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    MobileNumber2 = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AddressInformations", x => x.AddressInformationId);
                    table.ForeignKey(
                        name: "FK_AddressInformations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Declarations",
                columns: table => new
                {
                    DeclarationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Declarations", x => x.DeclarationId);
                    table.ForeignKey(
                        name: "FK_Declarations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentUploads",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentUploads", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentUploads_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EMIPlans",
                columns: table => new
                {
                    EMIId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    PrincipleAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TermMonths = table.Column<int>(type: "int", nullable: false),
                    RateOfInterest = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MonthlyEMI = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalInterestPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRepaymentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMIPlans", x => x.EMIId);
                    table.ForeignKey(
                        name: "FK_EMIPlans_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EMIPlans_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FamilyEmergencyDetails",
                columns: table => new
                {
                    FamilyEmergencyDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RelationshipWithApplicant = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyEmergencyDetails", x => x.FamilyEmergencyDetailsId);
                    table.ForeignKey(
                        name: "FK_FamilyEmergencyDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HomeLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    OwnershipType = table.Column<int>(type: "int", nullable: false),
                    PropertyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanPurpose = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_HomeLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoanDetails",
                columns: table => new
                {
                    LoanDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    LoanApplicationId = table.Column<int>(type: "int", nullable: false),
                    RequestedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    AppliedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MonthlyInstallment = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanDetails", x => x.LoanDetailsId);
                    table.ForeignKey(
                        name: "FK_LoanDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalDetails",
                columns: table => new
                {
                    PersonalDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DistrictOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PANNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EducationQualification = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    ResidentialStatus = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SignatureImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IDProofImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.PersonalDetailsId);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonalDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkExperienceYears = table.Column<int>(type: "int", nullable: false),
                    LoanPurpose = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_PersonalLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManufacturingYear = table.Column<int>(type: "int", nullable: false),
                    OnRoadPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LoanPurposeVehicle = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLoanApplications", x => x.LoanApplicationBaseId);
                    table.ForeignKey(
                        name: "FK_VehicleLoanApplications_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "LoanProducts",
                columns: new[] { "LoanProductId", "IsActive", "LoanType" },
                values: new object[,]
                {
                    { 1, true, 1 },
                    { 2, true, 2 },
                    { 3, true, 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "CreatedAt", "Email", "FullName", "PasswordHash", "Roles", "Status", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager@gmail.com", "Manager One", "$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyYzOxJ6.Esi", 1, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_AddressInformations_LoanApplicationBaseId",
                table: "AddressInformations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_UserId",
                table: "Customers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Declarations_LoanApplicationBaseId",
                table: "Declarations",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_CustomerId",
                table: "DocumentUploads",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EMIPlans_CustomerId",
                table: "EMIPlans",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EMIPlans_LoanApplicationBaseId",
                table: "EMIPlans",
                column: "LoanApplicationBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_FamilyEmergencyDetails_LoanApplicationBaseId",
                table: "FamilyEmergencyDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicationBases_CustomerId",
                table: "LoanApplicationBases",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanDetails_LoanApplicationBaseId",
                table: "LoanDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoanProducts_LoanType",
                table: "LoanProducts",
                column: "LoanType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId1",
                table: "Notifications",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_CustomerId",
                table: "PersonalDetails",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalDetails_LoanApplicationBaseId",
                table: "PersonalDetails",
                column: "LoanApplicationBaseId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AddressInformations");

            migrationBuilder.DropTable(
                name: "Declarations");

            migrationBuilder.DropTable(
                name: "DocumentUploads");

            migrationBuilder.DropTable(
                name: "EMIPlans");

            migrationBuilder.DropTable(
                name: "FamilyEmergencyDetails");

            migrationBuilder.DropTable(
                name: "Faqs");

            migrationBuilder.DropTable(
                name: "HomeLoanApplications");

            migrationBuilder.DropTable(
                name: "LoanDetails");

            migrationBuilder.DropTable(
                name: "LoanProducts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PersonalDetails");

            migrationBuilder.DropTable(
                name: "PersonalLoanApplications");

            migrationBuilder.DropTable(
                name: "VehicleLoanApplications");

            migrationBuilder.DropTable(
                name: "LoanApplicationBases");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
