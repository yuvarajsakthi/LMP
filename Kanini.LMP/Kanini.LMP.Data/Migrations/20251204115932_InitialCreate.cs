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
                name: "LoanAccounts",
                columns: table => new
                {
                    LoanAccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CurrentPaymentStatus = table.Column<int>(type: "int", nullable: false),
                    DisbursementDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaysPastDue = table.Column<int>(type: "int", nullable: false),
                    LastStatusUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaidPrincipal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPaidInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrincipalRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastPaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalLateFeePaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanAccounts", x => x.LoanAccountId);
                });

            migrationBuilder.CreateTable(
                name: "LoanOriginationWorkflows",
                columns: table => new
                {
                    WorkflowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<int>(type: "int", nullable: false),
                    StepStatus = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    ManagerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanOriginationWorkflows", x => x.WorkflowId);
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
                name: "PaymentTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EMIId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LoanAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.TransactionId);
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
                    CreditScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PresentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    PermanentAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    District = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MobileNumber1 = table.Column<int>(type: "int", nullable: false),
                    MobileNumber2 = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentData = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_DocumentUploads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
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
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<int>(type: "int", nullable: false),
                    OwnershipType = table.Column<int>(type: "int", nullable: false),
                    PropertyCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    LoanPurpose = table.Column<int>(type: "int", nullable: false),
                    BuilderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BuilderRegistrationNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BuilderContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BuilderEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "LoanApplicants",
                columns: table => new
                {
                    LoanApplicantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ApplicantRole = table.Column<int>(type: "int", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanApplicants", x => x.LoanApplicantId);
                    table.ForeignKey(
                        name: "FK_LoanApplicants_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanApplicants_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
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
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    DistrictOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryOfBirth = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PANNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EducationQualification = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ResidentialStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    SignatureImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IDProofImage = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalDetails", x => x.PersonalDetailsId);
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
                    EmployerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkExperienceYears = table.Column<int>(type: "int", nullable: false),
                    OfficeAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
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
                name: "PropertyDetails",
                columns: table => new
                {
                    PropertyDetailsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PropertyType = table.Column<int>(type: "int", nullable: false),
                    PropertyAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OwnershipType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyDetails", x => x.PropertyDetailsId);
                    table.ForeignKey(
                        name: "FK_PropertyDetails_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleInformations",
                columns: table => new
                {
                    VehicleInformationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Variant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ManufacturingYear = table.Column<int>(type: "int", nullable: false),
                    VehicleCondition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ExShowroomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleInformations", x => x.VehicleInformationId);
                    table.ForeignKey(
                        name: "FK_VehicleInformations_LoanApplicationBases_LoanApplicationBaseId",
                        column: x => x.LoanApplicationBaseId,
                        principalTable: "LoanApplicationBases",
                        principalColumn: "LoanApplicationBaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleLoanApplications",
                columns: table => new
                {
                    LoanApplicationBaseId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<int>(type: "int", nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Variant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManufacturingYear = table.Column<int>(type: "int", nullable: false),
                    ExShowroomPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OnRoadPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DownPayment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestedLoanAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TenureMonths = table.Column<int>(type: "int", nullable: false),
                    InterestRate = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    LoanPurposeVehicle = table.Column<int>(type: "int", nullable: false),
                    DealerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DealerAddress = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DealerContactNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    DealerEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
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
                name: "IX_DocumentUploads_LoanApplicationBaseId",
                table: "DocumentUploads",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentUploads_UserId",
                table: "DocumentUploads",
                column: "UserId");

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
                name: "IX_LoanApplicants_CustomerId",
                table: "LoanApplicants",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanApplicants_LoanApplicationBaseId",
                table: "LoanApplicants",
                column: "LoanApplicationBaseId");

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
                name: "IX_PersonalDetails_LoanApplicationBaseId",
                table: "PersonalDetails",
                column: "LoanApplicationBaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyDetails_LoanApplicationBaseId",
                table: "PropertyDetails",
                column: "LoanApplicationBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleInformations_LoanApplicationBaseId",
                table: "VehicleInformations",
                column: "LoanApplicationBaseId");
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
                name: "HomeLoanApplications");

            migrationBuilder.DropTable(
                name: "LoanAccounts");

            migrationBuilder.DropTable(
                name: "LoanApplicants");

            migrationBuilder.DropTable(
                name: "LoanDetails");

            migrationBuilder.DropTable(
                name: "LoanOriginationWorkflows");

            migrationBuilder.DropTable(
                name: "LoanProducts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "PersonalDetails");

            migrationBuilder.DropTable(
                name: "PersonalLoanApplications");

            migrationBuilder.DropTable(
                name: "PropertyDetails");

            migrationBuilder.DropTable(
                name: "VehicleInformations");

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
