using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.CustomerDtos
{
    public class CustomerCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
        public string AadhaarNumber { get; set; } = null!;
        public string PANNumber { get; set; } = null!;
    }

    public class CustomerDTO
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Occupation { get; set; } = null!;
        public decimal AnnualIncome { get; set; }
        public decimal EligibilityScore { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
        public string AadhaarNumber { get; set; } = null!;
        public string PANNumber { get; set; } = null!;
        public int Age { get; set; }
    }

    public class CustomerUpdateDTO
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }

        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
        public string? AadhaarNumber { get; set; }
        public string? PANNumber { get; set; }
    }

    public class CustomerResponseDTO
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Occupation { get; set; } = null!;
        public decimal AnnualIncome { get; set; }
        public decimal EligibilityScore { get; set; }
        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
        public int Age { get; set; }
    }
}