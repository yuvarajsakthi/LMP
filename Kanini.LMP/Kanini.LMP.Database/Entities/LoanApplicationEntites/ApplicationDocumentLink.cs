using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanApplicationEntites
{
    public class ApplicationDocumentLink
    {
        [Key]
        public int ApplicationDocumentLinkId { get; set; }
        [Required]
        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        [Required]
        [ForeignKey(nameof(DocumentUpload))]
        public int DocumentId { get; set; }
        [Required]
        public DocumentType DocumentRequirementType { get; set; }
        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public string? VerificationNotes { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? VerifiedBy { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
        public LoanApplicationBase? LoanApplicationBase { get; set; }
        public DocumentUpload? DocumentUpload { get; set; }
    }
}
