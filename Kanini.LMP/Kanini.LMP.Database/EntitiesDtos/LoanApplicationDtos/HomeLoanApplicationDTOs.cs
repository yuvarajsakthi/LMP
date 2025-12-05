using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class HomeLoanApplicationCreateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TenureMonths { get; set; }

        [Required]
        public decimal RequestedLoanAmount { get; set; }

        [Required]
        public PropertyType PropertyType { get; set; }

        [Required]
        [MaxLength(250)]
        public string PropertyAddress { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;

        [Required]
        public int ZipCode { get; set; }

        [Required]
        public OwnershipType OwnershipType { get; set; }

        [Required]
        public decimal PropertyCost { get; set; }

        [Required]
        public decimal DownPayment { get; set; }

        [Required]
        public LoanPurposeHome LoanPurpose { get; set; }
    }

    public class HomeLoanApplicationDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public LoanType LoanProductType { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public int ZipCode { get; set; }
        public OwnershipType OwnershipType { get; set; }
        public decimal PropertyCost { get; set; }
        public decimal DownPayment { get; set; }
        public LoanPurposeHome LoanPurpose { get; set; }
    }

    public class HomeLoanApplicationUpdateDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }

        public ApplicationStatus? Status { get; set; }
        public decimal? InterestRate { get; set; }
        public string? RejectionReason { get; set; }
        public bool? IsActive { get; set; }
    }

    public class HomeLoanApplicationResponseDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public int ZipCode { get; set; }
        public OwnershipType OwnershipType { get; set; }
        public decimal PropertyCost { get; set; }
        public decimal DownPayment { get; set; }
        public LoanPurposeHome LoanPurpose { get; set; }
    }
}