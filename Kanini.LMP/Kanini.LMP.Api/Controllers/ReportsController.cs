using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.ManagerEntitiesDtos;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.LoanAnalystics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize(Roles = ApplicationConstants.Roles.Manager)]
    public class ReportsController : ControllerBase
    {
        private readonly IManagerAnalyticsService _analyticsService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IManagerAnalyticsService analyticsService, ILogger<ReportsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet(ApiConstants.Routes.ReportsController.LoanPerformance)]
        public async Task<ActionResult<ApiResponse<LoanPerformanceReportDto>>> GetLoanPerformanceReport(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanPerformanceReport, from, to);

                var report = await _analyticsService.GetLoanPerformanceReportAsync(from, to);
                
                _logger.LogInformation(ApplicationConstants.Messages.LoanPerformanceReportCompleted);
                return Ok(ApiResponse<LoanPerformanceReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanPerformanceReportFailed);
                return BadRequest(ApiResponse<LoanPerformanceReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.LoanPerformanceReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.RiskAssessment)]
        public async Task<ActionResult<ApiResponse<RiskAssessmentReportDto>>> GetRiskAssessmentReport(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRiskAssessmentReport, from, to);

                var report = await _analyticsService.GetRiskAssessmentReportAsync(from, to);
                
                _logger.LogInformation(ApplicationConstants.Messages.RiskAssessmentReportCompleted);
                return Ok(ApiResponse<RiskAssessmentReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RiskAssessmentReportFailed);
                return BadRequest(ApiResponse<RiskAssessmentReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.RiskAssessmentReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.Compliance)]
        public async Task<ActionResult<ApiResponse<ComplianceReportDto>>> GetComplianceReport()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingComplianceReport);
                var report = await _analyticsService.GetComplianceReportAsync();
                _logger.LogInformation(ApplicationConstants.Messages.ComplianceReportCompleted);
                return Ok(ApiResponse<ComplianceReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ComplianceReportFailed);
                return BadRequest(ApiResponse<ComplianceReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.ComplianceReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.CustomerAnalytics)]
        public async Task<ActionResult<ApiResponse<object>>> GetCustomerAnalyticsDashboard()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingCustomerAnalytics);
                
                var loanStatusDistribution = await _analyticsService.GetLoanStatusDistributionAsync();
                var loanTypeDistribution = await _analyticsService.GetLoanTypeDistributionAsync();
                var monthlyDistribution = await _analyticsService.GetMonthlyDistributionAsync();
                var dailyEngagement = await _analyticsService.GetDailyEngagementAsync();

                var analytics = new
                {
                    LoanStatusDistribution = loanStatusDistribution,
                    LoanTypeDistribution = loanTypeDistribution,
                    MonthlyTrends = monthlyDistribution,
                    DailyEngagement = dailyEngagement,
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation(ApplicationConstants.Messages.CustomerAnalyticsCompleted);
                return Ok(ApiResponse<object>.SuccessResponse(analytics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.CustomerAnalyticsFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerAnalyticsFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.AnalyticsDashboard)]
        public async Task<ActionResult<ApiResponse<object>>> GetAnalyticsDashboard()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingAnalyticsDashboard);
                
                var overallMetrics = await _analyticsService.GetOverallMetricsAsync();
                var applicationStatus = await _analyticsService.GetApplicationStatusSummaryAsync();
                var applicationTrends = await _analyticsService.GetApplicationTrendsAsync();
                var typePerformance = await _analyticsService.GetApplicationTypePerformanceAsync();
                var newApplications = await _analyticsService.GetNewApplicationsSummaryAsync();

                var dashboard = new
                {
                    OverallMetrics = overallMetrics,
                    ApplicationStatusSummary = applicationStatus,
                    ApplicationTrends = applicationTrends,
                    LoanTypePerformance = typePerformance,
                    NewApplicationsSummary = newApplications,
                    GeneratedAt = DateTime.UtcNow
                };

                _logger.LogInformation(ApplicationConstants.Messages.AnalyticsDashboardCompleted);
                return Ok(ApiResponse<object>.SuccessResponse(dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.AnalyticsDashboardFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.AnalyticsDashboardFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.LoanPerformanceSP)]
        public async Task<ActionResult<ApiResponse<LoanPerformanceReportDto>>> GetLoanPerformanceReportViaSP(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-3);
                var to = toDate ?? DateTime.UtcNow;
                var report = await _analyticsService.GetLoanPerformanceReportViaSPAsync(from, to);
                return Ok(ApiResponse<LoanPerformanceReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanPerformanceReportFailed);
                return BadRequest(ApiResponse<LoanPerformanceReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.LoanPerformanceReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.RiskAssessmentSP)]
        public async Task<ActionResult<ApiResponse<RiskAssessmentReportDto>>> GetRiskAssessmentReportViaSP(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;
                var report = await _analyticsService.GetRiskAssessmentReportViaSPAsync(from, to);
                return Ok(ApiResponse<RiskAssessmentReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RiskAssessmentReportFailed);
                return BadRequest(ApiResponse<RiskAssessmentReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.RiskAssessmentReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.ComplianceSP)]
        public async Task<ActionResult<ApiResponse<ComplianceReportDto>>> GetComplianceReportViaSP()
        {
            try
            {
                var report = await _analyticsService.GetComplianceReportViaSPAsync();
                return Ok(ApiResponse<ComplianceReportDto>.SuccessResponse(report));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ComplianceReportFailed);
                return BadRequest(ApiResponse<ComplianceReportDto>.ErrorResponse(ApplicationConstants.ErrorMessages.ComplianceReportFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.CustomerAnalyticsSP)]
        public async Task<ActionResult<ApiResponse<object>>> GetCustomerAnalyticsDashboardViaSP()
        {
            try
            {
                var analytics = await _analyticsService.GetCustomerAnalyticsViaSPAsync();
                return Ok(ApiResponse<object>.SuccessResponse(analytics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.CustomerAnalyticsFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerAnalyticsFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.ReportsController.Export)]
        public async Task<ActionResult<ApiResponse<object>>> ExportReport(string reportType,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string format = "json")
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingReportExport, reportType, format);
                
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;

                object report = reportType.ToLower() switch
                {
                    "loan-performance" => await _analyticsService.GetLoanPerformanceReportAsync(from, to),
                    "risk-assessment" => await _analyticsService.GetRiskAssessmentReportAsync(from, to),
                    "compliance" => await _analyticsService.GetComplianceReportAsync(),
                    _ => throw new ArgumentException(ApplicationConstants.ErrorMessages.InvalidReportType)
                };

                var exportData = new
                {
                    ReportType = reportType,
                    Format = format,
                    GeneratedAt = DateTime.UtcNow,
                    Data = report
                };

                _logger.LogInformation(ApplicationConstants.Messages.ReportExportCompleted, reportType);
                return Ok(ApiResponse<object>.SuccessResponse(exportData));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.ReportExportFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ReportExportFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.ReportExportFailed));
            }
        }
    }
}