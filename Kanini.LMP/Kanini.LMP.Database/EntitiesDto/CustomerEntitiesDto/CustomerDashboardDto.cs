using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class CustomerDashboardDto
    {
        public string CustomerName { get; set; } = null!;
        public EligibilityScoreDto EligibilityScore { get; set; } = null!;
        public List<LoanApplication> RecentApplications { get; set; } = [];
        public PendingLoanDto PedningLoanAmount { get; set; } = null!;
    }
}
