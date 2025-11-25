using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class ManagerAnalyticsRepository : IManagerAnalyticsRepository
    {
        private readonly LmpDbContext _context;

        public ManagerAnalyticsRepository(LmpDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalApplicationsCountAsync()
        {
            return await _context.LoanApplicationBases.CountAsync();
        }

        public async Task<int> GetApprovedApplicationsCountAsync()
        {
            return await _context.LoanApplicationBases
                .CountAsync(app => app.Status == ApplicationStatus.Approved);
        }

        public async Task<int> GetPendingWorkflowsCountAsync()
        {
            return await _context.LoanOriginationWorkflows
                .CountAsync(w => w.StepStatus == StepStatus.InProgress);
        }

        public async Task<IEnumerable<(ApplicationStatus Status, int Count)>> GetApplicationStatusCountsAsync()
        {
            return await _context.LoanApplicationBases
                .GroupBy(app => app.Status)
                .Select(g => new ValueTuple<ApplicationStatus, int>(g.Key, g.Count()))
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanApplicationBase>> GetApplicationsByDateRangeAsync(DateOnly fromDate, DateOnly toDate)
        {
            return await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= fromDate && app.SubmissionDate <= toDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<(string LoanType, int ThisMonthApproved, int ThisMonthTotal, int LastMonthApproved, int LastMonthTotal)>> GetLoanTypePerformanceAsync(int thisMonth, int lastMonth, int currentYear)
        {
            return await _context.LoanApplicationBases
                .GroupBy(app => app.LoanProductType)
                .Select(g => new ValueTuple<string, int, int, int, int>(
                    g.Key,
                    g.Count(app => app.Status == ApplicationStatus.Approved &&
                                  app.SubmissionDate.Month == thisMonth &&
                                  app.SubmissionDate.Year == currentYear),
                    g.Count(app => app.SubmissionDate.Month == thisMonth &&
                                  app.SubmissionDate.Year == currentYear),
                    g.Count(app => app.Status == ApplicationStatus.Approved &&
                                  app.SubmissionDate.Month == lastMonth),
                    g.Count(app => app.SubmissionDate.Month == lastMonth)
                ))
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanApplicationBase>> GetNewApplicationsAsync(DateOnly fromDate)
        {
            return await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= fromDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<(LoanApplicationBase Application, LoanApplicant Applicant, Customer Customer, User User)>> GetAppliedLoansWithDetailsAsync()
        {
            return await _context.LoanApplicationBases
                .Join(_context.LoanApplicants, app => app.LoanApplicationBaseId, la => la.LoanApplicationBaseId, (app, la) => new { app, la })
                .Join(_context.Customers, x => x.la.CustomerId, c => c.CustomerId, (x, c) => new { x.app, x.la, c })
                .Join(_context.Users, x => x.c.UserId, u => u.UserId, (x, u) => new { x.app, x.la, x.c, u })
                .Select(x => new ValueTuple<LoanApplicationBase, LoanApplicant, Customer, User>(x.app, x.la, x.c, x.u))
                .ToListAsync();
        }

        public async Task<LoanApplicationBase?> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.LoanApplicationBases
                .Where(app => app.LoanApplicationBaseId == applicationId)
                .FirstOrDefaultAsync();
        }

        public async Task<LoanOriginationWorkflow?> GetLatestWorkflowByApplicationIdAsync(int applicationId)
        {
            return await _context.LoanOriginationWorkflows
                .Where(w => w.LoanApplicationBaseId == applicationId)
                .OrderByDescending(w => w.CompletionDate)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<(ApplicationStatus Status, int Count)>> GetStatusDistributionAsync()
        {
            return await _context.LoanApplicationBases
                .GroupBy(app => app.Status)
                .Select(g => new ValueTuple<ApplicationStatus, int>(g.Key, g.Count()))
                .ToListAsync();
        }

        public async Task<IEnumerable<(string LoanType, int Count)>> GetLoanTypeDistributionAsync()
        {
            return await _context.LoanApplicationBases
                .GroupBy(app => app.LoanProductType)
                .Select(g => new ValueTuple<string, int>(g.Key, g.Count()))
                .ToListAsync();
        }

        public async Task<IEnumerable<LoanApplicationBase>> GetApplicationsByMonthRangeAsync(DateOnly fromDate)
        {
            return await _context.LoanApplicationBases
                .Where(app => app.SubmissionDate >= fromDate)
                .ToListAsync();
        }

        public async Task<int> GetDocumentLinksCountAsync()
        {
            return await _context.ApplicationDocumentLinks.CountAsync();
        }

        public async Task<int> GetVerifiedDocumentsCountAsync()
        {
            return await _context.ApplicationDocumentLinks
                .CountAsync(d => d.Status == DocumentStatus.Verified);
        }

        public async Task<IEnumerable<LoanAccount>> GetActiveLoanAccountsAsync()
        {
            return await _context.LoanAccounts
                .Where(la => la.CurrentPaymentStatus == LoanPaymentStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<(LoanAccount Account, LoanApplicationBase Application)>> GetActiveLoanAccountsWithApplicationsAsync()
        {
            return await _context.LoanAccounts
                .Join(_context.LoanApplicationBases, la => la.LoanApplicationBaseId, app => app.LoanApplicationBaseId, (la, app) => new { la, app })
                .Where(x => x.la.CurrentPaymentStatus == LoanPaymentStatus.Active)
                .Select(x => new ValueTuple<LoanAccount, LoanApplicationBase>(x.la, x.app))
                .ToListAsync();
        }

        public async Task<IEnumerable<(LoanApplicationBase Application, LoanApplicant Applicant, Customer Customer)>> GetApplicationsWithCustomersByDateRangeAsync(DateOnly fromDate, DateOnly toDate)
        {
            return await _context.LoanApplicationBases
                .Join(_context.LoanApplicants, app => app.LoanApplicationBaseId, la => la.LoanApplicationBaseId, (app, la) => new { app, la })
                .Join(_context.Customers, x => x.la.CustomerId, c => c.CustomerId, (x, c) => new { x.app, x.la, c })
                .Where(x => x.app.SubmissionDate >= fromDate && x.app.SubmissionDate <= toDate)
                .Select(x => new ValueTuple<LoanApplicationBase, LoanApplicant, Customer>(x.app, x.la, x.c))
                .ToListAsync();
        }

        // Stored Procedure Methods
        public async Task<IEnumerable<LoanTypePerformanceResult>> GetLoanTypePerformanceViaSPAsync(int thisMonth, int lastMonth, int currentYear)
        {
            return await _context.Database.SqlQueryRaw<LoanTypePerformanceResult>(
                "EXEC sp_GetLoanTypePerformance @ThisMonth = {0}, @LastMonth = {1}, @CurrentYear = {2}",
                thisMonth, lastMonth, currentYear).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationStatusSummaryResult>> GetApplicationStatusSummaryViaSPAsync()
        {
            return await _context.Database.SqlQueryRaw<ApplicationStatusSummaryResult>(
                "EXEC sp_GetApplicationStatusSummary").ToListAsync();
        }

        public async Task<IEnumerable<ApplicationTrendResult>> GetApplicationTrendsViaSPAsync()
        {
            return await _context.Database.SqlQueryRaw<ApplicationTrendResult>(
                "EXEC sp_GetApplicationTrends").ToListAsync();
        }

        public async Task<OverallMetricsResult> GetOverallMetricsViaSPAsync()
        {
            return await _context.Database.SqlQueryRaw<OverallMetricsResult>(
                "EXEC sp_GetOverallMetrics").FirstAsync();
        }

        public async Task<NewApplicationsSummaryResult> GetNewApplicationsSummaryViaSPAsync(int daysBack)
        {
            return await _context.Database.SqlQueryRaw<NewApplicationsSummaryResult>(
                "EXEC sp_GetNewApplicationsSummary @DaysBack = {0}", daysBack).FirstAsync();
        }

        // Reports Stored Procedures
        public async Task<LoanPerformanceReportResult> GetLoanPerformanceReportViaSPAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Database.SqlQueryRaw<LoanPerformanceReportResult>(
                "EXEC sp_GetLoanPerformanceReport @FromDate = {0}, @ToDate = {1}", fromDate, toDate).FirstAsync();
        }

        public async Task<RiskAssessmentReportResult> GetRiskAssessmentReportViaSPAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Database.SqlQueryRaw<RiskAssessmentReportResult>(
                "EXEC sp_GetRiskAssessmentReport @FromDate = {0}, @ToDate = {1}", fromDate, toDate).FirstAsync();
        }

        public async Task<ComplianceReportResult> GetComplianceReportViaSPAsync()
        {
            return await _context.Database.SqlQueryRaw<ComplianceReportResult>(
                "EXEC sp_GetComplianceReport").FirstAsync();
        }

        public async Task<CustomerAnalyticsResult> GetCustomerAnalyticsViaSPAsync()
        {
            return await _context.Database.SqlQueryRaw<CustomerAnalyticsResult>(
                "EXEC sp_GetCustomerAnalytics").FirstAsync();
        }
    }

    public class LoanTypePerformanceResult
    {
        public string LoanType { get; set; } = string.Empty;
        public int ThisMonthApproved { get; set; }
        public int ThisMonthTotal { get; set; }
        public int LastMonthApproved { get; set; }
        public int LastMonthTotal { get; set; }
    }

    public class ApplicationStatusSummaryResult
    {
        public string StatusName { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ApplicationTrendResult
    {
        public string PeriodLabel { get; set; } = string.Empty;
        public DateTime PeriodStartDate { get; set; }
        public int MetricValue { get; set; }
    }

    public class OverallMetricsResult
    {
        public int TotalApplications { get; set; }
        public decimal AppSuccessRate { get; set; }
        public decimal CurrentAppRate { get; set; }
        public decimal AppPendingRate { get; set; }
    }

    public class NewApplicationsSummaryResult
    {
        public int TotalNewApplications { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public decimal ApprovedPercentage { get; set; }
    }

    public class LoanPerformanceReportResult
    {
        public int TotalActiveLoans { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal OnTimePaymentRate { get; set; }
        public int OverdueLoans { get; set; }
        public int DefaultLoans { get; set; }
    }

    public class RiskAssessmentReportResult
    {
        public int TotalApplications { get; set; }
        public int HighRiskApplications { get; set; }
        public int MediumRiskApplications { get; set; }
        public int LowRiskApplications { get; set; }
        public decimal AverageCreditScore { get; set; }
        public decimal DefaultRate { get; set; }
    }

    public class ComplianceReportResult
    {
        public decimal ComplianceScore { get; set; }
        public decimal KYCCompletionRate { get; set; }
        public decimal DocumentVerificationRate { get; set; }
        public int RegulatoryViolations { get; set; }
    }

    public class CustomerAnalyticsResult
    {
        public string LoanStatusDistribution { get; set; } = string.Empty;
        public string LoanTypeDistribution { get; set; } = string.Empty;
        public string MonthlyTrends { get; set; } = string.Empty;
        public string DailyEngagement { get; set; } = string.Empty;
    }
}