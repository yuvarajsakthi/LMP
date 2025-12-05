using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.Authentication
{
    public class CustomerRegistrationDTO
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required, RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$"), MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string PANNumber { get; set; } = null!;

        [Required]
        public string AadhaarNumber { get; set; } = null!;

        [Required, Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
    }
}
