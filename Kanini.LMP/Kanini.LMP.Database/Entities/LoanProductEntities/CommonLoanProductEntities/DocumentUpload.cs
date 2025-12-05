using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class DocumentUpload
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey(nameof(LoanApplicationBase))]
        public int LoanApplicationBaseId { get; set; }
        public LoanApplicationBase LoanApplicationBase { get; set; } = null!;

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        [Required, MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [Required]
        public DocumentType DocumentType { get; set; }

        [Column(TypeName = "varbinary(max)")]
        public byte[]? DocumentData { get; set; }
        
        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
