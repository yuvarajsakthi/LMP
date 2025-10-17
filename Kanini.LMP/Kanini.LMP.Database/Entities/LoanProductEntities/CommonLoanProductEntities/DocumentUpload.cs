using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class DocumentUpload
    {
        [Key]
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId {  get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public DateTime UploadedAt { get; set; }

    }
}
