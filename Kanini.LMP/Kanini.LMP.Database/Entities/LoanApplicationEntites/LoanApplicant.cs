using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class LoanApplicant
    {
        [Key]
        public int LoanApplicantId { get; set; }
        [Required]
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        [Required]
        public ApplicantRole ApplicantRole { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public LoanApplicationBase? LoanApplicationBase { get; set; }
        public Customer? Customer { get; set; }
    }
}
