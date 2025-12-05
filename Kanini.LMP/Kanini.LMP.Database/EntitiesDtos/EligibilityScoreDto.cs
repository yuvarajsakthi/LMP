using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto
{
    public class EligibilityScoreDto
    {
        public int CustomerId { get; set; }
        public int LoanProductId { get; set; }
        public int CreditScore { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public int EligibilityScore { get; set; }
        public EligibilityStatus EligibilityStatus { get; set; }
        public DateTime CalculatedOn { get; set; }
    }

    public class EligibilityProfileRequest
    {
        public bool IsExistingBorrower { get; set; }
        public string? PAN { get; set; }
        public int? Age { get; set; }
        public decimal? AnnualIncome { get; set; }
        public string? Occupation { get; set; }
        public HomeOwnershipStatus? HomeOwnershipStatus { get; set; }
    }

    public class EligibilityCalculationRequest
    {
        public int CustomerId { get; set; }
        public int LoanProductId { get; set; }
    }

    public class EligibilityCheckRequest
    {
        public int CustomerId { get; set; }
        public int LoanProductId { get; set; } = 0;
    }
}