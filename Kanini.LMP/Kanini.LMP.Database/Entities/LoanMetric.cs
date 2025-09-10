using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities
{
    public class LoanMetric
    {
        [Key]
        public Guid LoanMetricId { get; set; } = Guid.NewGuid();
        public decimal TotalDisburedAmount { get; set; }
        public int TotalActiveLoan { get; set; }
        public int TotalPendingApplications { get; set; }
        public int NewCustomerCount { get; set; }
    }
}
