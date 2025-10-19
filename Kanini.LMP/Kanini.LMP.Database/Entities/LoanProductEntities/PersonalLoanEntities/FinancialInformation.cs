using System.ComponentModel.DataAnnotations.Schema;

namespace Kanini.LMP.Database.Entities.LoanProductEntities.PersonalLoanEntities
{
    public class FinancialInformation
    {
        public Guid FinancialInformationId { get; set; } = Guid.NewGuid();
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public int Salary { get; set; }
        public int Rent { get; set; }
        public int PrimaryOther { get; set; }
        public int RentandUtility { get; set; }
        public int FoodandClothing { get; set; }
        public int Education { get; set; }
        public int LoanRepayment { get; set; }
        public int ExpenseOther { get; set; }

    }
}