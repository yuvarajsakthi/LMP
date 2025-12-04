using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos
{
    public class UpdateCustomerSettingsDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [RegularExpression(@"^(\+91[-\s]?)?[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian phone number.")]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Occupation { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal AnnualIncome { get; set; }
    }
}
