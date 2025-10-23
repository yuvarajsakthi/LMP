using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
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
        [ForeignKey(nameof(PersonalLoanApplication))]
        public Guid LoanApplicationId { get; set; }

        // Link to the Customer
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        // --- Core Servicing Status ---

        [Required]
        public LoanPaymentStatus CurrentPaymentStatus { get; set; }

        public DateTime DisbursementDate { get; set; }

        public int DaysPastDue { get; set; } = 0;

        public DateTime LastStatusUpdate { get; set; }

        // --- Financial Tracking Details (For Customer Scape View) ---

        [Required]
        public decimal TotalLoanAmount { get; set; }


        public decimal TotalPaidPrincipal { get; set; } = 0m;

        public decimal TotalPaidInterest { get; set; } = 0m;

        public decimal PrincipalRemaining { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public decimal TotalLateFeePaidAmount { get; set; } = 0m;
    }
}
