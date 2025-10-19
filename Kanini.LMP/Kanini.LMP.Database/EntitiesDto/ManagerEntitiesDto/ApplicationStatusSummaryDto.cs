using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto
{
    internal class ApplicationStatusSummaryDto
    {
        public string StatusName { get; set; } //  "Closed", "Rejected"
        public int ApplicationCount { get; set; }
        public decimal Percentage { get; set; }
       
    }
}
