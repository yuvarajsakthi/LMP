using Kanini.LMP.Database.Entities.CustomerEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class DealerInformation
    {
        [Key]
        public Guid DealerInformationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(LoanApplicationBase))]
        public Guid LoanApplicationBaseId { get; set; }

        // FK → Linked User
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        // Dealer details
        [Required]
        [MaxLength(150)]

        public string DealerName { get; set; } = null!;
        [Required]
        [MaxLength(250)]
        public string DealerAddress { get; set; } = null!;
        [Required]
        [Phone]
        [MaxLength(15)]
        public string ContactNumber { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;
    }

}
