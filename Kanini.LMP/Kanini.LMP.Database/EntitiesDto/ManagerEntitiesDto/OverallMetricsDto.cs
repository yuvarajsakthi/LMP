using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto
{
    internal class OverallMetricsDto
    {
        public int TotalApplications { get; set; }
        public decimal AppSuccessRate { get; set; }  // (Total Approved / Total Submitted)
        public decimal CurrentAppRate { get; set; }
        public decimal AppPendingRate { get; set; } //review or pending
    }
}
