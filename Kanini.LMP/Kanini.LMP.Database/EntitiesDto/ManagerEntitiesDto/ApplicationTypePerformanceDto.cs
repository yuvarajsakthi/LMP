using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto
{
    internal class ApplicationTypePerformanceDto
    {
        public string LoanTypeName { get; set; } // take from LoanProductType in model
        public decimal ThisMonthValue { get; set; } //  Approval Rate for this month
        public decimal LastMonthValue { get; set; }  // Approval Rate for last month

    }
}
