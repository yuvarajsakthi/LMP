using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;

namespace Kanini.LMP.Database.EntitiesDto
{
    public class CustomerDashboardDto
    {
        public string CustomerName { get; set; } = null!;
        public int EligibilityScore { get; set; }
        public List<LoanApplication> RecentApplications { get; set; } = [];
        public decimal PedningLoanAmount { get; set; }
    }
}
