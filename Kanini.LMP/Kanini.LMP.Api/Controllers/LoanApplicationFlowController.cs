using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LoanApplicationFlowController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;

        public LoanApplicationFlowController(ILoanApplicationService loanApplicationService)
        {
            _loanApplicationService = loanApplicationService;
        }

        [HttpPost("personal/{customerId}")]
        public async Task<ActionResult<ApiResponse<PersonalLoanApplicationDTO>>> CreatePersonalLoan(int customerId, PersonalLoanApplicationCreateDTO dto)
        {
            try
            {
                var created = await _loanApplicationService.CreatePersonalLoanAsync(dto, customerId);
                return Ok(ApiResponse<PersonalLoanApplicationDTO>.SuccessResponse(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<PersonalLoanApplicationDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<PersonalLoanApplicationDTO>.ErrorResponse("Failed to create personal loan"));
            }
        }

        [HttpPost("home/{customerId}")]
        public async Task<ActionResult<ApiResponse<HomeLoanApplicationDTO>>> CreateHomeLoan(int customerId, HomeLoanApplicationCreateDTO dto)
        {
            try
            {
                var created = await _loanApplicationService.CreateHomeLoanAsync(dto, customerId);
                return Ok(ApiResponse<HomeLoanApplicationDTO>.SuccessResponse(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<HomeLoanApplicationDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<HomeLoanApplicationDTO>.ErrorResponse("Failed to create home loan"));
            }
        }

        [HttpPost("vehicle/{customerId}")]
        public async Task<ActionResult<ApiResponse<VehicleLoanApplicationDTO>>> CreateVehicleLoan(int customerId, VehicleLoanApplicationCreateDTO dto)
        {
            try
            {
                var created = await _loanApplicationService.CreateVehicleLoanAsync(dto, customerId);
                return Ok(ApiResponse<VehicleLoanApplicationDTO>.SuccessResponse(created));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<VehicleLoanApplicationDTO>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<VehicleLoanApplicationDTO>.ErrorResponse("Failed to create vehicle loan"));
            }
        }

        [HttpPut("{loanId}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateLoanStatus(int loanId, [FromBody] ApplicationStatus status)
        {
            try
            {
                var updated = await _loanApplicationService.UpdateLoanStatusAsync(loanId, status);
                return Ok(ApiResponse<object>.SuccessResponse(new { loanId, status = updated.Status }));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update loan status"));
            }
        }

        [HttpDelete("{loanId}")]
        public async Task<ActionResult<ApiResponse<object>>> WithdrawLoan(int loanId)
        {
            try
            {
                await _loanApplicationService.UpdateLoanStatusAsync(loanId, ApplicationStatus.Disbursed);
                return Ok(ApiResponse<object>.SuccessResponse(new { message = "Loan withdrawn successfully" }));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
            }
            catch (Exception)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Failed to withdraw loan"));
            }
        }
    }
}
