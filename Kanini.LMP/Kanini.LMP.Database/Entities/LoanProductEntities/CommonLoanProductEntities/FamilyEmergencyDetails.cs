using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class FamilyEmergencyDetails
    {
        public Guid FamilyEmergencyDetailsId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string RelationshipWithApplicant { get; set; } = null!;
        public int MobileNumber { get; set; }
        public string Address { get; set; } = null!;

    }
}