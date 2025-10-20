using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.ManagerEntities
{
    internal class LoanAccount
    {
        [Key]
        public Guid LoanAccountId { get; set; } = Guid.NewGuid();

        // Link to the completed Loan Application (1:1 relationship)
        [ForeignKey(nameof(LoanApplication))]
        public Guid LoanApplicationId { get; set; }
        

        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        [Required]
        public decimal TotalLoanAmount { get; set; }

        [Required]
        public decimal PrincipalRemaining { get; set; }

        [Required]
        public LoanPaymentStatus CurrentPaymentStatus { get; set; } // The source for the 'Loan Status Distribution' chart

        // Date the loan was officially disbursed/opened
        public DateTime DisbursementDate { get; set; }

        public int DaysPastDue { get; set; } = 0;

        public DateTime LastStatusUpdate { get; set; }
    }
}
