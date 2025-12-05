using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class AddressInformation
    {
        [Key]
        public int AddressInformationId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }

        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string PresentAddress { get; set; } = null!;

        [Required]
        [MaxLength(250)]
        public string PermanentAddress { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string District { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(50)")]
        public IndianStates State { get; set; }

        [Required]
        [MaxLength(10)]
        public string ZipCode { get; set; } = null!;

        // Contact details
        [EmailAddress]
        [MaxLength(100)]
        public string? EmailId { get; set; }

        [Required]
        [Phone]
        [MaxLength(15)]
        public string MobileNumber1 { get; set; } = null!;

        [Phone]
        [MaxLength(15)]
        public string? MobileNumber2 { get; set; }
    }
}
