using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class LoanApplicationBase
    {
        [Key]
        public int LoanApplicationBaseId { get; set; }
        
        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey(nameof(LoanProduct))]
        public LoanType LoanProductType { get; set; }

        [Required]
        public LoanDetails LoanDetails { get; set; } = null!;

        [Required]
        public DocumentUpload DocumentUpload { get; set; } = null!;
        
        [Required]
        public PersonalDetails PersonalDetails { get; set; } = null!;
        
        [Required]
        public AddressInformation AddressInformation { get; set; } = null!;
        
        [Required]
        public FamilyEmergencyDetails FamilyEmergencyDetails { get; set; } = null!;
        
        [Required]
        public Declaration Declaration { get; set; } = null!;

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;

        public Customer? Customer { get; set; }
        public ICollection<LoanApplicant>? Applicants { get; set; }
    }
}
