using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class PropertyDetails
    {
        [Key]
        public int PropertyDetailsId { get; set; }
        [Required]
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public PropertyType PropertyType { get; set; }
        [Required]
        [MaxLength(500)]
        public string PropertyAddress { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string State { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string ZipCode { get; set; } = null!;
        [Required]
        public OwnershipType OwnershipType { get; set; }
        public LoanApplicationBase? LoanApplicationBase { get; set; }
    }
}
