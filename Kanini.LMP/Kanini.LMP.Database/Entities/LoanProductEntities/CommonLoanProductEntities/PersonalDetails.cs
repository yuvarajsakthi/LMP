using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class PersonalDetails
    {
        public Guid PersonalDetailsId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public string FullName { get; set; } = null!;
        public DateOnly DateOfBirth { get; set; }
        public string DistrictOfBirth { get; set; } = null!;
        public string CountryOfBirth { get; set; } = null!;
        public string PANNumber { get; set; } = null!;
        public string EducationQualification { get; set; } = null!;
        public string ResidentialStatus { get; set; } = null!;
        public Gender Gender { get; set; }

    }
}