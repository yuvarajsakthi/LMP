using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [ApiController]
    [Route(ApiConstants.Routes.ApiController)]
    [Authorize(Roles = ApiConstants.Roles.Manager)]
    public class ManagerDashboardController : ControllerBase
    {
        private readonly IManagerDashboardService _service;

        public ManagerDashboardController(IManagerDashboardService service)
        {
            _service = service;
        }

        [HttpGet(ApiConstants.Routes.ManagerDashboardController.Stats)]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _service.GetDashboardStatsAsync();
            return Ok(stats);
        }

        [HttpGet(ApiConstants.Routes.ManagerDashboardController.Loans)]
        public async Task<IActionResult> GetAllLoanApplications()
        {
            var applications = await _service.GetAllLoanApplicationsAsync();
            return Ok(applications);
        }

        [HttpGet(ApiConstants.Routes.ManagerDashboardController.LoanById)]
        public async Task<IActionResult> GetLoanApplicationById(int id)
        {
            var application = await _service.GetLoanApplicationByIdAsync(id);
            if (application == null) return NotFound(ApiConstants.ErrorMessages.LoanApplicationNotFound);
            return Ok(application);
        }

        [HttpGet(ApiConstants.Routes.ManagerDashboardController.LoansByStatus)]
        public async Task<IActionResult> GetLoanApplicationsByStatus(string status)
        {
            var applications = await _service.GetLoanApplicationsByStatusAsync(status);
            return Ok(applications);
        }

        [HttpPut(ApiConstants.Routes.ManagerDashboardController.UpdateStatus)]
        public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusDTO dto)
        {
            var result = await _service.UpdateApplicationStatusAsync(dto);
            if (!result) return BadRequest(ApiConstants.ErrorMessages.FailedToUpdateApplicationStatus);
            return Ok(new { message = ApiConstants.SuccessMessages.ApplicationStatusUpdatedSuccessfully });
        }

        [HttpPost(ApiConstants.Routes.ManagerDashboardController.DisburseLoan)]
        public async Task<IActionResult> DisburseLoan(int id)
        {
            var result = await _service.DisburseLoanAsync(id);
            if (!result) return BadRequest(ApiConstants.ErrorMessages.FailedToDisburseLoan);
            return Ok(new { message = ApiConstants.SuccessMessages.LoanDisbursedSuccessfully });
        }
    }
}
