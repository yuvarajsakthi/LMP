using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.VehicleLoanEntities
{
    public class DealerInformation
    {
        [Key]
        public Guid DealerInformationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string DealerName { get; set; } = null!;
        public string DealerAddress { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

}
