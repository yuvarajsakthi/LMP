using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class VehicleLoanApplicationDTO : LoanApplicationBaseDto
    {
        [Required]
        public VehicleType VehicleType { get; set; }
        [Required]
        public string Manufacturer { get; set; } = null!;
        [Required]
        public string Model { get; set; } = null!;
        [Required]
        public int ManufacturingYear { get; set; }
        [Required]
        public decimal OnRoadPrice { get; set; }
        [Required]
        public decimal DownPayment { get; set; }
        [Required]
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
        public IFormFile? DocumentUpload { get; set; }
    }

    public class UpdateVehicleLoanApplicationDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}