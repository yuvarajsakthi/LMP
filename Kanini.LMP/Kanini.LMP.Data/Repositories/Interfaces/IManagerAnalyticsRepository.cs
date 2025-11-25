using Kanini.LMP.Data.Repositories.Implementations;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Data.Repositories.Interfaces
{
    public interface IManagerAnalyticsRepository
    {
        Task<int> GetTotalApplicationsCountAsync();
        Task<int> GetApprovedApplicationsCountAsync();
        Task<int> GetPendingWorkflowsCountAsync();
        Task<IEnumerable<(ApplicationStatus Status, int Count)>> GetApplicationStatusCountsAsync();
        Task<IEnumerable<LoanApplicationBase>> GetApplicationsByDateRangeAsync(DateOnly fromDate, DateOnly toDate);
        Task<IEnumerable<(string LoanType, int ThisMonthApproved, int ThisMonthTotal, int LastMonthApproved, int LastMonthTotal)>> GetLoanTypePerformanceAsync(int thisMonth, int lastMonth, int currentYear);
        Task<IEnumerable<LoanApplicationBase>> GetNewApplicationsAsync(DateOnly fromDate);
        Task<IEnumerable<(LoanApplicationBase Application, LoanApplicant Applicant, Customer Customer, User User)>> GetAppliedLoansWithDetailsAsync();
        Task<LoanApplicationBase?> GetApplicationByIdAsync(int applicationId);
        Task<LoanOriginationWorkflow?> GetLatestWorkflowByApplicationIdAsync(int applicationId);
        Task<IEnumerable<(ApplicationStatus Status, int Count)>> GetStatusDistributionAsync();
        Task<IEnumerable<(string LoanType, int Count)>> GetLoanTypeDistributionAsync();
        Task<IEnumerable<LoanApplicationBase>> GetApplicationsByMonthRangeAsync(DateOnly fromDate);
        Task<int> GetDocumentLinksCountAsync();
        Task<int> GetVerifiedDocumentsCountAsync();
        Task<IEnumerable<LoanAccount>> GetActiveLoanAccountsAsync();
        Task<IEnumerable<(LoanAccount Account, LoanApplicationBase Application)>> GetActiveLoanAccountsWithApplicationsAsync();
        Task<IEnumerable<(LoanApplicationBase Application, LoanApplicant Applicant, Customer Customer)>> GetApplicationsWithCustomersByDateRangeAsync(DateOnly fromDate, DateOnly toDate);

        // Stored Procedure Methods
        Task<IEnumerable<LoanTypePerformanceResult>> GetLoanTypePerformanceViaSPAsync(int thisMonth, int lastMonth, int currentYear);
        Task<IEnumerable<ApplicationStatusSummaryResult>> GetApplicationStatusSummaryViaSPAsync();
        Task<IEnumerable<ApplicationTrendResult>> GetApplicationTrendsViaSPAsync();
        Task<OverallMetricsResult> GetOverallMetricsViaSPAsync();
        Task<NewApplicationsSummaryResult> GetNewApplicationsSummaryViaSPAsync(int daysBack);

        // Reports Stored Procedures
        Task<LoanPerformanceReportResult> GetLoanPerformanceReportViaSPAsync(DateTime fromDate, DateTime toDate);
        Task<RiskAssessmentReportResult> GetRiskAssessmentReportViaSPAsync(DateTime fromDate, DateTime toDate);
        Task<ComplianceReportResult> GetComplianceReportViaSPAsync();
        Task<CustomerAnalyticsResult> GetCustomerAnalyticsViaSPAsync();
    }
}