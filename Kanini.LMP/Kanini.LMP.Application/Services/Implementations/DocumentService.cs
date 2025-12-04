using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentRepository _documentRepository;
        private readonly ILMPRepository<ApplicationDocumentLink, int> _documentLinkRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IUnitOfWork unitOfWork,
            IDocumentRepository documentRepository,
            ILMPRepository<ApplicationDocumentLink, int> documentLinkRepository,
            IMapper mapper,
            ILogger<DocumentService> logger)
        {
            _unitOfWork = unitOfWork;
            _documentRepository = documentRepository;
            _documentLinkRepository = documentLinkRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DocumentUploadDto> UploadDocumentAsync(DocumentUploadRequest request, int userId)
        {
            try
            {
                _logger.LogInformation("Processing document upload");

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        using var memoryStream = new MemoryStream();
                        await request.File.CopyToAsync(memoryStream);

                        var document = new DocumentUpload
                        {
                            LoanApplicationBaseId = request.LoanApplicationBaseId,
                            UserId = userId,
                            DocumentName = request.DocumentName,
                            DocumentType = request.DocumentType,
                            DocumentData = memoryStream.ToArray(),
                            UploadedAt = DateTime.UtcNow
                        };

                        var createdDocument = await _documentRepository.AddAsync(document);

                        var documentLink = new ApplicationDocumentLink
                        {
                            LoanApplicationBaseId = request.LoanApplicationBaseId,
                            DocumentId = createdDocument.DocumentId,
                            DocumentRequirementType = request.DocumentRequirementType,
                            Status = DocumentStatus.Pending,
                            LinkedAt = DateTime.UtcNow
                        };

                        await _documentLinkRepository.AddAsync(documentLink);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Document uploaded successfully");
                        return MapToDto(createdDocument, documentLink);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Document upload failed");
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentUploadFailed);
            }
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation("Processing document download");

                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document?.DocumentData == null)
                {
                    _logger.LogWarning("Document not found");
                    throw new FileNotFoundException(ApplicationConstants.Messages.DocumentNotFound);
                }

                return document.DocumentData;
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                _logger.LogError(ex, "Document download failed");
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDownloadFailed);
            }
        }

        public async Task<(byte[] fileData, string fileName)> DownloadDocumentWithNameAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation("Processing document download with name");
                return await _documentRepository.GetDocumentWithNameAsync(documentId);
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                _logger.LogError(ex, "Document download with name failed");
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDownloadFailed);
            }
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetDocumentsByApplicationAsync(int loanApplicationBaseId)
        {
            try
            {
                _logger.LogInformation("Retrieving documents by application");

                var documentLinks = await _documentLinkRepository.GetAllAsync(dl => dl.LoanApplicationBaseId == loanApplicationBaseId);
                var result = new List<DocumentUploadDto>();

                foreach (var link in documentLinks)
                {
                    var document = await _documentRepository.GetByIdAsync(link.DocumentId);
                    if (document != null)
                    {
                        result.Add(MapToDto(document, link));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve documents");
                throw new Exception(ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed);
            }
        }

        public async Task<DocumentUploadDto> VerifyDocumentAsync(DocumentVerificationRequest request, int managerId)
        {
            try
            {
                _logger.LogInformation("Processing document verification");

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var documentLink = await _documentLinkRepository.GetByIdAsync(request.DocumentId);

                        if (documentLink == null)
                        {
                            _logger.LogWarning("Document link not found");
                            throw new ArgumentException(ApplicationConstants.ErrorMessages.DocumentLinkNotFound);
                        }

                        documentLink.Status = request.Status;
                        documentLink.VerificationNotes = request.VerificationNotes;
                        documentLink.VerifiedAt = DateTime.UtcNow;
                        documentLink.VerifiedBy = managerId;

                        await _documentLinkRepository.UpdateAsync(documentLink);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        var document = await _documentRepository.GetByIdAsync(request.DocumentId);
                        _logger.LogInformation("Document verified successfully");
                        return MapToDto(document!, documentLink);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                _logger.LogError(ex, "Document verification failed");
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentVerificationFailed);
            }
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation("Processing document deletion");

                if (!await _documentRepository.IsDocumentOwnedByUserAsync(documentId, userId))
                {
                    _logger.LogWarning("Document not found or unauthorized");
                    return false;
                }

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        await _documentRepository.DeleteAsync(documentId);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("Document deleted successfully");
                        return true;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Document deletion failed");
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDeletionFailed);
            }
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetPendingDocumentsAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.RetrievingPendingDocuments);

                var pendingLinks = await _documentLinkRepository.GetAllAsync(dl => dl.Status == DocumentStatus.Pending);
                var result = new List<DocumentUploadDto>();

                foreach (var link in pendingLinks)
                {
                    var document = await _documentRepository.GetByIdAsync(link.DocumentId);
                    if (document != null)
                    {
                        result.Add(MapToDto(document, link));
                    }
                }

                _logger.LogInformation("Pending documents retrieved successfully");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.RetrievePendingDocumentsFailed);
            }
        }

        private DocumentUploadDto MapToDto(DocumentUpload document, ApplicationDocumentLink link)
        {
            var dto = _mapper.Map<DocumentUploadDto>(document);
            dto.Status = link.Status;
            dto.VerificationNotes = link.VerificationNotes;
            dto.VerifiedAt = link.VerifiedAt;
            dto.VerifiedBy = link.VerifiedBy;
            return dto;
        }
    }
}

