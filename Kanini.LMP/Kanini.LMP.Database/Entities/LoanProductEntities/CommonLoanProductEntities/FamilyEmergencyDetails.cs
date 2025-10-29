using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class FamilyEmergencyDetails
    {
        [Key]
        public Guid FamilyEmergencyDetailsId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(LoanApplicationBase))]
        public Guid LoanApplicationBaseId { get; set; }

        // FK to User
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        // Emergency contact information
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string RelationshipWithApplicant { get; set; } = null!;
        [Required]
        [Phone]
        public int MobileNumber { get; set; }
        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = null!;

    }
}