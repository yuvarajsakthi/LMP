using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Kanini.LMP.Data.Data
{
    public class LmpDbContextFactory : IDesignTimeDbContextFactory<LmpDbContext>
    {
        public LmpDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LmpDbContext>();
            optionsBuilder.UseSqlServer("Server=tcp:kalmp.database.windows.net,1433;Initial Catalog=lmp;Persist Security Info=False;User ID=lmp;Password=Admin@2025;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            return new LmpDbContext(optionsBuilder.Options);
        }
    }
}
