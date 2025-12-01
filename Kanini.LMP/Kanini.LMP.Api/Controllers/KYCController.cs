using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.KYC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.KYCController)]
    [ApiController]
    [Authorize]
    public class KYCController : ControllerBase
    {
        private readonly IKYCService _kycService;
        private readonly ILogger<KYCController> _logger;

        public KYCController(IKYCService kycService, ILogger<KYCController> logger)
        {
            _kycService = kycService;
            _logger = logger;
        }

        // Customer endpoints
        [HttpPost(ApplicationConstants.Routes.SubmitDocument)]
        public async Task<ActionResult> SubmitKYCDocument([FromBody] KYCSubmissionDto kycDto)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCSubmissionRequested, kycDto.CustomerId);

                var result = await _kycService.SubmitKYCDocumentAsync(kycDto);
                if (result)
                {
                    _logger.LogInformation(ApiConstants.LogMessages.KYCSubmissionCompleted, kycDto.CustomerId);
                    return Ok(new { success = true, message = ApplicationConstants.ErrorMessages.KYCSubmissionSuccess });
                }

                _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCSubmissionFailed);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCSubmissionFailed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCSubmissionFailed);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCSubmissionFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Status)]
        public async Task<ActionResult<KYCStatusDto>> GetKYCStatus(int customerId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalRequested, customerId);

                var status = await _kycService.GetCustomerKYCStatusAsync(customerId);

                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalCompleted, customerId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.MyStatus)]
        public async Task<ActionResult<KYCStatusDto>> GetMyKYCStatus()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalRequested, userId);

                var status = await _kycService.GetCustomerKYCStatusAsync(userId);

                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalCompleted, userId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                var userId = GetCurrentUserId();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed, userId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.IsCompleted)]
        public async Task<ActionResult<bool>> IsKYCCompleted(int customerId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalRequested, customerId);

                var isCompleted = await _kycService.IsKYCCompletedAsync(customerId);

                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalCompleted, customerId);
                return Ok(new { isCompleted });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed });
            }
        }

        // Manager endpoints
        [HttpGet(ApplicationConstants.Routes.PendingDocuments)]
        public async Task<ActionResult<IEnumerable<KYCVerificationDto>>> GetPendingKYCDocuments()
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.PendingKYCRetrievalRequested);

                var documents = await _kycService.GetPendingKYCDocumentsAsync();

                _logger.LogInformation(ApiConstants.LogMessages.PendingKYCRetrievalCompleted, documents.Count());
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PendingKYCRetrievalFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.PendingKYCRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Document)]
        public async Task<ActionResult<KYCVerificationDto>> GetKYCDocumentDetails(int documentId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalRequested, documentId);

                var document = await _kycService.GetKYCDocumentDetailsAsync(documentId);

                _logger.LogInformation(ApiConstants.LogMessages.KYCStatusRetrievalCompleted, documentId);
                return Ok(document);
            }
            catch (ArgumentException)
            {
                _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCDocumentNotFound, documentId);
                return NotFound(new { message = ApplicationConstants.ErrorMessages.KYCDocumentNotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed, documentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Verify)]
        public async Task<ActionResult> VerifyKYCDocument([FromBody] KYCVerificationRequestDto verificationDto)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCVerificationRequested, verificationDto.DocumentId);

                var result = await _kycService.VerifyKYCDocumentAsync(verificationDto);
                if (result)
                {
                    _logger.LogInformation(ApiConstants.LogMessages.KYCVerificationCompleted, verificationDto.DocumentId);
                    return Ok(new { success = true, message = ApplicationConstants.ErrorMessages.KYCVerificationSuccess });
                }

                _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCVerificationFailed, verificationDto.DocumentId);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCVerificationFailed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCVerificationFailed, verificationDto.DocumentId);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCVerificationFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Reject)]
        public async Task<ActionResult> RejectKYCDocument([FromBody] KYCVerificationRequestDto rejectionDto)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCRejectionRequested, rejectionDto.DocumentId);

                var result = await _kycService.RejectKYCDocumentAsync(rejectionDto);
                if (result)
                {
                    _logger.LogInformation(ApiConstants.LogMessages.KYCRejectionCompleted, rejectionDto.DocumentId);
                    return Ok(new { success = true, message = ApplicationConstants.ErrorMessages.KYCRejectionSuccess });
                }

                _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCRejectionFailed, rejectionDto.DocumentId);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCRejectionFailed });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCRejectionFailed, rejectionDto.DocumentId);
                return BadRequest(new { success = false, message = ApplicationConstants.ErrorMessages.KYCRejectionFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Score)]
        public async Task<ActionResult<decimal>> GetKYCScore(int customerId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.KYCScoreCalculationRequested, customerId);

                var score = await _kycService.CalculateKYCScoreAsync(customerId);

                _logger.LogInformation(ApiConstants.LogMessages.KYCScoreCalculationCompleted, customerId, score);
                return Ok(new { score });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCScoreCalculationFailed, customerId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.KYCScoreCalculationFailed });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}