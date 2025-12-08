using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class ManagerDashboardController : ControllerBase
    {
        private readonly IManagerDashboardService _service;

        public ManagerDashboardController(IManagerDashboardService service)
        {
            _service = service;
        }

        // 1. Dashboard - Stats and Graphs
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _service.GetDashboardStatsAsync();
            return Ok(stats);
        }

        // 2. Applied Loans - View All
        [HttpGet("loans")]
        public async Task<IActionResult> GetAllLoanApplications()
        {
            var applications = await _service.GetAllLoanApplicationsAsync();
            return Ok(applications);
        }

        // 2. Applied Loans - View by ID (with EMI and Documents)
        [HttpGet("loans/{id}")]
        public async Task<IActionResult> GetLoanApplicationById(int id)
        {
            var application = await _service.GetLoanApplicationByIdAsync(id);
            if (application == null) return NotFound("Loan application not found");
            return Ok(application);
        }

        // 2. Applied Loans - Filter by Status
        [HttpGet("loans/status/{status}")]
        public async Task<IActionResult> GetLoanApplicationsByStatus(string status)
        {
            var applications = await _service.GetLoanApplicationsByStatusAsync(status);
            return Ok(applications);
        }

        // 2. Applied Loans - Update Status (Approve/Reject)
        [HttpPut("loans/status")]
        public async Task<IActionResult> UpdateApplicationStatus([FromBody] UpdateApplicationStatusDTO dto)
        {
            var result = await _service.UpdateApplicationStatusAsync(dto);
            if (!result) return BadRequest("Failed to update application status");
            return Ok(new { message = "Application status updated successfully" });
        }
    }
}
