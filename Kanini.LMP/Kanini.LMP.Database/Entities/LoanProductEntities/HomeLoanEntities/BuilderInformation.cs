using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities
{
    public class BuilderInformation
    {
        [Key]
        public Guid BuilderInformationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string BuilderName { get; set; } = null!;
        public string ProjectName { get; set; } = null!;
        public string BuilderRegistrationNo { get; set; } = null!;
        public string ContactNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

    }
}
