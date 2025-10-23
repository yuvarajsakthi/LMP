using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable
{
    public class LoanApplicant
    {
       
        [Key]
        public Guid LoanApplicationId { get; set; }

        [Key]
        public Guid CustomerId { get; set; }

        // --- Specific M:M Data ---
        public string ApplicantRole { get; set; } = "Primary"; // e.g., Primary, Co-Borrower, Guarantor
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

       
        public LoanApplication LoanApplication { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
