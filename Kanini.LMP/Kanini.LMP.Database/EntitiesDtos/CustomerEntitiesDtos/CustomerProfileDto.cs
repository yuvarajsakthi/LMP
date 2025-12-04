using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class CustomerProfileDto
    {
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Occupation is required.")]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required(ErrorMessage = "Annual income is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Annual income must be positive.")]
        public decimal AnnualIncome { get; set; }

        [Required(ErrorMessage = "Credit score is required.")]
        [Range(0, 900, ErrorMessage = "Credit score must be between 0 and 900.")]
        public decimal CreditScore { get; set; }

        public string? ProfileImageBase64 { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int Age { get; set; }

    }
}
