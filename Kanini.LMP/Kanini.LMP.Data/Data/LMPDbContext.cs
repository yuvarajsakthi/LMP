using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Data
{
    public class LMPDbContext : DbContext
    {
        public LMPDbContext(DbContextOptions<LMPDbContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<EMIPlan> EMIPlans { get; set; }
        public DbSet<LoanApplicationBase> loanApplicationBases { get; set; }
        public DbSet<HomeLoanApplication> HomeLoanApplications { get; set; }
        public DbSet<PersonalLoanApplication> PersonalLoanApplications { get; set; }
        public DbSet<VehicleLoanApplication> VehicleLoanApplications { get; set; }
        public DbSet<AddressInformation> AddressInformation { get; set; }
        public DbSet<Declaration> Declarations { get; set; }
        public DbSet<DocumentUpload> DocumentUploads { get; set; }
        public DbSet<FamilyEmergencyDetails> FamilyEmergencyDetails { get; set; }
        public DbSet<LoanDetails> LoanDetails { get; set; }
        public DbSet<PersonalDetails> PersonalDetails { get; set; }
        public DbSet<BuilderInformation> BuilderInformation { get; set; }
        public DbSet<HomeLoanDetails> HomeLoanDetails { get; set; }
        public DbSet<PropertyDetails> PropertyDetails { get; set; }
        public DbSet<EmploymentDetails> EmploymentDetails { get; set; }
        public DbSet<FinancialInformation> FinancialInformation { get; set; }
        public DbSet<DealerInformation> DealerInformation { get; set; }
        public DbSet<VehicleLoanDetails> VehicleLoanDetails { get; set; }
        public DbSet<VehicleInformation> VehicleInformation { get; set; }
        public DbSet<LoanProduct> LoanProducts { get; set; }
        public DbSet<LoanAccount> LoanAccounts { get; set; }
        public DbSet<LoanOriginationWorkflow> LoanOriginationWorkflows { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


        }
    }
}
