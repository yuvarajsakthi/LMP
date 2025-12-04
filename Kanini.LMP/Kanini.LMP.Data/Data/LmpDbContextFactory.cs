using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kanini.LMP.Data.Data
{
    public class LmpDbContextFactory : IDesignTimeDbContextFactory<LmpDbContext>
    {
        public LmpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LmpDbContext>();
            optionsBuilder.UseSqlServer("Data Source=YUVARAJ;Database=LMPSharedDB;Integrated Security=True;TrustServerCertificate=True;Connection Timeout=30;");
            return new LmpDbContext(optionsBuilder.Options);
        }
    }
}
