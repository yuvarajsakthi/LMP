using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Data
{
    public class LMPDbContext : DbContext
    {
        public LMPDbContext(DbContextOptions<LMPDbContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PersonalLoanApplication> LoanApplications { get; set; }
        public DbSet<LoanProduct> LoanProducts { get; set; }
    }
}
