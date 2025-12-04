using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Data
{
    public class LmpDbContext : DbContext
    {
        public LmpDbContext(DbContextOptions<LmpDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<LoanApplicationBase> LoanApplicationBases => Set<LoanApplicationBase>();
        public DbSet<PersonalLoanApplication> PersonalLoanApplications => Set<PersonalLoanApplication>();
        public DbSet<HomeLoanApplication> HomeLoanApplications => Set<HomeLoanApplication>();
        public DbSet<VehicleLoanApplication> VehicleLoanApplications => Set<VehicleLoanApplication>();
        public DbSet<DocumentUpload> DocumentUploads => Set<DocumentUpload>();
        public DbSet<LoanProduct> LoanProducts => Set<LoanProduct>();
        public DbSet<EMIPlan> EMIPlans => Set<EMIPlan>();
        public DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
        public DbSet<LoanOriginationWorkflow> LoanOriginationWorkflows => Set<LoanOriginationWorkflow>();
        public DbSet<LoanAccount> LoanAccounts => Set<LoanAccount>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<AddressInformation> AddressInformations => Set<AddressInformation>();
        public DbSet<PersonalDetails> PersonalDetails => Set<PersonalDetails>();
        public DbSet<FamilyEmergencyDetails> FamilyEmergencyDetails => Set<FamilyEmergencyDetails>();
        public DbSet<LoanDetails> LoanDetails => Set<LoanDetails>();
        public DbSet<Declaration> Declarations => Set<Declaration>();
        public DbSet<LoanApplicant> LoanApplicants => Set<LoanApplicant>();
        public DbSet<ApplicationDocumentLink> ApplicationDocumentLinks => Set<ApplicationDocumentLink>();
        public DbSet<PropertyDetails> PropertyDetails => Set<PropertyDetails>();
        public DbSet<VehicleInformation> VehicleInformations => Set<VehicleInformation>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            b.Entity<LoanApplicationBase>().ToTable("LoanApplicationBases");
            b.Entity<PersonalLoanApplication>().ToTable("PersonalLoanApplications");
            b.Entity<HomeLoanApplication>().ToTable("HomeLoanApplications");
            b.Entity<VehicleLoanApplication>().ToTable("VehicleLoanApplications");

            b.Entity<Customer>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.Customer)
                .WithMany(c => c.LoanApplications)
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<EMIPlan>()
                .HasOne(e => e.LoanApplicationBase)
                .WithMany()
                .HasForeignKey(e => e.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<EMIPlan>()
                .HasOne(e => e.Customer)
                .WithMany()
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Notification>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<LoanApplicant>()
                .HasOne(la => la.LoanApplicationBase)
                .WithMany(lab => lab.Applicants)
                .HasForeignKey(la => la.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<LoanApplicant>()
                .HasOne(la => la.Customer)
                .WithMany()
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ApplicationDocumentLink>()
                .HasOne(adl => adl.LoanApplicationBase)
                .WithMany()
                .HasForeignKey(adl => adl.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<ApplicationDocumentLink>()
                .HasOne(adl => adl.DocumentUpload)
                .WithMany()
                .HasForeignKey(adl => adl.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<PropertyDetails>()
                .HasOne(pd => pd.LoanApplicationBase)
                .WithMany()
                .HasForeignKey(pd => pd.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<VehicleInformation>()
                .HasOne(vi => vi.LoanApplicationBase)
                .WithMany()
                .HasForeignKey(vi => vi.LoanApplicationBaseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data
            b.Entity<LoanProduct>().HasData(
                new LoanProduct { LoanProductId = 1, LoanType = LoanType.Personal, IsActive = true },
                new LoanProduct { LoanProductId = 2, LoanType = LoanType.Vehicle, IsActive = true },
                new LoanProduct { LoanProductId = 3, LoanType = LoanType.Home, IsActive = true });

            b.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Manager One",
                    Email = "manager@gmail.com",
                    PasswordHash = "hashedpassword",
                    Roles = UserEnums.Manager,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 2,
                    FullName = "Customer One",
                    Email = "customer1@gmail.com",
                    PasswordHash = "hashedpassword",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 3,
                    FullName = "Customer Two",
                    Email = "customer2@gmail.com",
                    PasswordHash = "hashedpassword",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    UserId = 4,
                    FullName = "Customer Three",
                    Email = "customer3@gmail.com",
                    PasswordHash = "hashedpassword",
                    Roles = UserEnums.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });

            b.Entity<Customer>().HasData(
                new Customer
                {
                    CustomerId = 1,
                    UserId = 2,
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Gender = Gender.Male,
                    PhoneNumber = "9876543210",
                    Occupation = "Software Engineer",
                    AnnualIncome = 1200000m,
                    CreditScore = 750m,
                    ProfileImage = new byte[] { 0x01 },
                    AadhaarNumber = "123456789012",
                    PANNumber = "ABCDE1234F",
                    HomeOwnershipStatus = HomeOwnershipStatus.Owned
                },
                new Customer
                {
                    CustomerId = 2,
                    UserId = 3,
                    DateOfBirth = new DateOnly(1985, 5, 15),
                    Gender = Gender.Female,
                    PhoneNumber = "9876543211",
                    Occupation = "Business Owner",
                    AnnualIncome = 2000000m,
                    CreditScore = 800m,
                    ProfileImage = new byte[] { 0x02 },
                    AadhaarNumber = "123456789013",
                    PANNumber = "ABCDE1234G",
                    HomeOwnershipStatus = HomeOwnershipStatus.Rented
                },
                new Customer
                {
                    CustomerId = 3,
                    UserId = 4,
                    DateOfBirth = new DateOnly(1995, 10, 20),
                    Gender = Gender.Male,
                    PhoneNumber = "9876543212",
                    Occupation = "Doctor",
                    AnnualIncome = 1800000m,
                    CreditScore = 780m,
                    ProfileImage = new byte[] { 0x03 },
                    AadhaarNumber = "123456789014",
                    PANNumber = "ABCDE1234H",
                    HomeOwnershipStatus = HomeOwnershipStatus.Owned
                });
        }
    }
}
