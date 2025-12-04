using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class VehicleInformation
    {
        [Key]
        public int VehicleInformationId { get; set; }
        [Required]
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string VehicleType { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = null!;
        [MaxLength(100)]
        public string? Variant { get; set; }
        [Required]
        public int ManufacturingYear { get; set; }
        [Required]
        [MaxLength(50)]
        public string VehicleCondition { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExShowroomPrice { get; set; }
        public LoanApplicationBase? LoanApplicationBase { get; set; }
    }
}
