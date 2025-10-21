using Kanini.LMP.Database.Entities.CustomerEntities;
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
    internal class LoanOriginationWorkflow
    {
        [Key]
        public Guid WorkflowId { get; set; } = Guid.NewGuid();

        // Link to the Loan Application
        [ForeignKey(nameof(LoanApplication))]
        public Guid LoanApplicationId { get; set; }

        [Required]
        public OriginationStepName StepName { get; set; }

        /// <summary>
        /// Status of the step: Completed, OnHold, Failed, InProgress.
        /// </summary>
        [Required]
        public string StepStatus { get; set; } = "InProgress";

        public DateTime? CompletionDate { get; set; }

        public Guid? ManagerId { get; set; } // Who completed the step

        public string? ManagerNotes { get; set; }
    }
}
