using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities;
using Kanini.LMP.Database.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class LoanApplicationBase
    {
        // *** NOTE: This ID serves as the Primary Key for ALL inherited applications ***
        [Key]
        public Guid LoanApplicationBaseId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(LoanProduct))]
        public string LoanProductType { get; set; } = null!; // Discriminator field

        // --- Common Nested Details (1:1 relationships) ---
        public LoanDetails LoanDetails { get; set; } = null!;
        public PersonalDetails PersonalDetails { get; set; } = null!;
        public AddressInformation AddressInformation { get; set; } = null!;
        public FamilyEmergencyDetails FamilyEmergencyDetails { get; set; } = null!;
        public EmploymentDetails EmploymentDetails { get; set; } = null!;
        public FinancialInformation FinancialInformation { get; set; } = null!;
        public Declaration Declaration { get; set; } = null!;

        // --- Common Workflow Fields ---
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;
        public DateOnly SubmissionDate { get; set; }
        public DateOnly? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public bool IsActive { get; set; } = true;

        // --- Common M:M Navigation Properties (Joint Account & Documents) ---
        /// <summary>
        /// M:M Joint Accounts: Links to all participating customers (Primary, Joint).
        /// </summary>
        public ICollection<LoanApplicant> Applicants { get; set; } = new List<LoanApplicant>();

        /// <summary>
        /// M:M Document Uploads: Links to the specific files uploaded for this application.
        /// </summary>
        public ICollection<ApplicationDocumentLink> DocumentLinks { get; set; } = new List<ApplicationDocumentLink>();
    }
}
