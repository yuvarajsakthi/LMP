using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class AddressInformation
    {
        public Guid AddressInformationId {  get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public string PresentAddress { get; set; } = null!;
        public string PermanentAddress { get; set; } = null!;
        public string District {  get; set; } = null!;
        public string Country { get; set; } = null!;
        public string EmailId { get; set; } = null!;
        public int MobileNumber1 { get; set; } 
        public int MobileNumber2 { get; set;}

        public IndianStates State { get; set; }

        public string ZipCode { get; set; } = null!;

    }
}