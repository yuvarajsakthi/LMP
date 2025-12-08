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
        public OwnershipType OwnershipType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PropertyCost { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DownPayment { get; set; }

        [Required]
        public LoanPurposeHome LoanPurpose { get; set; }
        
    }
}

