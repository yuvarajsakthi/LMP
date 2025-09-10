using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.Entities
{
    public class LoanProduct
    {
        [Key]
        public Guid LoanProductId { get; set; } = Guid.NewGuid();
        public string LoanProductName { get; set; } = null!;
        public decimal InterestRate { get; set; }
        public string InterestType { get; set; } = null!;
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public int MinTermMonths { get; set; }
        public int MaxTermMonths { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
