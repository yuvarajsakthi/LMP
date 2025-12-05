using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class VehicleLoanApplication : LoanApplicationBase
    {
        [Required]
        public VehicleType VehicleType { get; set; }
        [Required]
        [MaxLength(100)]
        public string Manufacturer { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Model { get; set; } = null!;
        [Required]
        [Range(1900, 2100)]
        public int ManufacturingYear { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OnRoadPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DownPayment { get; set; }

        [Required]
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }

    }
}
