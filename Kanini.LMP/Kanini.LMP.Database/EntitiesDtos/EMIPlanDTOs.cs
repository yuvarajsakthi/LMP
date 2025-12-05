using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos
{
    public class EMIPlanCreateDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal PrincipleAmount { get; set; }

        [Required]
        [Range(1, 360)]
        public int TermMonths { get; set; }

        [Required]
        [Range(0.1, 100)]
        public decimal RateOfInterest { get; set; }

        [Required]
        public decimal MonthlyEMI { get; set; }

        [Required]
        public decimal TotalInterestPaid { get; set; }

        [Required]
        public decimal TotalRepaymentAmount { get; set; }
    }

    public class EMIPlanDTO
    {
        public int EMIId { get; set; }
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public decimal PrincipleAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterestPaid { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public EMIPlanStatus Status { get; set; }
        public bool IsCompleted { get; set; }
    }

    public class EMIPlanUpdateDTO
    {
        [Required]
        public int EMIId { get; set; }

        public EMIPlanStatus? Status { get; set; }
        public bool? IsCompleted { get; set; }
    }

    public class EMIPlanResponseDTO
    {
        public int EMIId { get; set; }
        public int CustomerId { get; set; }
        public decimal PrincipleAmount { get; set; }
        public int TermMonths { get; set; }
        public decimal RateOfInterest { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterestPaid { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public EMIPlanStatus Status { get; set; }
        public bool IsCompleted { get; set; }
    }
}