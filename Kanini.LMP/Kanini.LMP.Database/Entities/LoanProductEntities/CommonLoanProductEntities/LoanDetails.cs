using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class LoanDetails
    {
        [Key]
        public Guid LoanDetailsId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(LoanApplication))]
        public Guid LoanApplicationId { get; set; }
        public decimal RequestedAmount { get; set; }
        public int TenureMonths { get; set; }
        public DateTime AppliedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? MonthlyInstallment { get; set; }
    }
}
