using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class PersonalLoanApplication : LoanApplicationBase
    {
        [Required]
        public EmploymentType EmploymentType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }
        [Required]
        public int WorkExperienceYears { get; set; }

        [Required]
        public LoanPurposePersonal LoanPurpose { get; set; }
    }
}
