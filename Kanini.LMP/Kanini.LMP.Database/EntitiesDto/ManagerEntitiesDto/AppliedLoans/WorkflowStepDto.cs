using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans
{
    public class WorkflowStepDto
    {
        public string StepName { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsCompleted { get; set; }
        public string StatusIndicator { get; set; }
    }
}
