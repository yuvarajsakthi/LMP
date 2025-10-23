using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable
{
    /// <summary>
    /// M:M Join Table linking a specific LoanApplication to a DocumentUpload.
    /// This tracks which documents were submitted for a given application.
    /// </summary>
    public class ApplicationDocumentLink
    {
        
        [Key]
        public Guid LoanApplicationId { get; set; }

        [Key]
        public Guid DocumentId { get; set; }

     
        public string DocumentRequirementType { get; set; } = "Unknown";//e.g., 'This PDF is the Address Proof'

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

       
        public LoanApplication LoanApplication { get; set; } = null!;
        public DocumentUpload DocumentUpload { get; set; } = null!;
    }

}
