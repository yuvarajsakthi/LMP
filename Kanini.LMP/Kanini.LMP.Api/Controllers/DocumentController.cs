using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, IUnitOfWork unitOfWork, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpPost(ApiConstants.Routes.Upload)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<DocumentUploadDto>>> UploadDocument([FromForm] DocumentUploadRequest request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized(ApiResponse<DocumentUploadDto>.ErrorResponse(ApplicationConstants.ErrorMessages.Unauthorized));

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentUploadRequested, request.DocumentName);

                await _unitOfWork.BeginTransactionAsync();
                var result = await _documentService.UploadDocumentAsync(request, userId);
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation(ApiConstants.LogMessages.DocumentUploadCompleted, result.DocumentId);
                return Ok(ApiResponse<DocumentUploadDto>.SuccessResponse(result, ApplicationConstants.Messages.Created));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentUploadFailed);
                return BadRequest(ApiResponse<DocumentUploadDto>.ErrorResponse(ApplicationConstants.ErrorMessages.DocumentUploadFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.DownloadDocument)]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized();

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentDownloadRequested, documentId);

                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);
                return File(fileData, "application/octet-stream", fileName);
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentNotFound, documentId);
                return NotFound(new { message = ApplicationConstants.Messages.DocumentNotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDownloadFailed, documentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentDownloadFailed });
            }
        }

        [HttpGet(ApiConstants.Routes.ViewDocument)]
        public async Task<IActionResult> ViewDocument(int documentId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized();

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentViewRequested, documentId);

                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);

                var contentType = GetContentType(fileName);
                Response.Headers[ApplicationConstants.Headers.ContentDisposition] = ApplicationConstants.Headers.Inline;

                _logger.LogInformation(ApiConstants.LogMessages.DocumentViewCompleted, documentId);
                return File(fileData, contentType);
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentNotFound, documentId);
                return NotFound(new { message = ApplicationConstants.Messages.DocumentNotFound });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentViewFailed, documentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentViewFailed });
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ApplicationConstants.FileExtensions.Pdf => ApplicationConstants.ContentTypes.Pdf,
                ApplicationConstants.FileExtensions.Jpg or ApplicationConstants.FileExtensions.Jpeg => ApplicationConstants.ContentTypes.Jpeg,
                ApplicationConstants.FileExtensions.Png => ApplicationConstants.ContentTypes.Png,
                ApplicationConstants.FileExtensions.Gif => ApplicationConstants.ContentTypes.Gif,
                ApplicationConstants.FileExtensions.Txt => ApplicationConstants.ContentTypes.Text,
                ApplicationConstants.FileExtensions.Doc => ApplicationConstants.ContentTypes.Doc,
                ApplicationConstants.FileExtensions.Docx => ApplicationConstants.ContentTypes.Docx,
                _ => ApplicationConstants.ContentTypes.OctetStream
            };
        }

        [HttpGet(ApiConstants.Routes.DocumentsByApplication)]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetDocumentsByApplication(int loanApplicationBaseId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentsByApplicationRequested, loanApplicationBaseId);

                var documents = await _documentService.GetDocumentsByApplicationAsync(loanApplicationBaseId);
                var documentsList = documents.ToList();

                _logger.LogInformation(ApiConstants.LogMessages.DocumentsByApplicationCompleted, documentsList.Count, loanApplicationBaseId);
                return Ok(documentsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed });
            }
        }

        [HttpPost(ApiConstants.Routes.Verify)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<DocumentUploadDto>> VerifyDocument([FromBody] DocumentVerificationRequest request)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int managerId))
                return Unauthorized();

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentVerificationRequested, request.DocumentId);

                var result = await _documentService.VerifyDocumentAsync(request, managerId);

                _logger.LogInformation(ApiConstants.LogMessages.DocumentVerificationCompleted, request.DocumentId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.DocumentVerificationFailed, request.DocumentId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentVerificationFailed, request.DocumentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentVerificationFailed });
            }
        }

        [HttpDelete(ApiConstants.Routes.DeleteDocument)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
                return Unauthorized();

            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentDeletionRequested, documentId);

                var success = await _documentService.DeleteDocumentAsync(documentId, userId);

                if (!success)
                {
                    _logger.LogWarning("Document not found or unauthorized access for document ID: {DocumentId}", documentId);
                    return NotFound(new { message = ApplicationConstants.Messages.DocumentNotFoundOrUnauthorized });
                }

                _logger.LogInformation(ApiConstants.LogMessages.DocumentDeletionCompleted, documentId);
                return Ok(new { message = ApplicationConstants.Messages.DocumentDeletedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDeletionFailed, documentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentDeletionFailed });
            }
        }

        [HttpGet(ApiConstants.Routes.Pending)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetPendingDocuments()
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.PendingDocumentsRequested);

                var documents = await _documentService.GetPendingDocumentsAsync();
                var documentsList = documents.ToList();

                _logger.LogInformation(ApiConstants.LogMessages.PendingDocumentsCompleted, documentsList.Count);
                return Ok(documentsList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed });
            }
        }
    }
}