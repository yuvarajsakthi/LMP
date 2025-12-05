using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class PersonalLoanApplicationCreateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TenureMonths { get; set; }

        [Required]
        public decimal RequestedLoanAmount { get; set; }

        [Required]
        public EmploymentType EmploymentType { get; set; }

        [Required]
        public decimal MonthlyIncome { get; set; }

        [Required]
        public int WorkExperienceYears { get; set; }

        [Required]
        public LoanPurposePersonal LoanPurpose { get; set; }
    }

    public class PersonalLoanApplicationDTO
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
        public EmploymentType EmploymentType { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int WorkExperienceYears { get; set; }
        public LoanPurposePersonal LoanPurpose { get; set; }
    }

    public class PersonalLoanApplicationUpdateDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }

        public ApplicationStatus? Status { get; set; }
        public decimal? InterestRate { get; set; }
        public string? RejectionReason { get; set; }
        public bool? IsActive { get; set; }
    }

    public class PersonalLoanApplicationResponseDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public EmploymentType EmploymentType { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int WorkExperienceYears { get; set; }
        public LoanPurposePersonal LoanPurpose { get; set; }
    }
}