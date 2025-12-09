using Kanini.LMP.Database.Entities.LoanProductEntities;
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

        public Customer Customer { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(LoanProduct))]
        public LoanType LoanProductType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(1, double.MaxValue, ErrorMessage = "Requested amount must be greater than zero")]
        public decimal RequestedAmount { get; set; }

        [Required]
        [Range(1, 360, ErrorMessage = "Tenure must be between 1 and 360 months")]
        public int TenureMonths { get; set; }

        [Required]
        public DateTime AppliedDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MonthlyInstallment { get; set; }

        [MaxLength(255)]
        public string? DocumentName { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? DocumentData { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? SignatureImage { get; set; }

        [Required]
        [MaxLength(100)]
        public string RelationFullName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string RelationshipWithApplicant { get; set; } = null!;
        [Required]
        [Phone]
        public int MobileNumber { get; set; }
        [Required]
        [MaxLength(250)]
        public string RelationAddress { get; set; } = null!;
        
        [Required]
        [MaxLength(250)]
        public string PresentAddress { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string PermanentAddress { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string District { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public IndianStates State { get; set; }

        [Required]
        [MaxLength(10)]
        public int ZipCode { get; set; }

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        
        [MaxLength(500)]
        public string? RejectionReason { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
    }
}
