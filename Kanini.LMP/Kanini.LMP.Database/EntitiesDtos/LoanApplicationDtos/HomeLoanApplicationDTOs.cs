using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos
{
    public class HomeLoanApplicationDTO : LoanApplicationBaseDto
    {
        [Required]
        public PropertyType PropertyType { get; set; }
        [Required]
        public string PropertyAddress { get; set; } = null!;
        [Required]
        public string City { get; set; } = null!;
        [Required]
        public OwnershipType OwnershipType { get; set; }
        [Required]
        public decimal PropertyCost { get; set; }
        [Required]
        public decimal DownPayment { get; set; }
        [Required]
        public LoanPurposeHome LoanPurpose { get; set; }
    }

    public class UpdateHomeLoanApplicationDTO
    {
        [Required]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        public ApplicationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}