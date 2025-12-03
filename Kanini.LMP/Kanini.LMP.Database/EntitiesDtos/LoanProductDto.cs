using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Database.EntitiesDtos
{
    public class LoanProductDto
    {
        public int LoanProductId { get; set; }
        public LoanType LoanType { get; set; }
        public bool IsActive { get; set; }
    }
}
