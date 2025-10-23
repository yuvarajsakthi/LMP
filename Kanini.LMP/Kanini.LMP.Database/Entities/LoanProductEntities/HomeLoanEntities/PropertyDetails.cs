using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class PropertyDetails
    {
        public Guid PropertyDetailsId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public PropertyType PropertyType { get; set; }
        public string PropertyAddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public int ZipCode { get; set; }
        public OwnershipType OwnershipType { get; set; }

    }
}
