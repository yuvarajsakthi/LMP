using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class HomeLoanDetails
    {
        [Key]
        public Guid HomeLoanDetailsId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(PersonalLoanApplication))]
        public Guid LoanApplicationId { get; set; }

        public decimal PropertyCost { get; set; }
        public decimal DownPayment { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime AppliedDate { get; set; }
        public LoanPurposeHome LoanPurpose { get; set; } 

    }
}
