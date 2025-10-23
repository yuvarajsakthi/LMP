namespace Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities
{
    public class Declaration
    {
        public Guid DeclarationId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public int Amount { get; set; }
        public string Description { get; set; } = null!;
        public string Purpose { get; set; } = null!;

    }
}
