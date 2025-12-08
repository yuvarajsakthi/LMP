using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class EMIPlan
    {
        [Key]
        public int EMIId { get; set; }
        [Required]
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Principle amount must be greater than zero")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipleAmount { get; set; }
        [Required]
        [Range(1, 360, ErrorMessage = "Term must be between 1 and 360 months")]
        public int TermMonths { get; set; }
        [Required]
        [Range(0.1, 100, ErrorMessage = "Interest rate must be valid")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal RateOfInterest { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyEMI { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalInterestPaid { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRepaymentAmount { get; set; }
        [Required]
        public EMIPlanStatus Status { get; set; } = EMIPlanStatus.Active;
        [Required]
        public bool IsCompleted { get; set; }
        public int PaidInstallments { get; set; } = 0;
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public LoanApplicationBase? LoanApplicationBase { get; set; }
        public Customer? Customer { get; set; }
    }
}
