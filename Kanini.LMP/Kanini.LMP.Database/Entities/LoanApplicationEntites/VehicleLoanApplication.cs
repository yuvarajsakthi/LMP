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
        [MaxLength(100)]
        public string Variant { get; set; } = null!;
        [Required]
        [Range(1900, 2100)]
        public int ManufacturingYear { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ExShowroomPrice { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OnRoadPrice { get; set; }
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
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
        [Required]
        [MaxLength(150)]
        public string DealerName { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string DealerAddress { get; set; } = null!;
        [Required]
        [Phone]
        [MaxLength(15)]
        public string DealerContactNumber { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string DealerEmail { get; set; } = null!;
    }
}
