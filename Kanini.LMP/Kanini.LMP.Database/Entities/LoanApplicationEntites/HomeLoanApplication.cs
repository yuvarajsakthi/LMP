using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class HomeLoanApplication : LoanApplicationBase
    {
        [Required]
        public PropertyType PropertyType { get; set; }
        [Required]
        [MaxLength(250)]
        public string PropertyAddress { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string State { get; set; } = null!;
        [Required]
        public int ZipCode { get; set; }
        [Required]
        public OwnershipType OwnershipType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PropertyCost { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DownPayment { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequestedLoanAmount { get; set; }
        [Required]
        [Range(1, 360)]
        public int TenureMonths { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }
        [Required]
        public LoanPurposeHome LoanPurpose { get; set; }
        [Required]
        [MaxLength(100)]
        public string BuilderName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string ProjectName { get; set; } = null!;
        [Required]
        [MaxLength(50)]
        public string BuilderRegistrationNo { get; set; } = null!;
        [Required]
        [Phone]
        [MaxLength(15)]
        public string BuilderContactNumber { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string BuilderEmail { get; set; } = null!;
    }
}

