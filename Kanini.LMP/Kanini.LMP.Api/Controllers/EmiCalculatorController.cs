using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class EmiCalculatorController : ControllerBase
    {
        private readonly IEmiCalculatorService _emiCalculatorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmiCalculatorController> _logger;

        public EmiCalculatorController(IEmiCalculatorService emiCalculatorService, IUnitOfWork unitOfWork, ILogger<EmiCalculatorController> logger)
        {
            _emiCalculatorService = emiCalculatorService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.Calculate)]
        public async Task<ActionResult<ApiResponse<EMIPlanDTO>>> CalculateEmi([FromBody] CalculateEmiRequest request)
        {
            try
            {
                _logger.LogInformation("Processing EMI calculation");

                var result = await _emiCalculatorService.CalculateEmiAsync(request.PrincipalAmount, request.InterestRate, request.TermMonths);

                _logger.LogInformation("EMI calculation completed");
                return Ok(ApiResponse<EMIPlanDTO>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(ApiResponse<EMIPlanDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.EMICalculationFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.Create)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<EMIPlanDTO>>> CreateEmiPlan([FromBody] EMIPlanCreateDTO createDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var result = await _emiCalculatorService.CreateEmiPlanAsync(createDto);
                await _unitOfWork.CommitTransactionAsync();
                return Ok(ApiResponse<EMIPlanDTO>.SuccessResponse(result, ApplicationConstants.Messages.Created));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlanCreationFailed);
                return BadRequest(ApiResponse<EMIPlanDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlanCreationFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.GetById)]
        public async Task<ActionResult<ApiResponse<EMIPlanDTO>>> GetEmiPlan(int emiId)
        {
            try
            {
                var result = await _emiCalculatorService.GetEmiPlanByIdAsync(emiId);
                return result != null ? Ok(ApiResponse<EMIPlanDTO>.SuccessResponse(result)) : NotFound(ApiResponse<EMIPlanDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlanNotFound));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve EMI plan");
                return BadRequest(ApiResponse<EMIPlanDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlanRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.GetByLoan)]
        public async Task<ActionResult<ApiResponse<IEnumerable<EMIPlanDTO>>>> GetEmiPlansByLoan(int loanApplicationId)
        {
            try
            {
                var result = await _emiCalculatorService.GetEmiPlansByLoanApplicationAsync(loanApplicationId);
                return Ok(ApiResponse<IEnumerable<EMIPlanDTO>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed);
                return BadRequest(ApiResponse<IEnumerable<EMIPlanDTO>>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.Dashboard)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer)]
        public async Task<ActionResult<ApiResponse<object>>> GetEMIDashboard()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingEMIDashboard);

                var customerIdClaim = User.FindFirst(ApplicationConstants.Claims.CustomerId)?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    _logger.LogWarning("Customer ID not found in token");
                    return Unauthorized(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerIdNotFound));
                }

                var dashboard = await _emiCalculatorService.GetCustomerEMIDashboardAsync(customerId);
                if (dashboard == null)
                {
                    _logger.LogWarning("No active EMI found for customer");
                    return NotFound(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.NoActiveEMIFound));
                }

                _logger.LogInformation("EMI dashboard completed");
                return Ok(ApiResponse<object>.SuccessResponse(dashboard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIDashboardFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIDashboardFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.All)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer)]
        public async Task<ActionResult<ApiResponse<object>>> GetAllEMIs()
        {
            try
            {
                var customerIdClaim = User.FindFirst(ApplicationConstants.Claims.CustomerId)?.Value;
                if (string.IsNullOrEmpty(customerIdClaim) || !int.TryParse(customerIdClaim, out int customerId))
                {
                    return Unauthorized(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.CustomerIdNotFound));
                }

                var emis = await _emiCalculatorService.GetAllCustomerEMIsAsync(customerId);
                return Ok(ApiResponse<object>.SuccessResponse(emis));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.Schedule)]
        public async Task<ActionResult<ApiResponse<object>>> GetEMISchedule(int emiId)
        {
            try
            {
                var schedule = await _emiCalculatorService.GenerateEMIScheduleAsync(emiId);
                return Ok(ApiResponse<object>.SuccessResponse(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.Prepayment)]
        public async Task<ActionResult<ApiResponse<object>>> CalculatePrepayment(int emiId, [FromBody] PrepaymentRequest request)
        {
            try
            {
                var calculation = await _emiCalculatorService.CalculatePrepaymentAsync(emiId, request.PrepaymentAmount);
                return Ok(ApiResponse<object>.SuccessResponse(calculation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMICalculationFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.LateFee)]
        public async Task<ActionResult<ApiResponse<object>>> CalculateLateFee(int emiId)
        {
            try
            {
                var lateFee = await _emiCalculatorService.CalculateLateFeeAsync(emiId, DateTime.UtcNow);
                return Ok(ApiResponse<object>.SuccessResponse(new { LateFee = lateFee }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMICalculationFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.RestructureCalculate)]
        public async Task<ActionResult<ApiResponse<object>>> CalculateRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            try
            {
                var result = await _emiCalculatorService.CalculateEMIRestructureAsync(restructureDto);
                return Ok(ApiResponse<object>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMICalculationFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.RestructureApply)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<object>>> ApplyRestructure([FromBody] EMIRestructureDto restructureDto)
        {
            try
            {
                _logger.LogInformation("Processing EMI restructure");

                await _unitOfWork.BeginTransactionAsync();
                var result = await _emiCalculatorService.ApplyEMIRestructureAsync(restructureDto);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("EMI restructure completed");
                return Ok(ApiResponse<object>.SuccessResponse(result, ApplicationConstants.Messages.Updated));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to restructure EMI");
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIRestructureFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.CompleteDetails)]
        public async Task<ActionResult<ApiResponse<object>>> GetCompleteEMIDetails(int emiId)
        {
            try
            {
                var result = await _emiCalculatorService.GetCompleteEMIDetailsAsync(emiId);
                return result != null ? Ok(ApiResponse<object>.SuccessResponse(result)) : NotFound(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlanNotFound));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve EMI plan");
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlanRetrievalFailed));
            }
        }

        [HttpPost(ApiConstants.Routes.EmiCalculatorController.CalculateSP)]
        public async Task<ActionResult<ApiResponse<EMIPlanDTO>>> CalculateEmiViaSP([FromBody] CalculateEmiRequest request)
        {
            try
            {
                var result = await _emiCalculatorService.CalculateEmiViaSPAsync(request.PrincipalAmount, request.InterestRate, request.TermMonths);
                return Ok(ApiResponse<EMIPlanDTO>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMICalculationFailed);
                return BadRequest(ApiResponse<EMIPlanDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.EMICalculationFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.EmiCalculatorController.ScheduleSP)]
        public async Task<ActionResult<ApiResponse<object>>> GetEMIScheduleViaSP(int emiId)
        {
            try
            {
                var schedule = await _emiCalculatorService.GenerateEMIScheduleViaSPAsync(emiId);
                return Ok(ApiResponse<object>.SuccessResponse(schedule));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed));
            }
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