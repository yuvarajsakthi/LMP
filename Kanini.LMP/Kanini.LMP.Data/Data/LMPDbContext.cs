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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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


            // 8. FINAL SAFETY NET
            // Ensure all remaining relationships default to RESTRICT for safety
            foreach (var relationship in b.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }






            //BELOW FLUENT API IS NOT NEEDED AFTER REFACTORING TO TPT AND IMPROVED RELATIONSHIPS.



            ///* 3. LoanApplicationBase -> LoanProduct (FK GUID) */
            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.LoanProductType)
            //    .WithMany()
            //    .HasForeignKey(la => la.LoanProductType)
            //    .OnDelete(DeleteBehavior.Restrict);

            ///* 4. 1-to-1 details owned by LoanApplicationBase */
            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.LoanDetails)
            //    .WithOne()
            //    .HasForeignKey<LoanDetails>(ld => ld.LoanApplicationId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.PersonalDetails)
            //    .WithOne()
            //    .HasForeignKey<PersonalDetails>(pd => pd.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.AddressInformation)
            //    .WithOne()
            //    .HasForeignKey<AddressInformation>(ai => ai.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.FamilyEmergencyDetails)
            //    .WithOne()
            //    .HasForeignKey<FamilyEmergencyDetails>(fed => fed.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.EmploymentDetails)
            //    .WithOne()
            //    .HasForeignKey<EmploymentDetails>(ed => ed.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.FinancialInformation)
            //    .WithOne()
            //    .HasForeignKey<FinancialInformation>(fi => fi.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicationBase>()
            //    .HasOne(la => la.Declaration)
            //    .WithOne()
            //    .HasForeignKey<Declaration>(d => d.DeclarationId)
            //    .OnDelete(DeleteBehavior.Cascade);

            ///* 5. M:N – LoanApplicant */
            //b.Entity<LoanApplicant>()
            //    .HasOne(la => la.LoanApplication)
            //    .WithMany(la => la.Applicants)
            //    .HasForeignKey(la => la.LoanApplicationId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<LoanApplicant>()
            //    .HasOne(la => la.Customer)
            //    .WithMany(c => c.Applications)
            //    .HasForeignKey(la => la.CustomerId)
            //    .OnDelete(DeleteBehavior.Cascade);

            ///* 6. M:N – ApplicationDocumentLink */
            //b.Entity<ApplicationDocumentLink>()
            //    .HasOne(ad => ad.LoanApplication)
            //    .WithMany(la => la.DocumentLinks)
            //    .HasForeignKey(ad => ad.LoanApplicationId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //b.Entity<ApplicationDocumentLink>()
            //    .HasOne(ad => ad.DocumentUpload)
            //    .WithMany(d => d.ApplicationLinks)
            //    .HasForeignKey(ad => ad.DocumentId)
            //    .OnDelete(DeleteBehavior.Cascade);

            ///* 7. EMIPlan -> PersonalLoanApplication */
            ////b.Entity<EMIPlan>()
            ////    .HasOne(e => e.LoanAppicationBaseId)
            ////    .WithMany()
            ////    .HasForeignKey(e => e.LoanApplicationId)
            ////    .OnDelete(DeleteBehavior.Cascade);

            ///* 8. PaymentTransaction -> EMIPlan */
            ////b.Entity<PaymentTransaction>()
            ////    .HasOne(pt => pt.EMIId)
            ////    .WithMany()
            ////    .HasForeignKey(pt => pt.EMIId)
            ////    .OnDelete(DeleteBehavior.Cascade);

            ///* 9. LoanOriginationWorkflow -> LoanApplicationBase */
            ////b.Entity<LoanOriginationWorkflow>()
            ////    .HasOne(w => w.LoanApplicationBaseId)
            ////    .WithMany()
            ////    .HasForeignKey(w => w.LoanApplicationBaseId)
            ////    .OnDelete(DeleteBehavior.Cascade);

            ///* 10. LoanAccount -> LoanApplicationBase & Customer */
            ////b.Entity<LoanAccount>()
            ////    .HasOne(a => a.LoanApplicationBaseId)
            ////    .WithMany()
            ////    .HasForeignKey(a => a.LoanApplicationBaseId)
            ////    .OnDelete(DeleteBehavior.Cascade);

            ////b.Entity<LoanAccount>()
            ////    .HasOne(a => a.CustomerId)
            ////    .WithMany()
            ////    .HasForeignKey(a => a.CustomerId)
            ////    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}