using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.ManagerWorkflowController)]
    [ApiController]
    [Authorize]
    public class ManagerWorkflowController : ControllerBase
    {
        private readonly IManagerWorkflowService _managerWorkflowService;
        private readonly ILogger<ManagerWorkflowController> _logger;

        public ManagerWorkflowController(IManagerWorkflowService managerWorkflowService, ILogger<ManagerWorkflowController> logger)
        {
            _managerWorkflowService = managerWorkflowService;
            _logger = logger;
        }

        [HttpGet(ApplicationConstants.Routes.PendingApplications)]
        public async Task<ActionResult<IEnumerable<AppliedLoanListDto>>> GetPendingApplications()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPendingApplications);
                var applications = await _managerWorkflowService.GetPendingApplicationsAsync();
                _logger.LogInformation("Pending applications retrieval completed");
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve pending applications");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Application)]
        public async Task<ActionResult<LoanApprovalDetailDto>> GetApplicationDetails(int applicationId)
        {
            try
            {
                _logger.LogInformation("Processing application details");
                var details = await _managerWorkflowService.GetApplicationDetailsAsync(applicationId);
                _logger.LogInformation("Application details retrieval completed");
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve application details");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.StartWorkflow)]
        public async Task<ActionResult<LoanOriginationWorkflowDTO>> StartWorkflow(int applicationId, int managerId)
        {
            try
            {
                _logger.LogInformation("Processing workflow start");
                var workflow = await _managerWorkflowService.StartWorkflowAsync(applicationId, managerId);
                _logger.LogInformation("Workflow start completed");
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start workflow");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPut(ApplicationConstants.Routes.UpdateStep)]
        public async Task<ActionResult<LoanOriginationWorkflowDTO>> UpdateWorkflowStep(
            int applicationId,
            [FromBody] UpdateStepRequest request)
        {
            try
            {
                _logger.LogInformation("Processing workflow step update");
                var workflow = await _managerWorkflowService.UpdateWorkflowStepAsync(
                    applicationId, request.StepName, request.Status, request.Notes, request.ManagerId);
                _logger.LogInformation("Workflow step update completed");
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update workflow step");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.WorkflowStatus)]
        public async Task<ActionResult<IEnumerable<WorkflowStepDto>>> GetWorkflowStatus(int applicationId)
        {
            try
            {
                _logger.LogInformation("Processing workflow status");
                var status = await _managerWorkflowService.GetWorkflowStatusAsync(applicationId);
                _logger.LogInformation("Workflow status retrieval completed");
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve workflow status");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.VerifyDocuments)]
        public async Task<ActionResult> VerifyDocuments(int applicationId, int managerId, [FromBody] string verificationNotes)
        {
            try
            {
                _logger.LogInformation("Processing document verification");
                var result = await _managerWorkflowService.VerifyDocumentsAsync(applicationId, managerId, verificationNotes);
                _logger.LogInformation("Document verification completed");
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.DocumentsVerifiedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify documents");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Approve)]
        public async Task<ActionResult> ApproveApplication(int applicationId, int managerId, [FromBody] string approvalNotes)
        {
            try
            {
                _logger.LogInformation("Processing application approval");
                var result = await _managerWorkflowService.ApproveApplicationAsync(applicationId, managerId, approvalNotes);
                _logger.LogInformation("Application approval completed");
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationApprovedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve application");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.WorkflowReject)]
        public async Task<ActionResult> RejectApplication(int applicationId, int managerId, [FromBody] string rejectionReason)
        {
            try
            {
                _logger.LogInformation("Processing application rejection");
                var result = await _managerWorkflowService.RejectApplicationAsync(applicationId, managerId, rejectionReason);
                _logger.LogInformation("Application rejection completed");
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationRejected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reject application");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Revise)]
        public async Task<ActionResult> ReviseApplication(int applicationId, int managerId, [FromBody] LoanRevisionInputDto revisionData)
        {
            try
            {
                _logger.LogInformation("Processing application revision");
                var result = await _managerWorkflowService.ReviseApplicationAsync(applicationId, revisionData, managerId);
                _logger.LogInformation("Application revision completed");
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationRevisedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revise application");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Disburse)]
        public async Task<ActionResult> DisburseApplication(int applicationId, int managerId, [FromBody] decimal disbursedAmount)
        {
            try
            {
                _logger.LogInformation("Processing application disbursement");
                var result = await _managerWorkflowService.DisburseApplicationAsync(applicationId, managerId, disbursedAmount);
                _logger.LogInformation("Application disbursement completed");
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.LoanDisbursedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disburse application");
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }
    }

    public class UpdateStepRequest
    {
        public ManagerEnum StepName { get; set; }
        public StepStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int ManagerId { get; set; }
    }
}