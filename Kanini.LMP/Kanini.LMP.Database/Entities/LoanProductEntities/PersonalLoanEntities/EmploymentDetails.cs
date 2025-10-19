using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities
{
    public class EmploymentDetails
    {
        public Guid EmploymentDetailsId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string Designation {  get; set; } = null!;
        public int Experience { get; set; }
        public string EmailId { get; set; } = null!;
        public string OfficeAddress { get; set; } = null!;
    }
}