using Microsoft.EntityFrameworkCore;
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

        /* ------------------------------------------------
         * Fluent API – keeps data-annotation attributes intact
         * ------------------------------------------------*/
        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            /* 1. Composite keys */
            b.Entity<LoanApplicant>().HasKey(la => new { la.LoanApplicationId, la.CustomerId });
            b.Entity<ApplicationDocumentLink>().HasKey(ad => new { ad.LoanApplicationId, ad.DocumentId });

            /* 2. TPT – table-per-type inheritance */
            b.Entity<LoanApplicationBase>().ToTable("LoanApplicationBases");
            b.Entity<PersonalLoanApplication>().ToTable("PersonalLoanApplications");
            b.Entity<HomeLoanApplication>().ToTable("HomeLoanApplications");
            b.Entity<VehicleLoanApplication>().ToTable("VehicleLoanApplications");

            /* 3. LoanApplicationBase -> LoanProduct (FK GUID) */
            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.LoanProductType)
                .WithMany()
                .HasForeignKey(la => la.LoanProductType)
                .OnDelete(DeleteBehavior.Restrict);

            /* 4. 1-to-1 details owned by LoanApplicationBase */
            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.LoanDetails)
                .WithOne()
                .HasForeignKey<LoanDetails>(ld => ld.LoanApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.PersonalDetails)
                .WithOne()
                .HasForeignKey<PersonalDetails>(pd => pd.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.AddressInformation)
                .WithOne()
                .HasForeignKey<AddressInformation>(ai => ai.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.FamilyEmergencyDetails)
                .WithOne()
                .HasForeignKey<FamilyEmergencyDetails>(fed => fed.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.EmploymentDetails)
                .WithOne()
                .HasForeignKey<EmploymentDetails>(ed => ed.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.FinancialInformation)
                .WithOne()
                .HasForeignKey<FinancialInformation>(fi => fi.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicationBase>()
                .HasOne(la => la.Declaration)
                .WithOne()
                .HasForeignKey<Declaration>(d => d.DeclarationId)
                .OnDelete(DeleteBehavior.Cascade);

            /* 5. M:N – LoanApplicant */
            b.Entity<LoanApplicant>()
                .HasOne(la => la.LoanApplication)
                .WithMany(la => la.Applicants)
                .HasForeignKey(la => la.LoanApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<LoanApplicant>()
                .HasOne(la => la.Customer)
                .WithMany(c => c.Applications)
                .HasForeignKey(la => la.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            /* 6. M:N – ApplicationDocumentLink */
            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.LoanApplication)
                .WithMany(la => la.DocumentLinks)
                .HasForeignKey(ad => ad.LoanApplicationId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<ApplicationDocumentLink>()
                .HasOne(ad => ad.DocumentUpload)
                .WithMany(d => d.ApplicationLinks)
                .HasForeignKey(ad => ad.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            /* 7. EMIPlan -> PersonalLoanApplication */
            //b.Entity<EMIPlan>()
            //    .HasOne(e => e.LoanAppicationBaseId)
            //    .WithMany()
            //    .HasForeignKey(e => e.LoanApplicationId)
            //    .OnDelete(DeleteBehavior.Cascade);

            /* 8. PaymentTransaction -> EMIPlan */
            //b.Entity<PaymentTransaction>()
            //    .HasOne(pt => pt.EMIId)
            //    .WithMany()
            //    .HasForeignKey(pt => pt.EMIId)
            //    .OnDelete(DeleteBehavior.Cascade);

            /* 9. LoanOriginationWorkflow -> LoanApplicationBase */
            //b.Entity<LoanOriginationWorkflow>()
            //    .HasOne(w => w.LoanApplicationBaseId)
            //    .WithMany()
            //    .HasForeignKey(w => w.LoanApplicationBaseId)
            //    .OnDelete(DeleteBehavior.Cascade);

            /* 10. LoanAccount -> LoanApplicationBase & Customer */
            //b.Entity<LoanAccount>()
            //    .HasOne(a => a.LoanApplicationBaseId)
            //    .WithMany()
            //    .HasForeignKey(a => a.LoanApplicationBaseId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanAccount>()
            //    .HasOne(a => a.CustomerId)
            //    .WithMany()
            //    .HasForeignKey(a => a.CustomerId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}