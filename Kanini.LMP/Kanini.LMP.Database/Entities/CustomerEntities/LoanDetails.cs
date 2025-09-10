using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities
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
    }
}
