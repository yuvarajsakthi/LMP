using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto
{
    internal class NewApplicationsSummaryDto
    {
        public int TotalNewApplications { get; set; }
        public int ApprovedCount { get; set; } // Count of recently approved
        public int PendingCount { get; set; }  // Count of recently submitted/pending 
        public decimal ApprovedPercentage { get; set; } 

    }
}
