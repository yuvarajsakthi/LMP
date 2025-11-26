using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanCentricView;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.CustomerScape;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDtos.ManagerEntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class ManagerAnalyticsService : IManagerAnalyticsService
    {
        private readonly IManagerAnalyticsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ManagerAnalyticsService> _logger;

        public ManagerAnalyticsService(IManagerAnalyticsRepository repository, IUnitOfWork unitOfWork, ILogger<ManagerAnalyticsService> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Manager Dashboard Methods
        public async Task<OverallMetricsDto> GetOverallMetricsAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingOverallMetrics);

                var totalApplications = await _repository.GetTotalApplicationsCountAsync();
                var approvedApplications = await _repository.GetApprovedApplicationsCountAsync();
                var pendingApplications = await _repository.GetPendingWorkflowsCountAsync();

                var successRate = totalApplications > 0 ? (decimal)approvedApplications / totalApplications * 100 : 0;
                var pendingRate = totalApplications > 0 ? (decimal)pendingApplications / totalApplications * 100 : 0;

                var result = new OverallMetricsDto
                {
                    TotalApplications = totalApplications,
                    AppSuccessRate = Math.Round(successRate, 1),
                    CurrentAppRate = Math.Round(successRate, 1),
                    AppPendingRate = Math.Round(pendingRate, 1)
                };

                _logger.LogInformation(ApplicationConstants.Messages.OverallMetricsCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.OverallMetricsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.OverallMetricsFailed);
            }
        }

        public async Task<IEnumerable<ApplicationStatusSummaryDto>> GetApplicationStatusSummaryAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationStatusSummary);

                var statusCounts = await _repository.GetApplicationStatusCountsAsync();
                var statusList = statusCounts.ToList();
                var totalCount = statusList.Sum(s => s.Count);

                var result = statusList.Select(s => new ApplicationStatusSummaryDto
                {
                    StatusName = s.Status.ToString(),
                    ApplicationCount = s.Count,
                    Percentage = totalCount > 0 ? Math.Round((decimal)s.Count / totalCount * 100, 1) : 0
                });

                _logger.LogInformation(ApplicationConstants.Messages.ApplicationStatusSummaryCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
            }
        }

        public async Task<IEnumerable<ApplicationTrendDto>> GetApplicationTrendsAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationTrends);

                var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));
                var applications = await _repository.GetApplicationsByDateRangeAsync(thirtyDaysAgo, DateOnly.FromDateTime(DateTime.Now));

                var weeklyData = applications
                    .GroupBy(app =>
                    {
                        var daysDiff = app.SubmissionDate.DayNumber - thirtyDaysAgo.DayNumber;
                        return daysDiff / 7;
                    })
                    .Select(g => new ApplicationTrendDto
                    {
                        PeriodLabel = $"Week {g.Key + 1}",
                        PeriodStartDate = thirtyDaysAgo.AddDays(g.Key * 7).ToDateTime(TimeOnly.MinValue),
                        MetricValue = g.Count()
                    })
                    .OrderBy(x => x.PeriodStartDate)
                    .ToList();

                _logger.LogInformation(ApplicationConstants.Messages.ApplicationTrendsCompleted);
                return weeklyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
            }
        }

        public async Task<IEnumerable<ApplicationTypePerformanceDto>> GetApplicationTypePerformanceAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanTypePerformance);

                var thisMonth = DateTime.Now.Month;
                var lastMonth = DateTime.Now.AddMonths(-1).Month;
                var currentYear = DateTime.Now.Year;

                var loanTypes = await _repository.GetLoanTypePerformanceAsync(thisMonth, lastMonth, currentYear);

                var result = loanTypes.Select(lt => new ApplicationTypePerformanceDto
                {
                    LoanTypeName = lt.LoanType,
                    ThisMonthValue = lt.ThisMonthTotal > 0 ? Math.Round((decimal)lt.ThisMonthApproved / lt.ThisMonthTotal * 100, 1) : 0,
                    LastMonthValue = lt.LastMonthTotal > 0 ? Math.Round((decimal)lt.LastMonthApproved / lt.LastMonthTotal * 100, 1) : 0
                });

                _logger.LogInformation(ApplicationConstants.Messages.LoanTypePerformanceCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
            }
        }

        public async Task<IEnumerable<ApplicationTypePerformanceDto>> GetApplicationTypePerformanceViaSPAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanTypePerformanceSP);

                var thisMonth = DateTime.Now.Month;
                var lastMonth = DateTime.Now.AddMonths(-1).Month;
                var currentYear = DateTime.Now.Year;

                var loanTypes = await _repository.GetLoanTypePerformanceViaSPAsync(thisMonth, lastMonth, currentYear);

                var result = loanTypes.Select(lt => new ApplicationTypePerformanceDto
                {
                    LoanTypeName = lt.LoanType,
                    ThisMonthValue = lt.ThisMonthTotal > 0 ? Math.Round((decimal)lt.ThisMonthApproved / lt.ThisMonthTotal * 100, 1) : 0,
                    LastMonthValue = lt.LastMonthTotal > 0 ? Math.Round((decimal)lt.LastMonthApproved / lt.LastMonthTotal * 100, 1) : 0
                });

                _logger.LogInformation(ApplicationConstants.Messages.LoanTypePerformanceSPCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
            }
        }

        public async Task<IEnumerable<ApplicationStatusSummaryDto>> GetApplicationStatusSummaryViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing application status summary via stored procedure");
                var statusSummary = await _repository.GetApplicationStatusSummaryViaSPAsync();
                _logger.LogInformation("Application status summary stored procedure completed");
                return statusSummary.Select(s => new ApplicationStatusSummaryDto
                {
                    StatusName = s.StatusName,
                    ApplicationCount = s.ApplicationCount,
                    Percentage = s.Percentage
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
            }
        }

        public async Task<IEnumerable<ApplicationTrendDto>> GetApplicationTrendsViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing application trends via stored procedure");
                var trends = await _repository.GetApplicationTrendsViaSPAsync();
                _logger.LogInformation("Application trends stored procedure completed");
                return trends.Select(t => new ApplicationTrendDto
                {
                    PeriodLabel = t.PeriodLabel,
                    PeriodStartDate = t.PeriodStartDate,
                    MetricValue = t.MetricValue
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
            }
        }

        public async Task<OverallMetricsDto> GetOverallMetricsViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing overall metrics via stored procedure");
                var metrics = await _repository.GetOverallMetricsViaSPAsync();
                _logger.LogInformation("Overall metrics stored procedure completed");
                return new OverallMetricsDto
                {
                    TotalApplications = metrics.TotalApplications,
                    AppSuccessRate = metrics.AppSuccessRate,
                    CurrentAppRate = metrics.CurrentAppRate,
                    AppPendingRate = metrics.AppPendingRate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.OverallMetricsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.OverallMetricsFailed);
            }
        }

        public async Task<NewApplicationsSummaryDto> GetNewApplicationsSummaryViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing new applications summary via stored procedure");
                var summary = await _repository.GetNewApplicationsSummaryViaSPAsync(7);
                _logger.LogInformation("New applications summary stored procedure completed");
                return new NewApplicationsSummaryDto
                {
                    TotalNewApplications = summary.TotalNewApplications,
                    ApprovedCount = summary.ApprovedCount,
                    PendingCount = summary.PendingCount,
                    ApprovedPercentage = summary.ApprovedPercentage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
            }
        }

        // Reports Stored Procedures
        public async Task<LoanPerformanceReportDto> GetLoanPerformanceReportViaSPAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Processing loan performance report via stored procedure");
                var result = await _repository.GetLoanPerformanceReportViaSPAsync(fromDate, toDate);
                _logger.LogInformation("Loan performance report stored procedure completed");
                return new LoanPerformanceReportDto
                {
                    ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    TotalActiveLoans = result.TotalActiveLoans,
                    TotalLoanAmount = result.TotalLoanAmount,
                    OnTimePaymentRate = result.OnTimePaymentRate,
                    OverdueLoans = result.OverdueLoans,
                    DefaultLoans = result.DefaultLoans,
                    PortfolioPerformance = new List<PortfolioPerformanceDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve loan performance report via SP");
                throw new Exception("Failed to retrieve loan performance report via SP");
            }
        }

        public async Task<RiskAssessmentReportDto> GetRiskAssessmentReportViaSPAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Processing risk assessment report via stored procedure");
                var result = await _repository.GetRiskAssessmentReportViaSPAsync(fromDate, toDate);
                _logger.LogInformation("Risk assessment report stored procedure completed");
                return new RiskAssessmentReportDto
                {
                    ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    TotalApplications = result.TotalApplications,
                    HighRiskApplications = result.HighRiskApplications,
                    MediumRiskApplications = result.MediumRiskApplications,
                    LowRiskApplications = result.LowRiskApplications,
                    AverageCreditScore = result.AverageCreditScore,
                    DefaultRate = result.DefaultRate,
                    RiskDistribution = new List<RiskCategoryDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve risk assessment report via SP");
                throw new Exception("Failed to retrieve risk assessment report via SP");
            }
        }

        public async Task<ComplianceReportDto> GetComplianceReportViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing compliance report via stored procedure");
                var result = await _repository.GetComplianceReportViaSPAsync();
                _logger.LogInformation("Compliance report stored procedure completed");
                return new ComplianceReportDto
                {
                    ReportDate = DateTime.UtcNow,
                    ComplianceScore = result.ComplianceScore,
                    KYCCompletionRate = result.KYCCompletionRate,
                    DocumentVerificationRate = result.DocumentVerificationRate,
                    RegulatoryViolations = result.RegulatoryViolations,
                    AuditFindings = new List<AuditFindingDto>(),
                    ComplianceIssues = new List<ComplianceIssueDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve compliance report via SP");
                throw new Exception("Failed to retrieve compliance report via SP");
            }
        }

        public async Task<object> GetCustomerAnalyticsViaSPAsync()
        {
            try
            {
                _logger.LogInformation("Processing customer analytics via stored procedure");
                var result = await _repository.GetCustomerAnalyticsViaSPAsync();
                _logger.LogInformation("Customer analytics stored procedure completed");
                return new
                {
                    LoanStatusDistribution = result.LoanStatusDistribution,
                    LoanTypeDistribution = result.LoanTypeDistribution,
                    MonthlyTrends = result.MonthlyTrends,
                    DailyEngagement = result.DailyEngagement,
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve customer analytics via SP");
                throw new Exception("Failed to retrieve customer analytics via SP");
            }
        }

        public async Task<NewApplicationsSummaryDto> GetNewApplicationsSummaryAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingNewApplicationsSummary);

                var sevenDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                var newApplications = await _repository.GetNewApplicationsAsync(sevenDaysAgo);
                var applicationsList = newApplications.ToList();

                var approvedCount = applicationsList.Count(app => app.Status == ApplicationStatus.Approved);
                var pendingCount = applicationsList.Count(app => app.Status == ApplicationStatus.Submitted);
                var totalNew = applicationsList.Count;

                var result = new NewApplicationsSummaryDto
                {
                    TotalNewApplications = totalNew,
                    ApprovedCount = approvedCount,
                    PendingCount = pendingCount,
                    ApprovedPercentage = totalNew > 0 ? Math.Round((decimal)approvedCount / totalNew * 100, 1) : 0
                };

                _logger.LogInformation(ApplicationConstants.Messages.NewApplicationsSummaryCompleted);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
            }
        }

        // Applied Loans Methods
        public async Task<IEnumerable<AppliedLoanListDto>> GetAppliedLoansAsync()
        {
            try
            {
                var applicationsWithDetails = await _repository.GetAppliedLoansWithDetailsAsync();

                var applications = applicationsWithDetails.Select(x => new AppliedLoanListDto
                {
                    ApplicationId = x.Application.LoanApplicationBaseId,
                    CustomerId = x.Customer.CustomerId,
                    CustomerFullName = x.User.FullName,
                    LoanProductType = x.Application.LoanProductType,
                    ApplicationNumber = $"APP-{x.Application.LoanApplicationBaseId}",
                    RequestedLoanAmount = 0, // Will be populated from LoanDetails if needed
                    SubmissionDate = x.Application.SubmissionDate.ToDateTime(TimeOnly.MinValue),
                    Status = x.Application.Status
                })
                .OrderByDescending(x => x.SubmissionDate);

                return applications;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve applied loans");
                throw new Exception("Failed to retrieve applied loans");
            }
        }

        public async Task<LoanApprovalDetailDto> GetLoanApprovalDetailAsync(int applicationId)
        {
            try
            {
                var application = await _repository.GetApplicationByIdAsync(applicationId);
                if (application == null) return new LoanApprovalDetailDto();

                var workflow = await _repository.GetLatestWorkflowByApplicationIdAsync(applicationId);

                return new LoanApprovalDetailDto
                {
                    LoanApplicationId = applicationId,
                    ApplicationNumber = $"APP-{applicationId}",
                    LoanProductType = application.LoanProductType,
                    CurrentStatus = application.Status,
                    CustomerFullName = "", // Will need to join with customer data
                    CustomerId = "", // Will need to join with customer data
                    CustomerOccupation = "",
                    ProfileImageBase64 = "",
                    RequestedLoanAmount = 0,
                    RequestedTenureMonths = 0,
                    EligibilityAmountCalculated = 0,
                    EligibilityMessage = "",
                    RevisedAmountOffered = 0,
                    RevisedTenureMonthsOffered = 0,
                    SignatureImageBase64 = "",
                    IDProofImageBase64 = "",
                    OriginationPipeline = new List<WorkflowStepDto>(),
                    KYCEligibilityScore = 0,
                    LoanDefaultScore = 0,
                    OverallEligibilityScore = 0,
                    ExplainerData = new ModelExplainerDataDto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve loan approval detail for application ID: {0}", applicationId);
                throw new Exception("Failed to retrieve loan approval detail");
            }
        }

        // Customer Scape Methods - Placeholder implementations
        public async Task<CustomerScapeMasterDto> GetCustomerScapeAsync(int customerId)
        {
            return new CustomerScapeMasterDto();
        }

        public async Task<CustomerProfileDto> GetCustomerProfileAsync(int customerId)
        {
            return new CustomerProfileDto();
        }

        public async Task<IEnumerable<CustomerLoanSummaryDto>> GetCustomerLoanHistoryAsync(int customerId)
        {
            return new List<CustomerLoanSummaryDto>();
        }

        // Loan Analytics Methods
        public async Task<IEnumerable<LoanStatusDistributionDto>> GetLoanStatusDistributionAsync()
        {
            try
            {
                var statusCounts = await _repository.GetStatusDistributionAsync();
                var statusList = statusCounts.ToList();
                var total = statusList.Sum(s => s.Count);

                return statusList.Select(s => new LoanStatusDistributionDto
                {
                    StatusName = s.Status.ToString(),
                    Percentage = total > 0 ? Math.Round((decimal)s.Count / total * 100, 1) : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve loan status distribution");
                throw new Exception("Failed to retrieve loan status distribution");
            }
        }

        public async Task<IEnumerable<LoanTypeDistributionDto>> GetLoanTypeDistributionAsync()
        {
            try
            {
                var typeCounts = await _repository.GetLoanTypeDistributionAsync();
                var typeList = typeCounts.ToList();
                var total = typeList.Sum(t => t.Count);

                return typeList.Select(t => new LoanTypeDistributionDto
                {
                    LoanTypeName = t.LoanType,
                    Percentage = total > 0 ? Math.Round((decimal)t.Count / total * 100, 1) : 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve loan type distribution");
                throw new Exception("Failed to retrieve loan type distribution");
            }
        }

        public async Task<IEnumerable<MonthlyDistributionDto>> GetMonthlyDistributionAsync()
        {
            try
            {
                var sixMonthsAgo = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6));
                var applications = await _repository.GetApplicationsByMonthRangeAsync(sixMonthsAgo);

                var monthlyData = applications
                    .GroupBy(app => new { app.SubmissionDate.Year, app.SubmissionDate.Month })
                    .Select(g => new MonthlyDistributionDto
                    {
                        Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                        Value = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                return monthlyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve monthly distribution");
                throw new Exception("Failed to retrieve monthly distribution");
            }
        }

        public async Task<IEnumerable<CountryDistributionDto>> GetCountryDistributionAsync()
        {
            try
            {
                // Since we don't have country data in current schema, return India-focused data
                var totalApplications = await _repository.GetTotalApplicationsCountAsync();

                return new List<CountryDistributionDto>
                {
                    new CountryDistributionDto
                    {
                        Country = "India",
                        Percentage = 100.0m
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve country distribution");
                throw new Exception("Failed to retrieve country distribution");
            }
        }

        public async Task<IEnumerable<DailyEngagementDto>> GetDailyEngagementAsync()
        {
            try
            {
                var sevenDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-7));
                var applications = await _repository.GetApplicationsByDateRangeAsync(sevenDaysAgo, DateOnly.FromDateTime(DateTime.Now));

                var dailyData = applications
                    .GroupBy(app => app.SubmissionDate)
                    .Select(g => new DailyEngagementDto
                    {
                        DayOfMonth = g.Key.Day,
                        NumberOfLoans = g.Count(app => app.Status == ApplicationStatus.Approved || app.Status == ApplicationStatus.Rejected)
                    })
                    .OrderBy(x => x.DayOfMonth)
                    .ToList();

                return dailyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve daily engagement");
                throw new Exception("Failed to retrieve daily engagement");
            }
        }

        // Loan Centric View Methods - Placeholder implementations
        public async Task<IEnumerable<LoanProductSummaryDto>> GetLoanProductSummaryAsync()
        {
            return new List<LoanProductSummaryDto>();
        }

        public async Task<LoanProductAnalysisDto> GetLoanProductAnalysisAsync(string loanProductType)
        {
            return new LoanProductAnalysisDto();
        }

        // Risk Assessment Reports
        public async Task<RiskAssessmentReportDto> GetRiskAssessmentReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var applications = await _repository.GetApplicationsWithCustomersByDateRangeAsync(
                    DateOnly.FromDateTime(fromDate), DateOnly.FromDateTime(toDate));
                var applicationsList = applications.ToList();

                var totalApplications = applicationsList.Count;
                var avgCreditScore = applicationsList.Any() ? applicationsList.Average(x => (decimal)x.Customer.CreditScore) : 0;

                // Risk categorization based on credit score
                var highRisk = applicationsList.Count(x => x.Customer.CreditScore < 550);
                var mediumRisk = applicationsList.Count(x => x.Customer.CreditScore >= 550 && x.Customer.CreditScore < 700);
                var lowRisk = applicationsList.Count(x => x.Customer.CreditScore >= 700);

                // Calculate default rate (rejected applications as proxy)
                var defaultRate = totalApplications > 0 ?
                    (decimal)applicationsList.Count(x => x.Application.Status == ApplicationStatus.Rejected) / totalApplications * 100 : 0;

                var riskDistribution = new List<RiskCategoryDto>
                {
                    new RiskCategoryDto { RiskLevel = "High Risk", Count = highRisk, Percentage = totalApplications > 0 ? (decimal)highRisk / totalApplications * 100 : 0 },
                    new RiskCategoryDto { RiskLevel = "Medium Risk", Count = mediumRisk, Percentage = totalApplications > 0 ? (decimal)mediumRisk / totalApplications * 100 : 0 },
                    new RiskCategoryDto { RiskLevel = "Low Risk", Count = lowRisk, Percentage = totalApplications > 0 ? (decimal)lowRisk / totalApplications * 100 : 0 }
                };

                return new RiskAssessmentReportDto
                {
                    ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    TotalApplications = totalApplications,
                    HighRiskApplications = highRisk,
                    MediumRiskApplications = mediumRisk,
                    LowRiskApplications = lowRisk,
                    AverageCreditScore = Math.Round(avgCreditScore, 0),
                    DefaultRate = Math.Round(defaultRate, 2),
                    RiskDistribution = riskDistribution
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve risk assessment report");
                throw new Exception("Failed to retrieve risk assessment report");
            }
        }

        // Compliance Reports
        public async Task<ComplianceReportDto> GetComplianceReportAsync()
        {
            try
            {
                var totalApplications = await _repository.GetTotalApplicationsCountAsync();
                var documentsCount = await _repository.GetDocumentLinksCountAsync();
                var verifiedDocuments = await _repository.GetVerifiedDocumentsCountAsync();

                var kycCompletionRate = totalApplications > 0 ? (decimal)documentsCount / totalApplications * 100 : 0;
                var docVerificationRate = documentsCount > 0 ? (decimal)verifiedDocuments / documentsCount * 100 : 0;

                // Mock compliance score calculation
                var complianceScore = (kycCompletionRate + docVerificationRate) / 2;

                var auditFindings = new List<AuditFindingDto>
                {
                    new AuditFindingDto
                    {
                        Category = "Documentation",
                        Description = "Missing income verification documents",
                        Severity = "Medium",
                        IdentifiedDate = DateTime.UtcNow.AddDays(-15),
                        Status = "In Progress"
                    }
                };

                return new ComplianceReportDto
                {
                    ReportDate = DateTime.UtcNow,
                    ComplianceScore = Math.Round(complianceScore, 1),
                    KYCCompletionRate = Math.Round(kycCompletionRate, 1),
                    DocumentVerificationRate = Math.Round(docVerificationRate, 1),
                    RegulatoryViolations = 0,
                    AuditFindings = auditFindings,
                    ComplianceIssues = new List<ComplianceIssueDto>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve compliance report");
                throw new Exception("Failed to retrieve compliance report");
            }
        }

        // Loan Performance Reports
        public async Task<LoanPerformanceReportDto> GetLoanPerformanceReportAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                var activeLoans = await _repository.GetActiveLoanAccountsAsync();
                var activeLoansList = activeLoans.ToList();

                var totalActiveLoans = activeLoansList.Count;
                var totalLoanAmount = activeLoansList.Sum(la => la.TotalLoanAmount);
                var overdueLoans = activeLoansList.Count(la => la.DaysPastDue > 0);
                var defaultLoans = activeLoansList.Count(la => la.CurrentPaymentStatus == LoanPaymentStatus.Defaulted);

                // Calculate on-time payment rate
                var onTimePaymentRate = totalActiveLoans > 0 ?
                    (decimal)(totalActiveLoans - overdueLoans) / totalActiveLoans * 100 : 0;

                // Portfolio performance by loan type
                var accountsWithApplications = await _repository.GetActiveLoanAccountsWithApplicationsAsync();
                var portfolioPerformance = accountsWithApplications
                    .GroupBy(x => x.Application.LoanProductType)
                    .Select(g => new PortfolioPerformanceDto
                    {
                        LoanType = g.Key,
                        ActiveLoans = g.Count(),
                        TotalAmount = g.Sum(x => x.Account.TotalLoanAmount),
                        CollectionRate = g.Count() > 0 ? (decimal)g.Count(x => x.Account.DaysPastDue == 0) / g.Count() * 100 : 0,
                        OverdueCount = g.Count(x => x.Account.DaysPastDue > 0)
                    })
                    .ToList();

                return new LoanPerformanceReportDto
                {
                    ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    TotalActiveLoans = totalActiveLoans,
                    TotalLoanAmount = totalLoanAmount,
                    OnTimePaymentRate = Math.Round(onTimePaymentRate, 1),
                    OverdueLoans = overdueLoans,
                    DefaultLoans = defaultLoans,
                    PortfolioPerformance = portfolioPerformance
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve loan performance report");
                throw new Exception("Failed to retrieve loan performance report");
            }
        }
    }
}