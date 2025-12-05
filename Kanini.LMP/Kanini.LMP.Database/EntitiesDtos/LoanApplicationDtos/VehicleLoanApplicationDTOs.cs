using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class VehicleLoanApplicationCreateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int TenureMonths { get; set; }

        [Required]
        public decimal RequestedLoanAmount { get; set; }

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
        public decimal OnRoadPrice { get; set; }

        [Required]
        public decimal DownPayment { get; set; }

        [Required]
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
    }

    public class VehicleLoanApplicationDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public LoanType LoanProductType { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; }
        public VehicleType VehicleType { get; set; }
        public string Manufacturer { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int ManufacturingYear { get; set; }
        public decimal OnRoadPrice { get; set; }
        public decimal DownPayment { get; set; }
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
    }

    public class VehicleLoanApplicationUpdateDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }

        public ApplicationStatus? Status { get; set; }
        public decimal? InterestRate { get; set; }
        public string? RejectionReason { get; set; }
        public bool? IsActive { get; set; }
    }

    public class VehicleLoanApplicationResponseDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public decimal? InterestRate { get; set; }
        public int TenureMonths { get; set; }
        public decimal RequestedLoanAmount { get; set; }
        public VehicleType VehicleType { get; set; }
        public string Manufacturer { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int ManufacturingYear { get; set; }
        public decimal OnRoadPrice { get; set; }
        public decimal DownPayment { get; set; }
        public LoanPurposeVehicle LoanPurposeVehicle { get; set; }
    }
}