using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class VehicleLoanDetails
    {
        [Key]
        public Guid VehicleLoanDetailsId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(PersonalLoanApplication))]
        public Guid LoanApplicationId { get; set; }

        public decimal OnRoadPrice { get; set; }
        public decimal DownPayment { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime AppliedDate { get; set; }
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
    }
}
