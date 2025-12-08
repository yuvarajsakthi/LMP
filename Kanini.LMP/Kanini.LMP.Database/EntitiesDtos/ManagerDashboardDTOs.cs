namespace Kanini.LMP.Database.EntitiesDtos
{
    // 1. Dashboard Stats with Graph Data
    public class DashboardStatsDTO
    {
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int DisbursedApplications { get; set; }
        public decimal TotalDisbursedAmount { get; set; }
        public int ActiveLoans { get; set; }
        public List<LoanTypeDistributionDTO> LoanTypeDistribution { get; set; } = new();
        public List<MonthlyApplicationTrendDTO> MonthlyTrend { get; set; } = new();
    }

    public class LoanTypeDistributionDTO
    {
        public string LoanType { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class MonthlyApplicationTrendDTO
    {
        public string Month { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }

    // 2. Applied Loan Details
    public class LoanApplicationDetailDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string LoanType { get; set; } = string.Empty;
        public decimal RequestedAmount { get; set; }
        public int TenureMonths { get; set; }
        public decimal? InterestRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmissionDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? RejectionReason { get; set; }
        public EMIStatusDTO? EMIStatus { get; set; }
        public List<DocumentDTO> Documents { get; set; } = new();
    }

    public class EMIStatusDTO
    {
        public int EMIId { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalRepaymentAmount { get; set; }
        public decimal TotalInterestPaid { get; set; }
        public int TermMonths { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }

    public class DocumentDTO
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public byte[]? DocumentData { get; set; }
    }

    public class UpdateApplicationStatusDTO
    {
        public int LoanApplicationBaseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal? InterestRate { get; set; }
        public string? RejectionReason { get; set; }
    }

    // 3. FAQ Management
    public class ManagerFaqDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string? Answer { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AnswerFaqDTO
    {
        public int Id { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
