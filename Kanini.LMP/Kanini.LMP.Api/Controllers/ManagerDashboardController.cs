using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.ManagerDashboardController)]
    [ApiController]
    [Authorize]
    public class ManagerDashboardController : ControllerBase
    {
        private readonly IManagerAnalyticsService _managerAnalyticsService;
        private readonly ILogger<ManagerDashboardController> _logger;

        public ManagerDashboardController(IManagerAnalyticsService managerAnalyticsService, ILogger<ManagerDashboardController> logger)
        {
            _managerAnalyticsService = managerAnalyticsService;
            _logger = logger;
        }

        // Removed - Frontend will compose dashboard from individual endpoints

        [HttpGet(ApplicationConstants.Routes.OverallMetrics)]
        public async Task<ActionResult<OverallMetricsDto>> GetOverallMetrics()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingOverallMetrics);
                var metrics = await _managerAnalyticsService.GetOverallMetricsAsync();
                _logger.LogInformation(ApplicationConstants.Messages.OverallMetricsCompleted);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.OverallMetricsFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.ApplicationStatus)]
        public async Task<ActionResult<IEnumerable<ApplicationStatusSummaryDto>>> GetApplicationStatusSummary()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationStatusSummary);
                var statusSummary = await _managerAnalyticsService.GetApplicationStatusSummaryAsync();
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationStatusSummaryCompleted);
                return Ok(statusSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.ApplicationTrends)]
        public async Task<ActionResult<IEnumerable<ApplicationTrendDto>>> GetApplicationTrends()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationTrends);
                var trends = await _managerAnalyticsService.GetApplicationTrendsAsync();
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationTrendsCompleted);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.LoanTypePerformance)]
        public async Task<ActionResult<IEnumerable<ApplicationTypePerformanceDto>>> GetLoanTypePerformance()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanTypePerformance);
                var performance = await _managerAnalyticsService.GetApplicationTypePerformanceAsync();
                _logger.LogInformation(ApplicationConstants.Messages.LoanTypePerformanceCompleted);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("loan-type-performance-sp")]
        public async Task<ActionResult<IEnumerable<ApplicationTypePerformanceDto>>> GetLoanTypePerformanceViaSP()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingLoanTypePerformanceSP);
                var performance = await _managerAnalyticsService.GetApplicationTypePerformanceViaSPAsync();
                _logger.LogInformation(ApplicationConstants.Messages.LoanTypePerformanceSPCompleted);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.LoanTypePerformanceFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("application-status-sp")]
        public async Task<ActionResult<IEnumerable<ApplicationStatusSummaryDto>>> GetApplicationStatusSummaryViaSP()
        {
            try
            {
                var statusSummary = await _managerAnalyticsService.GetApplicationStatusSummaryViaSPAsync();
                return Ok(statusSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationStatusSummaryFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("application-trends-sp")]
        public async Task<ActionResult<IEnumerable<ApplicationTrendDto>>> GetApplicationTrendsViaSP()
        {
            try
            {
                var trends = await _managerAnalyticsService.GetApplicationTrendsViaSPAsync();
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationTrendsFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("overall-metrics-sp")]
        public async Task<ActionResult<OverallMetricsDto>> GetOverallMetricsViaSP()
        {
            try
            {
                var metrics = await _managerAnalyticsService.GetOverallMetricsViaSPAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.OverallMetricsFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet("new-applications-sp")]
        public async Task<ActionResult<NewApplicationsSummaryDto>> GetNewApplicationsSummaryViaSP()
        {
            try
            {
                var summary = await _managerAnalyticsService.GetNewApplicationsSummaryViaSPAsync();
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.NewApplications)]
        public async Task<ActionResult<NewApplicationsSummaryDto>> GetNewApplicationsSummary()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingNewApplicationsSummary);
                var summary = await _managerAnalyticsService.GetNewApplicationsSummaryAsync();
                _logger.LogInformation(ApplicationConstants.Messages.NewApplicationsSummaryCompleted);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.NewApplicationsSummaryFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }
    }
}