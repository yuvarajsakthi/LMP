using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities
{
    public class EMIPlan
    {
        public Guid EMIId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(LoanApplication))]
        public Guid LoanAppicationId { get; set; }
        public decimal PrincipleAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInerestPaid { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public string Status { get; set; } = "Active"; // Active / Overdue / Closed
        public bool IsCompleted { get; set; }
    }
}
