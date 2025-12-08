using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Http;
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

        // Document Upload
        [Required]
        public List<DocumentUploadDTO> Documents { get; set; } = new();

        // Personal Details
        [Required]
        public PersonalDetailsDTO PersonalDetails { get; set; } = null!;

        // Address Information
        [Required]
        public AddressInformationDTO AddressInformation { get; set; } = null!;

        // Family & Emergency Details
        [Required]
        public FamilyEmergencyDetailsDTO FamilyEmergencyDetails { get; set; } = null!;

        // Declaration
        [Required]
        public DeclarationDTO Declaration { get; set; } = null!;
    }

    public class DocumentUploadDTO
    {
        [Required, MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        public DocumentType DocumentType { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }

    public class PersonalDetailsDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required, MaxLength(100)]
        public string DistrictOfBirth { get; set; } = null!;

        [Required, RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format.")]
        [MaxLength(10)]
        public string PANNumber { get; set; } = null!;

        [Required]
        public EducationQualification EducationQualification { get; set; }

        [Required]
        public ResidentialStatus ResidentialStatus { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public IFormFile SignatureImage { get; set; } = null!;

        [Required]
        public IFormFile IDProofImage { get; set; } = null!;
    }

    public class AddressInformationDTO
    {
        [Required, MaxLength(250)]
        public string PresentAddress { get; set; } = null!;

        [Required, MaxLength(250)]
        public string PermanentAddress { get; set; } = null!;

        [Required, MaxLength(100)]
        public string District { get; set; } = null!;

        [Required]
        public IndianStates State { get; set; }

        [Required, MaxLength(10)]
        public string ZipCode { get; set; } = null!;

        [EmailAddress, MaxLength(100)]
        public string? EmailId { get; set; }

        [Required, Phone, MaxLength(15)]
        public string MobileNumber1 { get; set; } = null!;

        [Phone, MaxLength(15)]
        public string? MobileNumber2 { get; set; }
    }

    public class FamilyEmergencyDetailsDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string RelationshipWithApplicant { get; set; } = null!;

        [Required, Phone]
        public string MobileNumber { get; set; } = null!;

        [Required, MaxLength(250)]
        public string Address { get; set; } = null!;
    }

    public class DeclarationDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, Range(0, double.MaxValue, ErrorMessage = "Amount must be non-negative")]
        public decimal Amount { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; } = null!;

        [Required, MaxLength(250)]
        public string Purpose { get; set; } = null!;
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