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
                _logger.LogInformation(ApplicationConstants.Messages.PendingApplicationsCompleted);
                return Ok(applications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PendingApplicationsFailed);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Application)]
        public async Task<ActionResult<LoanApprovalDetailDto>> GetApplicationDetails(int applicationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationDetails, applicationId);
                var details = await _managerWorkflowService.GetApplicationDetailsAsync(applicationId);
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationDetailsCompleted, applicationId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationDetailsFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.StartWorkflow)]
        public async Task<ActionResult<LoanOriginationWorkflowDTO>> StartWorkflow(int applicationId, int managerId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingWorkflowStart, applicationId, managerId);
                var workflow = await _managerWorkflowService.StartWorkflowAsync(applicationId, managerId);
                _logger.LogInformation(ApplicationConstants.Messages.WorkflowStartCompleted, applicationId);
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.WorkflowStartFailed, applicationId);
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
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingWorkflowStepUpdate, applicationId);
                var workflow = await _managerWorkflowService.UpdateWorkflowStepAsync(
                    applicationId, request.StepName, request.Status, request.Notes, request.ManagerId);
                _logger.LogInformation(ApplicationConstants.Messages.WorkflowStepUpdateCompleted, applicationId);
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.WorkflowStepUpdateFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpGet(ApplicationConstants.Routes.WorkflowStatus)]
        public async Task<ActionResult<IEnumerable<WorkflowStepDto>>> GetWorkflowStatus(int applicationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingWorkflowStatus, applicationId);
                var status = await _managerWorkflowService.GetWorkflowStatusAsync(applicationId);
                _logger.LogInformation(ApplicationConstants.Messages.WorkflowStatusCompleted, applicationId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.WorkflowStatusFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.VerifyDocuments)]
        public async Task<ActionResult> VerifyDocuments(int applicationId, int managerId, [FromBody] string verificationNotes)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentVerification, applicationId);
                var result = await _managerWorkflowService.VerifyDocumentsAsync(applicationId, managerId, verificationNotes);
                _logger.LogInformation(ApplicationConstants.Messages.DocumentVerificationCompleted, applicationId);
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.DocumentsVerifiedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentVerificationFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Approve)]
        public async Task<ActionResult> ApproveApplication(int applicationId, int managerId, [FromBody] string approvalNotes)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationApproval, applicationId);
                var result = await _managerWorkflowService.ApproveApplicationAsync(applicationId, managerId, approvalNotes);
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationApprovalCompleted, applicationId);
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationApprovedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationApprovalFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.WorkflowReject)]
        public async Task<ActionResult> RejectApplication(int applicationId, int managerId, [FromBody] string rejectionReason)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationRejection, applicationId);
                var result = await _managerWorkflowService.RejectApplicationAsync(applicationId, managerId, rejectionReason);
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationRejectionCompleted, applicationId);
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationRejected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationRejectionFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Revise)]
        public async Task<ActionResult> ReviseApplication(int applicationId, int managerId, [FromBody] LoanRevisionInputDto revisionData)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationRevision, applicationId);
                var result = await _managerWorkflowService.ReviseApplicationAsync(applicationId, revisionData, managerId);
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationRevisionCompleted, applicationId);
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.ApplicationRevisedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationRevisionFailed, applicationId);
                return StatusCode(500, new { message = ApplicationConstants.ErrorMessages.InternalServerError });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Disburse)]
        public async Task<ActionResult> DisburseApplication(int applicationId, int managerId, [FromBody] decimal disbursedAmount)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingApplicationDisbursement, applicationId);
                var result = await _managerWorkflowService.DisburseApplicationAsync(applicationId, managerId, disbursedAmount);
                _logger.LogInformation(ApplicationConstants.Messages.ApplicationDisbursementCompleted, applicationId);
                return Ok(new { Success = result, Message = ApplicationConstants.ErrorMessages.LoanDisbursedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicationDisbursementFailed, applicationId);
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