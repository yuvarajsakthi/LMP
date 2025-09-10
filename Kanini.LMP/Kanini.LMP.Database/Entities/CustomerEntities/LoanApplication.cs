using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class LoanApplication
    {
        [Key]
        public Guid LoanApplicationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(LoanProduct))]
        public string LoanProductType { get; set; } = null!;

        public LoanDetails LoanDetails { get; set; } = null!;
        public PersonalDetails PersonalDetails { get; set; } = null!;
        public AddressInformation AddressInformation { get; set; } = null!;
        public FamilyEmergencyDetails FamilyEmergencyDetails { get; set; } = null!;
        public EmploymentDetails EmploymentDetails { get; set; } = null!;
        public FinancialInformation FinancialInformation { get; set; } = null!;
        public Declaration Declaration { get; set; } = null!;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum ApplicationStatus
    {
        Draft,
        LoanDetailsCompleted,
        DocumentUploaded,
        PersonalDetailsCompleted,
        AddressCompleted,
        FamilyDetailsCompleted,
        EmploymentCompleted,
        FinancialCompleted,
        DeclarationCompleted,
        Submitted
    }

}
