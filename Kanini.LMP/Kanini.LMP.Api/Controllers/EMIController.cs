using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class EMIController : ControllerBase
    {
        private readonly IEmiCalculatorService _emiService;

        public EMIController(IEmiCalculatorService emiService)
        {
            _emiService = emiService;
        }

        [HttpGet("dashboard/{customerId}")]
        public async Task<ActionResult<ApiResponse<EMIDashboardDTO>>> GetEMIDashboard(int customerId)
        {
            try
            {
                var dashboard = await _emiService.GetCustomerEMIDashboardAsync(new IdDTO { Id = customerId });
                if (dashboard == null)
                    return NotFound(ApiResponse<EMIDashboardDTO>.ErrorResponse("No active EMI plan found"));

                return Ok(ApiResponse<EMIDashboardDTO>.SuccessResponse(dashboard));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<EMIDashboardDTO>.ErrorResponse("Failed to retrieve EMI dashboard"));
            }
        }

        [HttpPost("pay/{emiId}")]
        public async Task<ActionResult<ApiResponse<object>>> PayMonthlyEMI(int emiId)
        {
            try
            {
                var result = await _emiService.PayMonthlyEMIAsync(new IdDTO { Id = emiId });
                if (!result)
                    return BadRequest(ApiResponse<object>.ErrorResponse("Payment not allowed at this time"));

                return Ok(ApiResponse<object>.SuccessResponse(new { message = "EMI paid successfully" }));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to process EMI payment"));
            }
        }

        [HttpPost("create-on-disbursement/{loanId}")]
        public async Task<ActionResult<ApiResponse<object>>> CreateEMIOnDisbursement(int loanId)
        {
            try
            {
                var emiPlan = await _emiService.CreateEmiPlanOnDisbursementAsync(new IdDTO { Id = loanId });
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "EMI plan created successfully", emiPlan }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(ex.Message));
            }
        }
    }
}
