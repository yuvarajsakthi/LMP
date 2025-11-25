using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.CustomerEntities.JunctionTable;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDocumentRepository _documentRepository;
        private readonly IApplicationDocumentLinkRepository _documentLinkRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(
            IUnitOfWork unitOfWork,
            IDocumentRepository documentRepository,
            IApplicationDocumentLinkRepository documentLinkRepository,
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
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentUpload, request.DocumentName, userId);

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

                        _logger.LogInformation(ApplicationConstants.Messages.DocumentUploadedSuccessfully, createdDocument.DocumentId);
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
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentUploadFailed, request.DocumentName, userId);
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentUploadFailed);
            }
        }

        public async Task<byte[]> DownloadDocumentAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentDownload, documentId, userId);

                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document?.DocumentData == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentNotFound, documentId);
                    throw new FileNotFoundException(ApplicationConstants.Messages.DocumentNotFound);
                }

                return document.DocumentData;
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDownloadFailed, documentId);
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDownloadFailed);
            }
        }

        public async Task<(byte[] fileData, string fileName)> DownloadDocumentWithNameAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentDownload, documentId, userId);
                return await _documentRepository.GetDocumentWithNameAsync(documentId);
            }
            catch (Exception ex) when (!(ex is FileNotFoundException))
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDownloadFailed, documentId);
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDownloadFailed);
            }
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetDocumentsByApplicationAsync(int loanApplicationBaseId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.RetrievingDocumentsByApplication, loanApplicationBaseId);

                var documentLinks = await _documentLinkRepository.GetLinksByApplicationAsync(loanApplicationBaseId);
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
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed, loanApplicationBaseId);
                throw new Exception(ApplicationConstants.ErrorMessages.RetrieveDocumentsFailed);
            }
        }

        public async Task<DocumentUploadDto> VerifyDocumentAsync(DocumentVerificationRequest request, int managerId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentVerification, request.DocumentId, managerId);

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var documentLink = await _documentLinkRepository.GetLinkByApplicationAndDocumentAsync(
                            request.LoanApplicationBaseId, request.DocumentId);

                        if (documentLink == null)
                        {
                            _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentLinkNotFound, request.DocumentId, request.LoanApplicationBaseId);
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
                        _logger.LogInformation(ApplicationConstants.Messages.DocumentVerifiedSuccessfully, request.DocumentId);
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
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentVerificationFailed, request.DocumentId);
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentVerificationFailed);
            }
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingDocumentDeletion, documentId, userId);

                if (!await _documentRepository.IsDocumentOwnedByUserAsync(documentId, userId))
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.DocumentNotFoundOrUnauthorized, documentId, userId);
                    return false;
                }

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        await _documentRepository.DeleteAsync(documentId);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation(ApplicationConstants.Messages.DocumentDeletedSuccessfully, documentId);
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
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentDeletionFailed, documentId);
                throw new Exception(ApplicationConstants.ErrorMessages.DocumentDeletionFailed);
            }
        }

        public async Task<IReadOnlyList<DocumentUploadDto>> GetPendingDocumentsAsync()
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.RetrievingPendingDocuments);

                var pendingLinks = await _documentLinkRepository.GetPendingLinksAsync();
                var result = new List<DocumentUploadDto>();

                foreach (var link in pendingLinks)
                {
                    var document = await _documentRepository.GetByIdAsync(link.DocumentId);
                    if (document != null)
                    {
                        result.Add(MapToDto(document, link));
                    }
                }

                _logger.LogInformation(ApplicationConstants.Messages.PendingDocumentsRetrieved, result.Count);
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