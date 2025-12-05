using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class PersonalDetails
    {
        [Key]
        public int PersonalDetailsId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;
        
        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        // Personal info
        [Required]
        [MaxLength(100)]

        public string FullName { get; set; } = null!;
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        [MaxLength(100)]
        public string DistrictOfBirth { get; set; } = null!;
        [Required]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Invalid PAN format.")]
        [MaxLength(10)]
        public string PANNumber { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public EducationQualification EducationQualification { get; set; }
        [Required]
        [MaxLength(50)]
        public ResidentialStatus ResidentialStatus { get; set; }
        [Required]
        public Gender Gender { get; set; }
        // Digital images
        [Required]
        [Column(TypeName = "varbinary(max)")]

        public byte[] SignatureImage { get; set; } = null!;
        [Required]
        [Column(TypeName = "varbinary(max)")]
        public byte[] IDProofImage { get; set; } = null!;

    }
}