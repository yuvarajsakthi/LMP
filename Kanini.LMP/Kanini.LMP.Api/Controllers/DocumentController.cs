using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.DocumentController)]
    [ApiController]
    [Authorize]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(IDocumentService documentService, ILogger<DocumentController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        [HttpPost(ApplicationConstants.Routes.Upload)]
        public async Task<ActionResult<DocumentUploadDto>> UploadDocument([FromForm] DocumentUploadRequest request)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentUploadRequested, request.DocumentName);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _documentService.UploadDocumentAsync(request, userId);

                _logger.LogInformation(ApplicationConstants.Messages.DocumentUploadCompleted, result.DocumentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentUploadFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentUploadFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.DownloadDocument)]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentDownloadRequested, documentId);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);

                _logger.LogInformation(ApplicationConstants.Messages.DocumentDownloadCompleted, documentId);
                return File(fileData, ApplicationConstants.ContentTypes.OctetStream, fileName);
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

        [HttpGet(ApplicationConstants.Routes.ViewDocument)]
        public async Task<IActionResult> ViewDocument(int documentId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentViewRequested, documentId);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var (fileData, fileName) = await _documentService.DownloadDocumentWithNameAsync(documentId, userId);

                var contentType = GetContentType(fileName);
                Response.Headers[ApplicationConstants.Headers.ContentDisposition] = ApplicationConstants.Headers.Inline;

                _logger.LogInformation(ApplicationConstants.Messages.DocumentViewCompleted, documentId);
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

        [HttpGet(ApplicationConstants.Routes.DocumentsByApplication)]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetDocumentsByApplication(int loanApplicationBaseId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentsByApplicationRequested, loanApplicationBaseId);

                var documents = await _documentService.GetDocumentsByApplicationAsync(loanApplicationBaseId);

                _logger.LogInformation(ApplicationConstants.Messages.DocumentsByApplicationCompleted, documents.Count, loanApplicationBaseId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed });
            }
        }

        [HttpPost(ApplicationConstants.Routes.Verify)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<DocumentUploadDto>> VerifyDocument([FromBody] DocumentVerificationRequest request)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentVerificationRequested, request.DocumentId);

                var managerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var result = await _documentService.VerifyDocumentAsync(request, managerId);

                _logger.LogInformation(ApplicationConstants.Messages.DocumentVerificationCompleted, request.DocumentId);
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

        [HttpDelete(ApplicationConstants.Routes.DeleteDocument)]
        public async Task<IActionResult> DeleteDocument(int documentId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.DocumentDeletionRequested, documentId);

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var success = await _documentService.DeleteDocumentAsync(documentId, userId);

                if (!success)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentNotFoundOrUnauthorized, documentId, userId);
                    return NotFound(new { message = ApplicationConstants.Messages.DocumentNotFoundOrUnauthorized });
                }

                _logger.LogInformation(ApplicationConstants.Messages.DocumentDeletionCompleted, documentId);
                return Ok(new { message = ApplicationConstants.Messages.DocumentDeletedSuccessfully });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDeletionFailed, documentId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentDeletionFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Pending)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<IReadOnlyList<DocumentUploadDto>>> GetPendingDocuments()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.PendingDocumentsRequested);

                var documents = await _documentService.GetPendingDocumentsAsync();

                _logger.LogInformation(ApplicationConstants.Messages.PendingDocumentsCompleted, documents.Count);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed });
            }
        }
    }
}