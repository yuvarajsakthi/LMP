namespace Kanini.LMP.Database.EntitiesDtos
{
    public class LoanProductDto
    {
        public int LoanProductId { get; set; }
        public string LoanProductName { get; set; } = null!;
        public string LoanProductDescription { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
