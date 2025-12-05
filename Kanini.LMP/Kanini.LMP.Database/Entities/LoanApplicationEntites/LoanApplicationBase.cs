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

        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }

        [Required]
        [Range(1, 360)]
        public int TenureMonths { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequestedLoanAmount { get; set; }
        
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;

        public Customer? Customer { get; set; }

    }
}
