using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class PersonalLoanApplication : LoanApplicationBase
    {
        [Required]
        [MaxLength(100)]
        public string EmployerName { get; set; } = null!;
        [Required]
        [MaxLength(100)]
        public string Designation { get; set; } = null!;
        [Required]
        public EmploymentType EmploymentType { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyIncome { get; set; }
        [Required]
        public int WorkExperienceYears { get; set; }
        [Required]
        [MaxLength(250)]
        public string OfficeAddress { get; set; } = null!;
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RequestedLoanAmount { get; set; }
        [Required]
        [Range(1, 360)]
        public int TenureMonths { get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal? InterestRate { get; set; }
        [Required]
        public LoanPurposePersonal LoanPurpose { get; set; }
    }
}
