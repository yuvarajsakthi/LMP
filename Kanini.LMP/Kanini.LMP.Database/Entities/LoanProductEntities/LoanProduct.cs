using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities.LoanProductEntities
{
    public class LoanProduct
    {
        [Key]
        public Guid LoanProductId { get; set; } = Guid.NewGuid();
        public string LoanProductName { get; set; } = null!;
        public string LoanProductDescription { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}
