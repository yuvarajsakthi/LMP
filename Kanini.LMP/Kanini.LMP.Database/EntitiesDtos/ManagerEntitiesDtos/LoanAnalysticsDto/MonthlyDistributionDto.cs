using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics
{
    public class MonthlyDistributionDto
    {
        /// <summary>
        /// DTO for the 'Monthly Loan Distribution' chart (Line/Bar Chart).
        /// Source: LoanApplication (group by SubmissionDate or ApprovedDate)
        /// </summary>

        public required string Month { get; set; } 
        public decimal Value { get; set; } 

    }
}
