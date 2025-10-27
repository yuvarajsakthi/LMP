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

        // FK → Linked Loan Application
        [Required]
        [ForeignKey(nameof(PersonalLoanApplication))]
        public Guid LoanApplicationId { get; set; }
        // Financial details
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Property cost must be greater than zero")]

        public decimal PropertyCost { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Down payment cannot be negative")]
        public decimal DownPayment { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested loan amount must be greater than zero")]
        public decimal RequestedLoanAmount { get; set; }

        public int TenureMonths { get; set; }
        public decimal? InterestRate { get; set; }
        public DateTime AppliedDate { get; set; }
        public LoanPurposeHome LoanPurpose { get; set; } 

    }
}
