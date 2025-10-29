using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;
using System.Runtime.ConstrainedExecution;

namespace Kanini.LMP.Database
{
    public class LmpDbContext : DbContext
    {
        public LmpDbContext(DbContextOptions<LmpDbContext> options) : base(options) { }

        /* ---------- Aggregates ---------- */
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<LoanApplicationBase> LoanApplicationBases => Set<LoanApplicationBase>();
        public DbSet<PersonalLoanApplication> PersonalLoanApplications => Set<PersonalLoanApplication>();
        public DbSet<HomeLoanApplication> HomeLoanApplications => Set<HomeLoanApplication>();
        public DbSet<VehicleLoanApplication> VehicleLoanApplications => Set<VehicleLoanApplication>();

        public DbSet<LoanApplicant> LoanApplicants => Set<LoanApplicant>();
        public DbSet<ApplicationDocumentLink> ApplicationDocumentLinks => Set<ApplicationDocumentLink>();
        public DbSet<DocumentUpload> DocumentUploads => Set<DocumentUpload>();
        public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
        public DbSet<EMIPlan> EMIPlans => Set<EMIPlan>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
        public DbSet<LoanOriginationWorkflow> LoanOriginationWorkflows => Set<LoanOriginationWorkflow>();
        public DbSet<LoanAccount> LoanAccounts => Set<LoanAccount>();
        public DbSet<Notification> Notifications => Set<Notification>();

        /* ---------- Common-detail entities ---------- */
        public DbSet<AddressInformation> AddressInformations => Set<AddressInformation>();
        public DbSet<PersonalDetails> PersonalDetails => Set<PersonalDetails>();
        public DbSet<FamilyEmergencyDetails> FamilyEmergencyDetails => Set<FamilyEmergencyDetails>();
        public DbSet<EmploymentDetails> EmploymentDetails => Set<EmploymentDetails>();
        public DbSet<FinancialInformation> FinancialInformations => Set<FinancialInformation>();
        public DbSet<LoanDetails> LoanDetails => Set<LoanDetails>();
        public DbSet<Declaration> Declarations => Set<Declaration>();

        /* ---------- Product-specific details ---------- */
        public DbSet<BuilderInformation> BuilderInformations => Set<BuilderInformation>();
        public DbSet<HomeLoanDetails> HomeLoanDetails => Set<HomeLoanDetails>();
        public DbSet<PropertyDetails> PropertyDetails => Set<PropertyDetails>();
        public DbSet<DealerInformation> DealerInformations => Set<DealerInformation>();
        public DbSet<VehicleInformation> VehicleInformations => Set<VehicleInformation>();
        public DbSet<VehicleLoanDetails> VehicleLoanDetails => Set<VehicleLoanDetails>();


        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            //1.TPT(Table - Per - Type) Inheritance Setup

            // TPT configuration requires that each derived entity uses its own table.

            b.Entity<LoanApplicationBase>().ToTable("LoanApplicationBases");

            // Explicit TPT configuration for derived entities
            b.Entity<PersonalLoanApplication>().ToTable("PersonalLoanApplications");
            b.Entity<HomeLoanApplication>().ToTable("HomeLoanApplications");
            b.Entity<VehicleLoanApplication>().ToTable("VehicleLoanApplications");


            //2. COMPOSITE KEYS AND CORE RELATIONSHIPS (Customer, User, LoanProduct)

            // User (1) to Customer (1) - Standard 1:1 relationship
            b.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict to prevent user deletion

            // Make LoanProductName unique and the target of the FK.
            b.Entity<LoanProduct>()
                .HasIndex(lp => lp.LoanProductName) // Ensure it's unique
                .IsUnique();

            // LoanApplicationBase (M) to LoanProduct (1)
            b.Entity<LoanApplicationBase>()
                .HasOne<LoanProduct>()
                .WithMany()
                .HasForeignKey(la => la.LoanProductType) // The string FK column in LoanApplicationBase
                .HasPrincipalKey(lp => lp.LoanProductName) // <--- TARGETS THE STRING COLUMN
                .OnDelete(DeleteBehavior.Restrict);


            //3. M:M JOINT ACCOUNTS (LoanApplicant)

            b.Entity<LoanApplicant>()
       .HasKey(la => new { la.LoanApplicationBaseId, la.CustomerId });

            b.Entity<LoanApplicant>()
                .HasOne(la => la.LoanApplicationBase)
                .WithMany(app => app.Applicants)
                .HasForeignKey(la => la.LoanApplicationBaseId) // Uses Base ID
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicant>()
                .HasOne(la => la.Customer)
                .WithMany(c => c.Applications)
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            //4. M:M DOCUMENT UPLOADS (ApplicationDocumentLink)

            b.Entity<ApplicationDocumentLink>()
       .HasKey(ad => new { ad.LoanApplicationBaseId, ad.DocumentId });

            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.LoanApplicationBase)
                .WithMany(app => app.DocumentLinks)
                .HasForeignKey(ad => ad.LoanApplicationBaseId) // Uses Base ID
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.DocumentUpload)
                .WithMany(doc => doc.ApplicationLinks)
                .HasForeignKey(ad => ad.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            //5. 1:1 NESTED DETAILS 

            // Linking to UserId will breaks the TPT architecture entirely so better to use LoanApplicationBaseId here.

            Action<ModelBuilder, Type, string, string> ConfigureOneToOne = (builder, detailType, navPropName, fkPropName) =>
            {
                builder.Entity<LoanApplicationBase>()
                    .HasOne(navPropName)
                    .WithOne()
                    .HasForeignKey(detailType, fkPropName)
                    .OnDelete(DeleteBehavior.Cascade);
            };

            ConfigureOneToOne(b, typeof(LoanDetails), nameof(LoanApplicationBase.LoanDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(PersonalDetails), nameof(LoanApplicationBase.PersonalDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(AddressInformation), nameof(LoanApplicationBase.AddressInformation), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(FamilyEmergencyDetails), nameof(LoanApplicationBase.FamilyEmergencyDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(EmploymentDetails), nameof(LoanApplicationBase.EmploymentDetails), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(FinancialInformation), nameof(LoanApplicationBase.FinancialInformation), "LoanApplicationBaseId");
            ConfigureOneToOne(b, typeof(Declaration), nameof(LoanApplicationBase.Declaration), "LoanApplicationBaseId");

            // Configure Product-Specific Details 1:1 link to their respective derived class
            // Configure Product-Specific Details 1:1 link to their respective derived class
            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.PropertyDetails)
                .WithOne()
                .HasForeignKey<PropertyDetails>(pd => pd.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.VehicleInformation)
                .WithOne()
                .HasForeignKey<VehicleInformation>(vi => vi.LoanApplicationBaseId) // FIX APPLIED HERE
                .OnDelete(DeleteBehavior.Cascade);

            // FIX ADDED for DealerInformation (assuming this belongs to VehicleLoanApplication)
            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.DealerInformation)
                .WithOne()
                .HasForeignKey<DealerInformation>(di => di.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // FIX ADDED for VehicleLoanDetails (assuming this belongs to VehicleLoanApplication)
            b.Entity<VehicleLoanApplication>()
                .HasOne(vla => vla.VehicleLoanDetails)
                .WithOne()
                .HasForeignKey<VehicleLoanDetails>(vld => vld.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // FIX ADDED for BuilderInformation (assuming this belongs to HomeLoanApplication)
            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.BuilderInformation)
                .WithOne()
                .HasForeignKey<BuilderInformation>(bi => bi.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            // FIX ADDED for HomeLoanDetails (assuming this belongs to HomeLoanApplication)
            b.Entity<HomeLoanApplication>()
                .HasOne(hla => hla.HomeLoanDetails)
                .WithOne()
                .HasForeignKey<HomeLoanDetails>(hld => hld.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Cascade);

            //6.MANAGER MODELS INTEGRATION

            // LoanOriginationWorkflow links to LoanApplicationBase
            b.Entity<LoanOriginationWorkflow>()
                .HasOne<LoanApplicationBase>()
                .WithMany()
                .HasForeignKey(w => w.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanAccount links to LoanApplicationBase (1:1)
            b.Entity<LoanAccount>()
                .HasOne<LoanApplicationBase>()
                .WithOne()
                .HasForeignKey<LoanAccount>(a => a.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // LoanAccount links to Customer (M:1)
            b.Entity<LoanAccount>()
                .HasOne<Customer>()
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);


            // 7. PAYMENT AND MANAGER ENTITIES


            // PaymentTransaction (M) to EMIPlan (1)
            b.Entity<PaymentTransaction>()
                .HasOne<EMIPlan>()
                .WithMany()
                .HasForeignKey(pt => pt.EMIId)
                .OnDelete(DeleteBehavior.Restrict);

            // EMIPlan (M) to LoanAccount (1) - Assumes EMIPlan is linked to the Servicing Account
            b.Entity<EMIPlan>()
                .HasOne<LoanAccount>()
                .WithMany()
                .HasForeignKey(e => e.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);


            //8. NOTIFICATIONS ENTITY
            b.Entity<Notification>(entity =>
            {
                // Define the primary key
                entity.HasKey(n => n.NotificationId);

                // Define the Foreign Key relationship to the User table
                entity.HasOne(n => n.User)                  // A Notification has one User (recipient)
                      .WithMany()                           // The User can have many Notifications
                      .HasForeignKey(n => n.UserId)         // The foreign key is the UserId property
                      .IsRequired()                         // Ensures UserId cannot be null
                      .OnDelete(DeleteBehavior.Cascade);    // OPTIONAL: Delete notifications if the User is deleted
            });


            // 9. FINAL SAFETY NET
            // Ensure all remaining relationships default to RESTRICT for safety
            foreach (var relationship in b.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


            // --------------------------------------------------------------------
            // 1. LOAN PRODUCTS
            // --------------------------------------------------------------------
            b.Entity<LoanProduct>().HasData(
                new LoanProduct
                {
                    LoanProductId = 1,
                    LoanProductName = "PersonalLoan-Standard",
                    LoanProductDescription = "Unsecured loan for personal use.",
                    IsActive = true
                },
                new LoanProduct
                {
                    LoanProductId = 2,
                    LoanProductName = "HomeLoan-New",
                    LoanProductDescription = "Home loan for property purchase.",
                    IsActive = true
                },
                new LoanProduct
                {
                    LoanProductId = 3,
                    LoanProductName = "CarLoan-Used",
                    LoanProductDescription = "Loan for used vehicles.",
                    IsActive = true
                });

            // --------------------------------------------------------------------
            // 2. USERS
            // --------------------------------------------------------------------
            b.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Alex Johnson",
                    Email = "alex.johnson@customer.com",
                    PasswordHash = "hashedpassword_alex",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 2,
                    FullName = "Emma Brown",
                    Email = "emma.brown@customer.com",
                    PasswordHash = "hashedpassword_emma",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2025, 10, 1, 15, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 3,
                    FullName = "Alice Smith (Credit)",
                    Email = "alice.smith@manager.com",
                    PasswordHash = "hashedpassword_alice",
                    Roles = UserEnums.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 15, 8, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 4,
                    FullName = "Bob Davis (Verification)",
                    Email = "bob.davis@manager.com",
                    PasswordHash = "hashedpassword_bob",
                    Roles = UserEnums.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 2, 20, 9, 30, 0, DateTimeKind.Utc)
                });

            // --------------------------------------------------------------------
            // 3. CUSTOMERS
            // --------------------------------------------------------------------
            b.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    UserId = 1,
                    DateOfBirth = new DateOnly(1995, 10, 20),
                    Gender = Gender.Male,
                    PhoneNumber = "9876543210",
                    Occupation = "Software Engineer",
                    AnnualIncome = 1_500_000.00m,
                    CreditScore = 780m,
                    ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                    UpdatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    HomeOwnershipStatus = HomeOwnershipStatus.Owned
                },
                new Customer
                {
                    CustomerId = 2,
                    UserId = 2,
                    DateOfBirth = new DateOnly(1998, 5, 15),
                    Gender = Gender.Female,
                    PhoneNumber = "8012345678",
                    Occupation = "Data Analyst",
                    AnnualIncome = 950_000.00m,
                    CreditScore = 725m,
                    ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF },
                    UpdatedAt = new DateTime(2025, 1, 15, 0, 0, 0, DateTimeKind.Utc),
                    HomeOwnershipStatus = HomeOwnershipStatus.Rented
                });

            // --------------------------------------------------------------------
            // 4. LOAN APPLICATION BASE (TPT discriminator)
            // --------------------------------------------------------------------
            b.Entity<LoanApplicationBase>().HasData(
                // Personal Loan – Approved
                new LoanApplicationBase
                {
                    LoanApplicationBaseId = 1,
                    LoanProductType = "PersonalLoan-Standard",
                    Status = ApplicationStatus.Approved,
                    SubmissionDate = new DateOnly(2025, 1, 15),
                    ApprovedDate = new DateOnly(2025, 1, 25),
                    IsActive = true
                },
                // Home Loan – Submitted
                new LoanApplicationBase
                {
                    LoanApplicationBaseId = 2,
                    LoanProductType = "HomeLoan-New",
                    Status = ApplicationStatus.Submitted,
                    SubmissionDate = new DateOnly(2025, 10, 20),
                    IsActive = true
                });

            // --------------------------------------------------------------------
            // 5. LOAN APPLICANTS (junction)
            // --------------------------------------------------------------------
            b.Entity<LoanApplicant>().HasData(
                new LoanApplicant
                {
                    LoanApplicationBaseId = 1,
                    CustomerId = 1,
                    ApplicantRole = ApplicantRole.Primary,
                    AddedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new LoanApplicant
                {
                    LoanApplicationBaseId = 1,
                    CustomerId = 2,
                    ApplicantRole = ApplicantRole.CoBorrower,
                    AddedDate = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc)
                },
                new LoanApplicant
                {
                    LoanApplicationBaseId = 2,
                    CustomerId = 2,
                    ApplicantRole = ApplicantRole.Primary,
                    AddedDate = new DateTime(2025, 10, 17, 0, 0, 0, DateTimeKind.Utc)
                });

            // --------------------------------------------------------------------
            // 6. PERSONAL DETAILS
            // --------------------------------------------------------------------
            var sig = new byte[] { 0x53, 0x49, 0x47 };
            var idp = new byte[] { 0x49, 0x44, 0x50 };

            b.Entity<PersonalDetails>().HasData(
                new PersonalDetails
                {
                    PersonalDetailsId = 1,
                    LoanApplicationBaseId = 1,
                    UserId = 1,
                    FullName = "Alex Johnson",
                    DateOfBirth = new DateOnly(1995, 10, 20),
                    DistrictOfBirth = "Bengaluru",
                    CountryOfBirth = "India",
                    PANNumber = "ABCDE1234F",
                    EducationQualification = "Master's Degree",
                    ResidentialStatus = "Owner",
                    Gender = Gender.Male,
                    SignatureImage = sig,
                    IDProofImage = idp
                },
                new PersonalDetails
                {
                    PersonalDetailsId = 2,
                    LoanApplicationBaseId = 2,
                    UserId = 2,
                    FullName = "Emma Brown",
                    DateOfBirth = new DateOnly(1998, 5, 15),
                    DistrictOfBirth = "Pune",
                    CountryOfBirth = "India",
                    PANNumber = "FGHIJ5678K",
                    EducationQualification = "Bachelor's Degree",
                    ResidentialStatus = "Renter",
                    Gender = Gender.Female,
                    SignatureImage = sig,
                    IDProofImage = idp
                });

            // --------------------------------------------------------------------
            // 7. LOAN DETAILS (Personal Loan only)
            // --------------------------------------------------------------------
            b.Entity<LoanDetails>().HasData(
                new LoanDetails
                {
                    LoanDetailsId = 1,
                    LoanApplicationBaseId = 1,
                    RequestedAmount = 500_000.00m,
                    TenureMonths = 60,
                    AppliedDate = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                    InterestRate = 12.00m,
                    MonthlyInstallment = 11_122.22m
                });

            // --------------------------------------------------------------------
            // 8. HOME LOAN DETAILS (Home Loan only)
            // --------------------------------------------------------------------
            b.Entity<HomeLoanDetails>().HasData(
                new HomeLoanDetails
                {
                    HomeLoanDetailsId = 1,
                    LoanApplicationBaseId = 2,
                    PropertyCost = 4_500_000.00m,
                    DownPayment = 900_000.00m,
                    RequestedLoanAmount = 3_600_000.00m,
                    TenureMonths = 360,
                    InterestRate = 9.00m,
                    AppliedDate = new DateTime(2025, 10, 20, 15, 30, 0, DateTimeKind.Utc),
                    LoanPurpose = LoanPurposeHome.Construction
                });

            // --------------------------------------------------------------------
            // 9. LOAN ACCOUNT (only for disbursed loan)
            // --------------------------------------------------------------------
            b.Entity<LoanAccount>().HasData(
                new LoanAccount
                {
                    LoanAccountId = 1,
                    LoanApplicationBaseId = 1,
                    CustomerId = 1,                     // primary applicant
                    CurrentPaymentStatus = LoanPaymentStatus.Active,
                    DisbursementDate = new DateTime(2025, 2, 1),
                    DaysPastDue = 0,
                    LastStatusUpdate = new DateTime(2025, 10, 25),
                    TotalLoanAmount = 500_000.00m,
                    TotalPaidPrincipal = 150_000.00m,
                    TotalPaidInterest = 45_000.00m,
                    PrincipalRemaining = 350_000.00m,
                    LastPaymentDate = new DateTime(2025, 10, 25),
                    TotalLateFeePaidAmount = 0.00m
                });

            // --------------------------------------------------------------------
            // 10. EMI PLAN (only for active loan)
            // --------------------------------------------------------------------
            b.Entity<EMIPlan>().HasData(
                new EMIPlan
                {
                    EMIId = 1,
                    LoanApplicationBaseId = 1,
                    LoanAccountId = 1,
                    PrincipleAmount = 500_000.00m,
                    TermMonths = 60,
                    RateOfInterest = 12.00m,
                    MonthlyEMI = 11_122.22m,
                    TotalInterestPaid = 167_333.20m,
                    TotalRepaymentAmount = 667_333.20m,
                    Status = EMIPlanStatus.Active,
                    IsCompleted = false
                });

            // --------------------------------------------------------------------
            // 11. DOCUMENTS & LINKS
            // --------------------------------------------------------------------
            var utilBill = new byte[] { 0x55, 0x54, 0x49, 0x4C };
            var payStub = new byte[] { 0x50, 0x41, 0x59, 0x53 };

            b.Entity<DocumentUpload>().HasData(
                new DocumentUpload
                {
                    DocumentId = 1,
                    LoanApplicationBaseId = 1,
                    UserId = 1,
                    DocumentName = "Electricity Bill Jan 2025",
                    DocumentType = "AddressProof",
                    UploadedAt = new DateTime(2025, 1, 15, 11, 0, 0, DateTimeKind.Utc),
                    DocumentData = utilBill
                },
                new DocumentUpload
                {
                    DocumentId = 2,
                    LoanApplicationBaseId = 2,
                    UserId = 2,
                    DocumentName = "Pay Stub October 2025",
                    DocumentType = "IncomeProof",
                    UploadedAt = new DateTime(2025, 10, 20, 16, 0, 0, DateTimeKind.Utc),
                    DocumentData = payStub
                });

            b.Entity<ApplicationDocumentLink>().HasData(
                new ApplicationDocumentLink
                {
                    LoanApplicationBaseId = 1,
                    DocumentId = 1,
                    DocumentRequirementType = "AddressProof-UtilityBill",
                    LinkedAt = new DateTime(2025, 1, 10, 0, 0, 0, DateTimeKind.Utc)
                },
                new ApplicationDocumentLink
                {
                    LoanApplicationBaseId = 2,
                    DocumentId = 2,
                    DocumentRequirementType = "IncomeProof-PayStub",
                    LinkedAt = new DateTime(2025, 10, 17, 0, 0, 0, DateTimeKind.Utc)
                });

            // --------------------------------------------------------------------
            // 12. ADDRESS INFORMATION
            // --------------------------------------------------------------------
            b.Entity<AddressInformation>().HasData(
                new AddressInformation
                {
                    AddressInformationId = 1,
                    LoanApplicationBaseId = 1,
                    UserId = 1,
                    PresentAddress = "Apt 101, Prestige Towers, Main Rd",
                    PermanentAddress = "Same as Present Address",
                    District = "Bengaluru",
                    State = IndianStates.Karnataka,
                    Country = "India",
                    ZipCode = "560001",
                    EmailId = "alex.johnson@example.com",
                    MobileNumber1 = 987654321,
                    MobileNumber2 = 998877665
                },
                new AddressInformation
                {
                    AddressInformationId = 2,
                    LoanApplicationBaseId = 2,
                    UserId = 2,
                    PresentAddress = "House 5, Green Valley Colony, North Ave",
                    PermanentAddress = "House 5, Green Valley Colony, North Ave",
                    District = "Pune",
                    State = IndianStates.Maharashtra,
                    Country = "India",
                    ZipCode = "411001",
                    EmailId = "emma.brown@example.com",
                    MobileNumber1 = 801234567,
                    MobileNumber2 = 809876543
                });

            // --------------------------------------------------------------------
            // 13. EMPLOYMENT DETAILS
            // --------------------------------------------------------------------
            b.Entity<EmploymentDetails>().HasData(
                new EmploymentDetails
                {
                    EmploymentDetailsId = 1,
                    UserId = 1,
                    CompanyName = "TechCorp Solutions",
                    Designation = "Senior Software Engineer",
                    Experience = 5,
                    EmailId = "alex.johnson@techcorp.com",
                    OfficeAddress = "TechCorp Tower, IT Hub, Bengaluru"
                },
                new EmploymentDetails
                {
                    EmploymentDetailsId = 2,
                    UserId = 2,
                    CompanyName = "Manufacturing Hub",
                    Designation = "Data Analyst",
                    Experience = 3,
                    EmailId = "emma.brown@manuhub.in",
                    OfficeAddress = "Unit B, Industrial Area, Pune"
                });

            // --------------------------------------------------------------------
            // 14. FINANCIAL INFORMATION
            // --------------------------------------------------------------------
            b.Entity<FinancialInformation>().HasData(
                new FinancialInformation
                {
                    FinancialInformationId = 1,
                    UserId = 1,
                    Salary = 125_000,
                    Rent = 25_000,
                    PrimaryOther = 5_000,
                    RentandUtility = 5_000,
                    FoodandClothing = 15_000,
                    Education = 0,
                    LoanRepayment = 30_000,
                    ExpenseOther = 5_000
                },
                new FinancialInformation
                {
                    FinancialInformationId = 2,
                    UserId = 2,
                    Salary = 79_166,
                    Rent = 15_000,
                    PrimaryOther = 2_000,
                    RentandUtility = 3_000,
                    FoodandClothing = 10_000,
                    Education = 5_000,
                    LoanRepayment = 15_000,
                    ExpenseOther = 3_000
                });

            // --------------------------------------------------------------------
            // 15. DECLARATIONS
            // --------------------------------------------------------------------
            b.Entity<Declaration>().HasData(
                new Declaration
                {
                    DeclarationId = 1,
                    LoanApplicationBaseId = 1,
                    Name = "No Existing Debt Declaration",
                    Amount = 0,
                    Description = "Applicant declares no undisclosed existing loans or financial liabilities.",
                    Purpose = "Compliance Check"
                },
                new Declaration
                {
                    DeclarationId = 2,
                    LoanApplicationBaseId = 2,
                    Name = "Source of Down Payment",
                    Amount = 50_000,
                    Description = "Applicant declares that the down payment of 50000 is from personal savings.",
                    Purpose = "AML/KYC Check"
                });

            // --------------------------------------------------------------------
            // 16. LOAN ORIGINATION WORKFLOW
            // --------------------------------------------------------------------
            b.Entity<LoanOriginationWorkflow>().HasData(
                new LoanOriginationWorkflow
                {
                    WorkflowId = 1,
                    LoanApplicationBaseId = 1,
                    StepName = ManagerEnum.Review,
                    StepStatus = StepStatus.Completed,
                    CompletionDate = new DateTime(2025, 1, 20, 14, 30, 0, DateTimeKind.Utc),
                    ManagerId = 3,
                    ManagerNotes = "Approved with standard rate."
                });

            // --------------------------------------------------------------------
            // 17. NOTIFICATIONS
            // --------------------------------------------------------------------
            b.Entity<Notification>().HasData(
                new Notification
                {
                    NotificationId = 1,
                    UserId = 1,
                    Title = "Loan Disbursed Successfully",
                    Message = "Funds disbursed to your account.",
                    IsRead = false,
                    CreatedAt = new DateTime(2025, 2, 2, 9, 30, 0, DateTimeKind.Utc)
                });

            // --------------------------------------------------------------------
            // 18. OPTIONAL: Builder, Property, Dealer, Vehicle, PaymentTransaction …
            // --------------------------------------------------------------------
            // (Add only the tables you actually use – the pattern is the same:
            //  replace every Guid with an int and keep the FK values consistent.)

         
    }
    }
}