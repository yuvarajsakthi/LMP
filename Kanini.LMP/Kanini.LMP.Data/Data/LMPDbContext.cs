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






            //BELOW FLUENT API IS NOT NEEDED AFTER REFACTORING TO TPT AND IMPROVED RELATIONSHIPS.

            //seeding starts

            b.Entity<Customer>().HasData(
    // --- Customer Record 1: Primary Applicant Profile ---
    new Customer
    {
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-333333333333"), // Used in the previous LoanApplicant seed
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        DateOfBirth = new DateOnly(1995, 10, 20), // Age will be calculated
        Gender = Gender.Male,
        PhoneNumber = "9876543210",
        Occupation = "Software Engineer",
        AnnualIncome = 1500000.00m,
        CreditScore = 780m,
        ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, // Placeholder byte[]
        UpdatedAt = DateTime.UtcNow,
        HomeOwnershipStatus = HomeOwnershipStatus.Owned
    },

    // --- Customer Record 2: Co-Applicant Profile ---
    new Customer
    {
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-444444444444"), // Used in the previous LoanApplicant seed
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        DateOfBirth = new DateOnly(1998, 05, 15), // Age will be calculated
        Gender = Gender.Female,
        PhoneNumber = "8012345678",
        Occupation = "Data Analyst",
        AnnualIncome = 950000.00m,
        CreditScore = 725m,
        ProfileImage = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF }, // Placeholder byte[]
        UpdatedAt = DateTime.UtcNow,
        HomeOwnershipStatus = HomeOwnershipStatus.Rented
    }
);


            b.Entity<EMIPlan>().HasData(
    // --- EMI Plan Record 1: Long-term, High-value, Active Loan ---
    new EMIPlan
    {
        EMIId = Guid.Parse("e0e1e2e3-f1f2-g1g2-h1h2-111111111111"),
        LoanAppicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"), // FK to a loan application
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-111111111111"),     // FK to a loan account

        PrincipleAmount = 500000.00m,
        TermMonths = 60,            // 5 years
        RateOfInterest = 12.00m,    // 12% Annual

        // Calculated based on P=500000, R_monthly=1%, N=60
        MonthlyEMI = 11122.22m,
        TotalInerestPaid = 167333.20m, // (11122.22 * 60) - 500000
        TotalRepaymentAmount = 667333.20m,

        Status = EMIPlanStatus.Active,
        IsCompleted = false
    },

    // --- EMI Plan Record 2: Short-term, Low-value, Closed Loan ---
    new EMIPlan
    {
        EMIId = Guid.Parse("e0e1e2e3-f1f2-g1g2-h1h2-222222222222"),
        LoanAppicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"), // FK to a different loan application
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-222222222222"),     // FK to a different loan account

        PrincipleAmount = 50000.00m,
        TermMonths = 12,            // 1 year
        RateOfInterest = 10.00m,    // 10% Annual

        // Calculated based on P=50000, R_monthly=0.833%, N=12
        MonthlyEMI = 4403.95m,
        TotalInerestPaid = 2847.40m, // (4403.95 * 12) - 50000
        TotalRepaymentAmount = 52847.40m,

        Status = EMIPlanStatus.Closed,
        IsCompleted = true
    }
);


            b.Entity<LoanDetails>().HasData(
           new LoanDetails
           {
               LoanDetailsId = Guid.Parse("ld1d2d3d-4e5e-6f7f-8a9a-111111111111"),
               LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
               LoanApplicationId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"), // Same as BaseId if it's a PersonalLoanApplication
               RequestedAmount = 500000.00m,
               TenureMonths = 60,
               AppliedDate = new DateTime(2025, 01, 15, 10, 00, 00, DateTimeKind.Utc),
               InterestRate = 12.00m,
               MonthlyInstallment = 11122.22m
           },
           new LoanDetails
           {
               LoanDetailsId = Guid.Parse("ld1d2d3d-4e5e-6f7f-8a9a-222222222222"),
               LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
               LoanApplicationId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
               RequestedAmount = 50000.00m,
               TenureMonths = 12,
               AppliedDate = new DateTime(2025, 10, 20, 15, 30, 00, DateTimeKind.Utc),
               InterestRate = 10.00m,
               MonthlyInstallment = 4403.95m
           }
       );

            var placeholderSignature = new byte[] { 0x53, 0x49, 0x47 };
            var placeholderIdProof = new byte[] { 0x49, 0x44, 0x50 };

            b.Entity<PersonalDetails>().HasData(
                new PersonalDetails
                {
                    PersonalDetailsId = Guid.Parse("pd1d2d3d-4e5e-6f7f-8a9a-111111111111"),
                    LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
                    UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
                    FullName = "Alex Johnson",
                    DateOfBirth = new DateOnly(1995, 10, 20),
                    DistrictOfBirth = "Bengaluru",
                    CountryOfBirth = "India",
                    PANNumber = "ABCDE1234F",
                    EducationQualification = "Master's Degree",
                    ResidentialStatus = "Owner",
                    Gender = Gender.Male,
                    SignatureImage = placeholderSignature,
                    IDProofImage = placeholderIdProof
                },
                new PersonalDetails
                {
                    PersonalDetailsId = Guid.Parse("pd1d2d3d-4e5e-6f7f-8a9a-222222222222"),
                    LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
                    UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
                    FullName = "Emma Brown",
                    DateOfBirth = new DateOnly(1988, 03, 10),
                    DistrictOfBirth = "Pune",
                    CountryOfBirth = "India",
                    PANNumber = "FGHIJ5678K",
                    EducationQualification = "Bachelor's Degree",
                    ResidentialStatus = "Renter",
                    Gender = Gender.Female,
                    SignatureImage = placeholderSignature,
                    IDProofImage = placeholderIdProof
                }
            );

            b.Entity<AddressInformation>().HasData(
    new AddressInformation
    {
        AddressInformationId = Guid.Parse("ai1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
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
        AddressInformationId = Guid.Parse("ai1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        PresentAddress = "House 5, Green Valley Colony, North Ave",
        PermanentAddress = "House 5, Green Valley Colony, North Ave",
        District = "Pune",
        State = IndianStates.Maharashtra,
        Country = "India",
        ZipCode = "411001",
        EmailId = "emma.brown@example.com",
        MobileNumber1 = 801234567,
        MobileNumber2 = 809876543
    }
);

            b.Entity<FamilyEmergencyDetails>().HasData(
    new FamilyEmergencyDetails
    {
        FamilyEmergencyDetailsId = Guid.Parse("fe1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        FullName = "Mia Johnson",
        RelationshipWithApplicant = "Spouse",
        MobileNumber = 900001111,
        Address = "Apt 101, Prestige Towers, Main Rd, Bengaluru"
    },
    new FamilyEmergencyDetails
    {
        FamilyEmergencyDetailsId = Guid.Parse("fe1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        FullName = "David Brown",
        RelationshipWithApplicant = "Father",
        MobileNumber = 955552222,
        Address = "House 5, Green Valley Colony, North Ave, Pune"
    }
);


            b.Entity<EmploymentDetails>().HasData(
    new EmploymentDetails
    {
        EmploymentDetailsId = Guid.Parse("ed1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        CompanyName = "TechCorp Solutions",
        Designation = "Senior Software Engineer",
        Experience = 5,
        EmailId = "alex.johnson@techcorp.com",
        OfficeAddress = "TechCorp Tower, IT Hub, Bengaluru"
    },
    new EmploymentDetails
    {
        EmploymentDetailsId = Guid.Parse("ed1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        CompanyName = "Manufacturing Hub",
        Designation = "Data Analyst",
        Experience = 3,
        EmailId = "emma.brown@manuhub.in",
        OfficeAddress = "Unit B, Industrial Area, Pune"
    }
);



            b.Entity<FinancialInformation>().HasData(
    new FinancialInformation
    {
        FinancialInformationId = Guid.Parse("fi1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        Salary = 125000, // Monthly salary in INR
        Rent = 25000,
        PrimaryOther = 5000, // Other income
        RentandUtility = 5000,
        FoodandClothing = 15000,
        Education = 0,
        LoanRepayment = 30000,
        ExpenseOther = 5000
    },
    new FinancialInformation
    {
        FinancialInformationId = Guid.Parse("fi1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        Salary = 79166, // Monthly salary (approx 950000 annual)
        Rent = 15000,
        PrimaryOther = 2000,
        RentandUtility = 3000,
        FoodandClothing = 10000,
        Education = 5000,
        LoanRepayment = 15000,
        ExpenseOther = 3000
    }
);


            b.Entity<Declaration>().HasData(
    new Declaration
    {
        DeclarationId = Guid.Parse("dc1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        Name = "No Existing Debt Declaration",
        Amount = 0,
        Description = "Applicant declares no undisclosed existing loans or financial liabilities.",
        Purpose = "Compliance Check"
    },
    new Declaration
    {
        DeclarationId = Guid.Parse("dc1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        Name = "Source of Down Payment",
        Amount = 50000,
        Description = "Applicant declares that the down payment of 50000 is from personal savings.",
        Purpose = "AML/KYC Check"
    }
);

            var utilityBillData = new byte[] { 0x55, 0x54, 0x49, 0x4C }; // "UTIL"
            var payStubData = new byte[] { 0x50, 0x41, 0x59, 0x53 };     // "PAYS"

            b.Entity<DocumentUpload>().HasData(
                // --- Document Record 1: Utility Bill for Loan 1 ---
                new DocumentUpload
                {
                    DocumentId = Guid.Parse("d1d2d3d4-e1e2-f1f2-0a0b-111111111111"), // Used in ApplicationDocumentLink
                    LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"), // Loan 1
                    UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"), // User 1
                    DocumentName = "Electricity Bill Jan 2025",
                    DocumentType = "AddressProof",
                    UploadedAt = new DateTime(2025, 01, 15, 11, 00, 00, DateTimeKind.Utc),
                    DocumentData = utilityBillData
                },

                // --- Document Record 2: Pay Stub for Loan 2 ---
                new DocumentUpload
                {
                    DocumentId = Guid.Parse("d1d2d3d4-e1e2-f1f2-0a0b-222222222222"), // Used in ApplicationDocumentLink
                    LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"), // Loan 2
                    UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"), // User 2
                    DocumentName = "Pay Stub October 2025",
                    DocumentType = "IncomeProof",
                    UploadedAt = new DateTime(2025, 10, 20, 16, 00, 00, DateTimeKind.Utc),
                    DocumentData = payStubData
                }
            );



            b.Entity<LoanApplicationBase>().HasData(
    // --- Application Record 1: Approved Personal Loan ---
    new LoanApplicationBase
    {
        // Primary Key
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),

        // FK to LoanProduct
        LoanProductType = "PersonalLoan-Standard",

        // Workflow Fields
        Status = ApplicationStatus.Approved,
        SubmissionDate = new DateOnly(2025, 01, 15),
        ApprovedDate = new DateOnly(2025, 01, 25),
        RejectionReason = null,
        IsActive = true

        // Note: Do not include the complex/navigation properties (LoanDetails, etc.) 
        // here unless they are true Owned Entities configured in OnModelCreating.
        // For separately keyed tables, EF Core manages the relationship based on FKs.
    },

    // --- Application Record 2: Submitted but Pending Car Loan ---
    new LoanApplicationBase
    {
        // Primary Key
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),

        // FK to LoanProduct
        LoanProductType = "CarLoan-Used",

        // Workflow Fields
        Status = ApplicationStatus.Submitted,
        SubmissionDate = new DateOnly(2025, 10, 20),
        ApprovedDate = null,
        RejectionReason = null,
        IsActive = true
    }
);





            b.Entity<BuilderInformation>().HasData(
    // --- Builder Record 1: For Application 1 (e.g., a Home Loan scenario) ---
    new BuilderInformation
    {
        BuilderInformationId = Guid.Parse("b1b2b3b4-c1c2-d1d2-e1e2-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"), // Loan 1
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"), // User 1
        BuilderName = "Greenfield Developers Pvt. Ltd.",
        ProjectName = "Emerald Heights Phase 2",
        BuilderRegistrationNo = "GFLD/2020/005",
        ContactNumber = "91900010001",
        Email = "sales@greenfielddev.com"
    },

    // --- Builder Record 2: For Application 2 (e.g., a different project) ---
    new BuilderInformation
    {
        BuilderInformationId = Guid.Parse("b1b2b3b4-c1c2-d1d2-e1e2-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"), // Loan 2
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"), // User 2
        BuilderName = "Cityscape Constructions Ltd.",
        ProjectName = "Metro Residences Block A",
        BuilderRegistrationNo = "CITY/2018/010",
        ContactNumber = "91900020002",
        Email = "info@cityscapeconst.in"
    }
);


            b.Entity<HomeLoanDetails>().HasData(
    // --- Home Loan Details Record 1: Purchase of a New Home ---
    new HomeLoanDetails
    {
        HomeLoanDetailsId = Guid.Parse("hl1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        LoanApplicationId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),

        PropertyCost = 7500000.00m,       // 75 Lakhs
        DownPayment = 1500000.00m,        // 15 Lakhs
        RequestedLoanAmount = 6000000.00m,  // 60 Lakhs
        TenureMonths = 240,               // 20 years
        InterestRate = 8.50m,
        AppliedDate = new DateTime(2025, 01, 15, 10, 00, 00, DateTimeKind.Utc),
        LoanPurpose = LoanPurposeHome.Purchase
    },

    // --- Home Loan Details Record 2: Plot Purchase and Construction ---
    new HomeLoanDetails
    {
        HomeLoanDetailsId = Guid.Parse("hl1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        LoanApplicationId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),

        PropertyCost = 4500000.00m,       // 45 Lakhs
        DownPayment = 900000.00m,         // 9 Lakhs
        RequestedLoanAmount = 3600000.00m,  // 36 Lakhs
        TenureMonths = 360,               // 30 years
        InterestRate = 9.00m,
        AppliedDate = new DateTime(2025, 10, 20, 15, 30, 00, DateTimeKind.Utc),
        LoanPurpose = LoanPurposeHome.Construction
    }
);


            b.Entity<PropertyDetails>().HasData(
    // --- Property Details Record 1: Apartment for Loan 1 ---
    new PropertyDetails
    {
        PropertyDetailsId = Guid.Parse("pdl1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),

        PropertyType = PropertyType.Residential,
        PropertyAddress = "Flat 502, Orchid Residency, Electronic City",
        City = "Bengaluru",
        State = "Karnataka",
        ZipCode = 560100,
        OwnershipType = OwnershipType.Owned
    },

    // --- Property Details Record 2: Plot for Construction Loan 2 ---
    new PropertyDetails
    {
        PropertyDetailsId = Guid.Parse("pdl1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),

        PropertyType = PropertyType.Industrial,
        PropertyAddress = "Plot 7, Phase 3, Green Meadows Layout, Off Highway 44",
        City = "Hyderabad",
        State = "Telangana",
        ZipCode = 500078,
        OwnershipType = OwnershipType.Builder
    }
);




            b.Entity<DealerInformation>().HasData(
    // --- Dealer Record 1: New Car Dealership for Application 1 ---
    new DealerInformation
    {
        DealerInformationId = Guid.Parse("dlinfo1d-2e3e-4f5f-6a7a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        DealerName = "Astra Motors Official",
        DealerAddress = "34/A, Ring Road, Industrial Zone, Bengaluru",
        ContactNumber = "918023456789",
        Email = "sales@astramotors.com"
    },

    // --- Dealer Record 2: Used Car Dealership for Application 2 ---
    new DealerInformation
    {
        DealerInformationId = Guid.Parse("dlinfo1d-2e3e-4f5f-6a7a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        DealerName = "Pre-Owned Wheels Hub",
        DealerAddress = "Unit 12, Main Street Market, Pune",
        ContactNumber = "919234567890",
        Email = "info@preownedwheels.in"
    }
);

            b.Entity<VehicleInformation>().HasData(
    // --- Vehicle Record 1: New Car for Application 1 ---
    new VehicleInformation
    {
        VehicleInformationId = Guid.Parse("vinfo1d2d3-4e5e-6f7f-8a9a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        VehicleType = VehicleType.Car,
        Manufacturer = "Astra Motors",
        Model = "Astra Sedan Pro",
        Variant = "Petrol Automatic",
        ManufacturingYear = 2025,
        VehicleCondition = LoanPurposeVehicle.New,
        ExShowroomPrice = 1200000.00m
    },

    // --- Vehicle Record 2: Used SUV for Application 2 ---
    new VehicleInformation
    {
        VehicleInformationId = Guid.Parse("vinfo1d2d3-4e5e-6f7f-8a9a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        VehicleType = VehicleType.SUV,
        Manufacturer = "Terra Vehicles",
        Model = "Terra X-Plore",
        Variant = "Diesel Manual",
        ManufacturingYear = 2022,
        VehicleCondition = LoanPurposeVehicle.Used,
        ExShowroomPrice = 1800000.00m
    }
);

            b.Entity<LoanProduct>().HasData(
    // --- Loan Product Record 1: Standard Personal Loan ---
    new LoanProduct
    {
        LoanProductId = Guid.Parse("lp1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        LoanProductName = "PersonalLoan-Standard",
        LoanProductDescription = "Unsecured loan for personal use, high flexibility, terms up to 7 years.",
        IsActive = true
    },

    // --- Loan Product Record 2: Used Car Loan ---
    new LoanProduct
    {
        LoanProductId = Guid.Parse("lp1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        LoanProductName = "CarLoan-Used",
        LoanProductDescription = "Secured loan for pre-owned vehicles, up to 90% financing, maximum term 5 years.",
        IsActive = true
    }
);


            b.Entity<LoanAccount>().HasData(
    // --- Loan Account Record 1: Active Loan, Current Status ---
    new LoanAccount
    {
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-111111111111"), // Used in EMIPlan seed
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"), // Application 1
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-333333333333"), // Customer 1

        CurrentPaymentStatus = LoanPaymentStatus.Active,
        DisbursementDate = new DateTime(2025, 02, 01),
        DaysPastDue = 0,
        LastStatusUpdate = new DateTime(2025, 10, 25),

        TotalLoanAmount = 500000.00m,
        TotalPaidPrincipal = 150000.00m,
        TotalPaidInterest = 45000.00m,
        PrincipalRemaining = 350000.00m, // 500000 - 150000
        LastPaymentDate = new DateTime(2025, 10, 25),
        TotalLateFeePaidAmount = 0.00m
    },

    // --- Loan Account Record 2: Closed Loan, Fully Paid Off ---
    new LoanAccount
    {
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-222222222222"), // Used in EMIPlan seed
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"), // Application 2
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-444444444444"), // Customer 2

        CurrentPaymentStatus = LoanPaymentStatus.FullyPaid,
        DisbursementDate = new DateTime(2023, 05, 10),
        DaysPastDue = 0,
        LastStatusUpdate = new DateTime(2024, 05, 10), // Date loan was closed

        TotalLoanAmount = 50000.00m,
        TotalPaidPrincipal = 50000.00m, // Fully paid
        TotalPaidInterest = 2847.40m, // From EMIPlan seed
        PrincipalRemaining = 0.00m,
        LastPaymentDate = new DateTime(2024, 05, 10),
        TotalLateFeePaidAmount = 150.00m // Example of paid fee
    }
);


            b.Entity<LoanOriginationWorkflow>().HasData(
    // --- Workflow Record 1: A completed step for the Approved Loan (Application 1) ---
    new LoanOriginationWorkflow
    {
        WorkflowId = Guid.Parse("w1f2w3f4-5w6w-7w8w-9a0a-111111111111"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        StepName = ManagerEnum.Review,
        StepStatus = StepStatus.Completed,
        CompletionDate = new DateTime(2025, 01, 20, 14, 30, 00, DateTimeKind.Utc),
        ManagerId = Guid.Parse("m1m2m3m4-n1n2-o1o2-p1p2-111111111111"),
        ManagerNotes = "Applicant's credit score is 780. Approved with standard rate."
    },

    // --- Workflow Record 2: An in-progress step for the Submitted Loan (Application 2) ---
    new LoanOriginationWorkflow
    {
        WorkflowId = Guid.Parse("w1f2w3f4-5w6w-7w8w-9a0a-222222222222"),
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        StepName = ManagerEnum.DocumentVerificationI,
        StepStatus = StepStatus.InProgress,
        CompletionDate = null,
        ManagerId = Guid.Parse("m1m2m3m4-n1n2-o1o2-p1p2-222222222222"),
        ManagerNotes = "Awaiting physical verification report for employment details."
    }
);


            b.Entity<User>().HasData(
    // --- User Record 1: Customer (Linked to Application 1) ---
    new User
    {
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        FullName = "Alex Johnson",
        Email = "alex.johnson@customer.com",
        PasswordHash = "hashedpassword_alex",
        Roles = UserEnums.Customer,
        Status = UserStatus.Active,
        CreatedAt = new DateTime(2025, 01, 01, 10, 00, 00, DateTimeKind.Utc)
    },
    // --- User Record 2: Customer (Linked to Application 2) ---
    new User
    {
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        FullName = "Emma Brown",
        Email = "emma.brown@customer.com",
        PasswordHash = "hashedpassword_emma",
        Roles = UserEnums.Customer,
        Status = UserStatus.Active,
        CreatedAt = new DateTime(2025, 10, 01, 15, 00, 00, DateTimeKind.Utc)
    },
    // --- User Record 3: Manager 1 (Linked to Workflow) ---
    new User
    {
        UserId = Guid.Parse("m1m2m3m4-n1n2-o1o2-p1p2-111111111111"),
        FullName = "Alice Smith (Credit)",
        Email = "alice.smith@manager.com",
        PasswordHash = "hashedpassword_alice",
        Roles = UserEnums.Manager,
        Status = UserStatus.Active,
        CreatedAt = new DateTime(2024, 01, 15, 08, 00, 00, DateTimeKind.Utc)
    },
    // --- User Record 4: Manager 2 (Linked to Workflow) ---
    new User
    {
        UserId = Guid.Parse("m1m2m3m4-n1n2-o1o2-p1p2-222222222222"),
        FullName = "Bob Davis (Verification)",
        Email = "bob.davis@manager.com",
        PasswordHash = "hashedpassword_bob",
        Roles = UserEnums.Manager,
        Status = UserStatus.Active,
        CreatedAt = new DateTime(2024, 02, 20, 09, 30, 00, DateTimeKind.Utc)
    }
);


            b.Entity<PaymentTransaction>().HasData(
    // --- Transaction Record 1: Successful EMI Payment ---
    new PaymentTransaction
    {
        TransactionId = Guid.Parse("pt1d2d3d-4e5e-6f7f-8a9a-111111111111"),
        EMIId = Guid.Parse("e0e1e2e3-f1f2-g1g2-h1h2-111111111111"), // Linked to Active EMI Plan
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-111111111111"), // Linked to Active Loan Account
        Amount = 11122.22m, // Full EMI amount
        PaymentDate = new DateTime(2025, 10, 25, 12, 00, 00, DateTimeKind.Utc),
        PaymentMethod = PaymentMethod.UPI,
        TransactionReference = "UPI-TXN-1234567890",
        Status = PaymentStatus.Success,
        CreatedAt = new DateTime(2025, 10, 25, 12, 00, 00, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 10, 25, 12, 00, 15, DateTimeKind.Utc),
        IsActive = true
    },

    // --- Transaction Record 2: Failed Payment Attempt ---
    new PaymentTransaction
    {
        TransactionId = Guid.Parse("pt1d2d3d-4e5e-6f7f-8a9a-222222222222"),
        EMIId = Guid.Parse("e0e1e2e3-f1f2-g1g2-h1h2-111111111111"),
        LoanAccountId = Guid.Parse("l1l2l3l4-m1m2-n1n2-o1o2-111111111111"),
        Amount = 11122.22m,
        PaymentDate = new DateTime(2025, 10, 24, 09, 30, 00, DateTimeKind.Utc),
        PaymentMethod = PaymentMethod.NetBanking,
        TransactionReference = "NB-TXN-0987654321",
        Status = PaymentStatus.Failed,
        CreatedAt = new DateTime(2025, 10, 24, 09, 30, 00, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 10, 24, 09, 30, 20, DateTimeKind.Utc),
        IsActive = true
    }
);


            b.Entity<Notification>().HasData(
    // --- Notification Record 1: Unread Notification for Customer 1 (Alex) ---
    new Notification
    {
        NotificationId = Guid.Parse("n1t2i3f4-5c6a-7t8i-9o0n-111111111111"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-555555555555"),
        Title = "Loan Disbursed Successfully",
        Message = "Your Personal Loan application (ID: ...00001) has been approved and the funds have been disbursed to your account.",
        IsRead = false,
        CreatedAt = new DateTime(2025, 02, 02, 09, 30, 00, DateTimeKind.Utc)
    },

    // --- Notification Record 2: Read Notification for Customer 2 (Emma) ---
    new Notification
    {
        NotificationId = Guid.Parse("n1t2i3f4-5c6a-7t8i-9o0n-222222222222"),
        UserId = Guid.Parse("u1u2u3u4-v1v2-w1w2-x1x2-666666666666"),
        Title = "Payment Due Reminder",
        Message = "A payment of ₹4,403.95 is due on your Loan Account (...22222) in 5 days.",
        IsRead = true,
        CreatedAt = new DateTime(2025, 10, 15, 14, 00, 00, DateTimeKind.Utc)
    }
);



            //junction table
            b.Entity<ApplicationDocumentLink>().HasData(
    // Record 1: Link Application 1 (Approved Loan) to Document 1 (Utility Bill)
    new ApplicationDocumentLink
    {
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        DocumentId = Guid.Parse("d1d2d3d4-e1e2-f1f2-0a0b-111111111111"),
        DocumentRequirementType = "AddressProof-UtilityBill",
        LinkedAt = DateTime.UtcNow.AddDays(-5)
    },

    // Record 2: Link Application 2 (Submitted Loan) to Document 2 (Pay Stub)
    new ApplicationDocumentLink
    {
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000002"),
        DocumentId = Guid.Parse("d1d2d3d4-e1e2-f1f2-0a0b-222222222222"),
        DocumentRequirementType = "IncomeProof-PayStub",
        LinkedAt = DateTime.UtcNow.AddDays(-3)
    }
);


            b.Entity<LoanApplicant>().HasData(
    // Record 1: Customer 1 (Alex Johnson) is the Primary Applicant for Loan 1
    new LoanApplicant
    {
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-333333333333"), // Alex Johnson
        ApplicantRole = ApplicantRole.Primary,
        AddedDate = DateTime.UtcNow.AddDays(-10)
    },

    // Record 2: Customer 2 (Emma Brown) is the Co-Applicant for the SAME Loan 1
    new LoanApplicant
    {
        LoanApplicationBaseId = Guid.Parse("a0a1a2a3-b1b2-c1c2-d1d2-000000000001"),
        CustomerId = Guid.Parse("c1c2c3c4-f3f4-e3e4-d3d4-444444444444"), // Emma Brown
        ApplicantRole = ApplicantRole.CoBorrower,
        AddedDate = DateTime.UtcNow.AddDays(-10)
    }
);
        }
    }
}