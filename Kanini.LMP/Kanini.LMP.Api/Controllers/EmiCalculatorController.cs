using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.EmiCalculatorController)]
    [ApiController]
    [Authorize]
    public class EmiCalculatorController : ControllerBase
    {
        private readonly IEmiCalculatorService _emiCalculatorService;
        private readonly ILogger<EmiCalculatorController> _logger;

        public EmiCalculatorController(IEmiCalculatorService emiCalculatorService, ILogger<EmiCalculatorController> logger)
        {
            _emiCalculatorService = emiCalculatorService;
            _logger = logger;
        }

        [HttpPost(ApplicationConstants.Routes.Calculate)]
        public async Task<ActionResult<EMIPlanDTO>> CalculateEmi([FromBody] CalculateEmiRequest request)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.EMICalculationRequested, request.PrincipalAmount, request.TermMonths);

                var result = await _emiCalculatorService.CalculateEmiAsync(request.PrincipalAmount, request.InterestRate, request.TermMonths);

                _logger.LogInformation(ApplicationConstants.Messages.EMICalculationCompleted, result.MonthlyEMI);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.EMICalculationFailed });
            }
        }

        [HttpPost("create")]
        public async Task<ActionResult<EMIPlanDTO>> CreateEmiPlan([FromBody] EMIPlanCreateDTO createDto)
        {
            var result = await _emiCalculatorService.CreateEmiPlanAsync(createDto);
            return Ok(result);
        }

        [HttpGet("{emiId}")]
        public async Task<ActionResult<EMIPlanDTO>> GetEmiPlan(int emiId)
        {
            var result = await _emiCalculatorService.GetEmiPlanByIdAsync(emiId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("loan/{loanApplicationId}")]
        public async Task<ActionResult<IEnumerable<EMIPlanDTO>>> GetEmiPlansByLoan(int loanApplicationId)
        {
            var result = await _emiCalculatorService.GetEmiPlansByLoanApplicationAsync(loanApplicationId);
            return Ok(result);
        }

        [HttpGet(ApplicationConstants.Routes.Dashboard)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer)]
        public async Task<IActionResult> GetEMIDashboard()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.EMIDashboardRequested);

                var customerIdClaim = User.FindFirst(ApplicationConstants.Claims.CustomerId)?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.CustomerIdNotFound);
                    return Unauthorized(new { message = ApplicationConstants.ErrorMessages.CustomerIdNotFound });
                }

                var dashboard = await _emiCalculatorService.GetCustomerEMIDashboardAsync(customerId);
                if (dashboard == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.NoActiveEMIFound, customerId);
                    return NotFound(new { message = ApplicationConstants.ErrorMessages.NoActiveEMIFound });
                }

                _logger.LogInformation(ApplicationConstants.Messages.EMIDashboardCompleted, customerId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIDashboardFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.EMIDashboardFailed });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = ApplicationConstants.Roles.Customer)]
        public async Task<IActionResult> GetAllEMIs()
        {
            try
            {
                var customerIdClaim = User.FindFirst(ApplicationConstants.Claims.CustomerId)?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized(new { message = ApplicationConstants.ErrorMessages.CustomerIdNotFound });
                }

                var emis = await _emiCalculatorService.GetAllCustomerEMIsAsync(customerId);
                return Ok(emis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIDashboardFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.EMIDashboardFailed });
            }
        }

        [HttpGet("{emiId}/schedule")]
        public async Task<IActionResult> GetEMISchedule(int emiId)
        {
            var schedule = await _emiCalculatorService.GenerateEMIScheduleAsync(emiId);
            return Ok(schedule);
        }

        [HttpPost("{emiId}/prepayment")]
        public async Task<IActionResult> CalculatePrepayment(int emiId, [FromBody] PrepaymentRequest request)
        {
            var calculation = await _emiCalculatorService.CalculatePrepaymentAsync(emiId, request.PrepaymentAmount);
            return Ok(calculation);
        }

        [HttpGet("{emiId}/latefee")]
        public async Task<IActionResult> CalculateLateFee(int emiId)
        {
            var lateFee = await _emiCalculatorService.CalculateLateFeeAsync(emiId, DateTime.UtcNow);
            return Ok(new { LateFee = lateFee });
        }

        [HttpPost("restructure/calculate")]
        public async Task<IActionResult> CalculateRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            var result = await _emiCalculatorService.CalculateEMIRestructureAsync(restructureDto);
            return Ok(result);
        }

        [HttpPost(ApplicationConstants.Routes.RestructureApply)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<IActionResult> ApplyRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.EMIRestructureRequested, restructureDto.EMIId);

                var result = await _emiCalculatorService.ApplyEMIRestructureAsync(restructureDto);

                _logger.LogInformation(ApplicationConstants.Messages.EMIRestructureCompleted, restructureDto.EMIId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIRestructureFailed, restructureDto.EMIId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.EMIRestructureFailed });
            }
        }

        [HttpGet("{emiId}/complete-details")]
        public async Task<IActionResult> GetCompleteEMIDetails(int emiId)
        {
            var result = await _emiCalculatorService.GetCompleteEMIDetailsAsync(emiId);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost("calculate-sp")]
        public async Task<ActionResult<EMIPlanDTO>> CalculateEmiViaSP([FromBody] CalculateEmiRequest request)
        {
            try
            {
                var result = await _emiCalculatorService.CalculateEmiViaSPAsync(request.PrincipalAmount, request.InterestRate, request.TermMonths);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EMI calculation via SP failed");
                return BadRequest(new { message = "EMI calculation failed" });
            }
        }

        [HttpGet("{emiId}/schedule-sp")]
        public async Task<IActionResult> GetEMIScheduleViaSP(int emiId)
        {
            var schedule = await _emiCalculatorService.GenerateEMIScheduleViaSPAsync(emiId);
            return Ok(schedule);
        }
    }

    public class CalculateEmiRequest
    {
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TermMonths { get; set; }
    }

    public class PrepaymentRequest
    {
        public decimal PrepaymentAmount { get; set; }
    }
}