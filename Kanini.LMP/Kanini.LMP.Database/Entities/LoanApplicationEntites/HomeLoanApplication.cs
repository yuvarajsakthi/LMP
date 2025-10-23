using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.HomeLoanEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class HomeLoanApplication
    {
        [Key]
        public Guid HomeLoanApplicationId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }

        [ForeignKey(nameof(LoanProduct))]
        public string LoanProductType { get; set; } = null!;
        public LoanDetails LoanDetails { get; set; } = null!;
        public PersonalDetails PersonalDetails { get; set; } = null!;
        public AddressInformation AddressInformation { get; set; } = null!;
        public FamilyEmergencyDetails FamilyEmergencyDetails { get; set; } = null!;
        public DocumentUpload DocumentUpload { get; set; } = null!;
        public BuilderInformation BuilderInformation { get; set; } = null!;
        public HomeLoanDetails HomeLoanDetails { get; set; } = null!;
        public PropertyDetails PropertyDetails { get; set; } = null!;
        public Declaration Declaration { get; set; } = null!;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
