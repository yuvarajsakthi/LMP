using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.EntitiesDto.KYC;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class KYCService : IKYCService
    {
        private readonly ILMPRepository<DocumentUpload, int> _documentRepository;
        private readonly ILMPRepository<User, int> _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<KYCService> _logger;

        // Required KYC document types
        private readonly List<string> _requiredKYCDocuments = new()
        {
            ApplicationConstants.KYCDocumentTypes.Aadhaar,
            ApplicationConstants.KYCDocumentTypes.PAN
        };

        public KYCService(
            ILMPRepository<DocumentUpload, int> documentRepository,
            ILMPRepository<User, int> userRepository,
            INotificationService notificationService,
            IMapper mapper,
            ILogger<KYCService> logger)
        {
            _documentRepository = documentRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> SubmitKYCDocumentAsync(KYCSubmissionDto kycDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingKYCSubmission, kycDto.CustomerId);

                var document = _mapper.Map<DocumentUpload>(kycDto);
                await _documentRepository.AddAsync(document);

                await _notificationService.NotifyManagerDocumentVerificationAsync(
                    3, kycDto.LoanApplicationId, ApplicationConstants.Roles.Customer);

                _logger.LogInformation(ApplicationConstants.Messages.KYCSubmissionCompleted, kycDto.CustomerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCSubmissionFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.KYCSubmissionFailed);
            }
        }

        public async Task<KYCStatusDto> GetCustomerKYCStatusAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId && d.DocumentType != null && d.DocumentType.StartsWith("KYC_"));

            var documentStatuses = _requiredKYCDocuments.Select(requiredDoc =>
            {
                var document = kycDocuments.FirstOrDefault(d => d.DocumentType == requiredDoc);
                return new KYCDocumentStatusDto
                {
                    DocumentType = requiredDoc.Replace("KYC_", ""),
                    Status = document != null ? "Submitted" : "Pending",
                    UploadedAt = document?.UploadedAt,
                    DocumentName = document?.DocumentName
                };
            }).ToList();

            var submittedCount = documentStatuses.Count(d => d.Status == "Submitted");
            var completionPercentage = (decimal)submittedCount / _requiredKYCDocuments.Count * 100;

            return new KYCStatusDto
            {
                CustomerId = customerId,
                IsKYCCompleted = submittedCount == _requiredKYCDocuments.Count,
                Documents = documentStatuses,
                CompletionPercentage = completionPercentage
            };
        }

        public async Task<IEnumerable<KYCVerificationDto>> GetPendingKYCDocumentsAsync()
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.DocumentType != null && d.DocumentType.StartsWith(ApplicationConstants.KYCDocumentTypes.Prefix));

            var users = await _userRepository.GetAllAsync();

            return kycDocuments.Select(doc =>
            {
                var user = users.FirstOrDefault(u => u.UserId == doc.UserId);
                var result = _mapper.Map<KYCVerificationDto>(doc);
                result.CustomerName = user?.FullName ?? ApplicationConstants.Messages.Unknown;
                return result;
            });
        }

        public async Task<KYCVerificationDto> GetKYCDocumentDetailsAsync(int documentId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingKYCStatusRetrieval, documentId);

                var document = await _documentRepository.GetByIdAsync(documentId);
                if (document == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCDocumentNotFound, documentId);
                    throw new ArgumentException(ApplicationConstants.ErrorMessages.KYCDocumentNotFound);
                }

                var user = await _userRepository.GetByIdAsync(document.UserId);

                var result = _mapper.Map<KYCVerificationDto>(document);
                result.CustomerName = user?.FullName ?? ApplicationConstants.Messages.Unknown;
                result.Status = ApplicationConstants.KYCStatus.Pending;

                _logger.LogInformation(ApplicationConstants.Messages.KYCStatusRetrievalCompleted, documentId);
                return result;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed, documentId);
                throw new Exception(ApplicationConstants.ErrorMessages.KYCStatusRetrievalFailed);
            }
        }

        public async Task<bool> VerifyKYCDocumentAsync(KYCVerificationRequestDto verificationDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingKYCVerification, verificationDto.DocumentId);

                var document = await _documentRepository.GetByIdAsync(verificationDto.DocumentId);
                if (document == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCDocumentNotFound, verificationDto.DocumentId);
                    return false;
                }

                document.DocumentName = $"{ApplicationConstants.ErrorMessages.VerifiedPrefix} {document.DocumentName}";
                await _documentRepository.UpdateAsync(document);

                await _notificationService.CreateNotificationAsync(new Database.EntitiesDto.NotificationDTO
                {
                    UserId = document.UserId,
                    Title = ApplicationConstants.NotificationTitles.KYCDocumentVerified,
                    Message = string.Format(ApplicationConstants.NotificationMessages.KYCDocumentVerifiedMessage,
                        document.DocumentType?.Replace(ApplicationConstants.KYCDocumentTypes.Prefix, ""))
                });

                _logger.LogInformation(ApplicationConstants.Messages.KYCVerificationCompleted, verificationDto.DocumentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCVerificationFailed, verificationDto.DocumentId);
                throw new Exception(ApplicationConstants.ErrorMessages.KYCVerificationFailed);
            }
        }

        public async Task<bool> RejectKYCDocumentAsync(KYCVerificationRequestDto rejectionDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingKYCRejection, rejectionDto.DocumentId);

                var document = await _documentRepository.GetByIdAsync(rejectionDto.DocumentId);
                if (document == null)
                {
                    _logger.LogWarning(ApplicationConstants.ErrorMessages.KYCDocumentNotFound, rejectionDto.DocumentId);
                    return false;
                }

                document.DocumentName = $"{ApplicationConstants.ErrorMessages.RejectedPrefix} {document.DocumentName}";
                await _documentRepository.UpdateAsync(document);

                await _notificationService.CreateNotificationAsync(new Database.EntitiesDto.NotificationDTO
                {
                    UserId = document.UserId,
                    Title = ApplicationConstants.NotificationTitles.KYCDocumentRejected,
                    Message = string.Format(ApplicationConstants.NotificationMessages.KYCDocumentRejectedMessage,
                        document.DocumentType?.Replace(ApplicationConstants.KYCDocumentTypes.Prefix, ""),
                        rejectionDto.Remarks ?? ApplicationConstants.NotificationMessages.DefaultRejectionReason)
                });

                _logger.LogInformation(ApplicationConstants.Messages.KYCRejectionCompleted, rejectionDto.DocumentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.KYCRejectionFailed, rejectionDto.DocumentId);
                throw new Exception(ApplicationConstants.ErrorMessages.KYCRejectionFailed);
            }
        }

        public async Task<bool> IsKYCCompletedAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId &&
                     d.DocumentType != null &&
                     d.DocumentType.StartsWith("KYC_") &&
                     d.DocumentName != null &&
                     d.DocumentName.Contains("[VERIFIED]"));

            var verifiedDocumentTypes = kycDocuments
                .Select(d => d.DocumentType)
                .Distinct()
                .ToList();

            return _requiredKYCDocuments.All(required =>
                verifiedDocumentTypes.Contains(required));
        }

        public async Task<decimal> CalculateKYCScoreAsync(int customerId)
        {
            var kycDocuments = await _documentRepository.GetAllAsync(
                d => d.UserId == customerId &&
                     d.DocumentType != null &&
                     d.DocumentType.StartsWith("KYC_"));

            var verifiedCount = kycDocuments.Count(d =>
                d.DocumentName != null && d.DocumentName.Contains("[VERIFIED]"));

            var totalRequired = _requiredKYCDocuments.Count;

            return totalRequired > 0 ? (decimal)verifiedCount / totalRequired * 100 : 0;
        }
    }
}