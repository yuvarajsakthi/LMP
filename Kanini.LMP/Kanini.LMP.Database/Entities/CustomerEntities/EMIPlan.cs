using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class EMIPlan
    {
        public Guid EMIId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(PersonalLoanApplication))]
        public Guid LoanAppicationId { get; set; }
        public decimal PrincipleAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInerestPaid { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public EMIPlanStatus Status { get; set; } = EMIPlanStatus.Active;
        public bool IsCompleted { get; set; }
    }
}
