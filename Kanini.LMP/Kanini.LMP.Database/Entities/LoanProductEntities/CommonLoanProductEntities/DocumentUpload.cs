using Kanini.LMP.Database.Entities.CustomerEntities;
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

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        [Required]
        [MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DocumentType { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "varbinary(max)")]
        public byte[]? DocumentData { get; set; }

        public virtual User? User { get; set; }
    }
}

