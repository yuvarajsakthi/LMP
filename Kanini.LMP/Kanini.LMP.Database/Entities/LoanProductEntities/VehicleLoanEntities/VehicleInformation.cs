using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class VehicleInformation
    {
        [Key]
        public Guid VehicleInformationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public VehicleType VehicleType { get; set; }  // Car, Bike, SUV, etc.
        public string Manufacturer { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Variant { get; set; } = null!;
        public int ManufacturingYear { get; set; }
        public LoanPurposeVehicle VehicleCondition { get; set; } // New or Used
        public decimal ExShowroomPrice { get; set; }
    }

}
