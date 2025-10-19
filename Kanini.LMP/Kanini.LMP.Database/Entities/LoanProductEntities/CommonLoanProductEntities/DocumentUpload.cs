using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class DocumentUpload
    {
        [Key]
        public Guid DocumentId { get; set; } = Guid.NewGuid();

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string DocumentName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DocumentType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "varbinary(max)")]
        public byte[]? DocumentData { get; set; }

        // Navigation property
        public virtual User? User { get; set; }
    }
}
