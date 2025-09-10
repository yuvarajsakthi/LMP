using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.CustomerEntities
{
    public class DocumentUpload
    {
        [Key]
        public Guid DocumentId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId {  get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public string? DocumentPath { get; set; }
        public DateTime UploadedAt { get; set; }

    }

}
