using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDtos;
using EntityPaymentStatus = Kanini.LMP.Database.Entities.PaymentStatus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, IUnitOfWork unitOfWork, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        [HttpGet(ApiConstants.Routes.PaymentController.LoanAccount)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer + "," + ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentTransactionDTO>>>> GetPaymentsByLoanAccount(int loanAccountId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPaymentsRetrieval);
                var payments = await _paymentService.GetPaymentsByLoanAccountAsync(loanAccountId);
                _logger.LogInformation(ApplicationConstants.Messages.PaymentsRetrievalCompleted);
                return Ok(ApiResponse<IReadOnlyList<PaymentTransactionDTO>>.SuccessResponse(payments));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed);
                return BadRequest(ApiResponse<IReadOnlyList<PaymentTransactionDTO>>.ErrorResponse(ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.PaymentController.EMIPlans)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer + "," + ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<ApiResponse<object>>> GetEMIPlans(int loanAccountId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingEMIPlansRetrieval);
                var emiPlans = await _paymentService.GetEMIPlansByLoanAccountAsync(loanAccountId);
                _logger.LogInformation(ApplicationConstants.Messages.EMIPlansRetrievalCompleted);
                return Ok(ApiResponse<object>.SuccessResponse(emiPlans));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.EMIPlansRetrievalFailed));
            }
        }

        [HttpPost]
        [Authorize(Roles = ApplicationConstants.Roles.Customer + "," + ApplicationConstants.Roles.Manager)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<PaymentTransactionDTO>>> CreatePayment(PaymentTransactionCreateDTO dto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPaymentCreation);
                
                await _unitOfWork.BeginTransactionAsync();
                var created = await _paymentService.CreatePaymentAsync(dto);
                await _unitOfWork.CommitTransactionAsync();
                
                _logger.LogInformation(ApplicationConstants.Messages.PaymentCreationCompleted);
                return CreatedAtAction(nameof(GetPaymentsByLoanAccount), 
                    new { loanAccountId = created.LoanAccountId }, 
                    ApiResponse<PaymentTransactionDTO>.SuccessResponse(created, ApplicationConstants.Messages.Created));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PaymentCreationFailed);
                return BadRequest(ApiResponse<PaymentTransactionDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.PaymentCreationFailed));
            }
        }

        [HttpPut(ApiConstants.Routes.PaymentController.UpdateStatus)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<PaymentTransactionDTO>>> UpdatePaymentStatus(int paymentId, [FromBody] EntityPaymentStatus status)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPaymentStatusUpdate);
                
                await _unitOfWork.BeginTransactionAsync();
                var updated = await _paymentService.UpdatePaymentStatusAsync(paymentId, status);
                await _unitOfWork.CommitTransactionAsync();
                
                _logger.LogInformation(ApplicationConstants.Messages.PaymentStatusUpdateCompleted);
                return Ok(ApiResponse<PaymentTransactionDTO>.SuccessResponse(updated, ApplicationConstants.Messages.Updated));
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PaymentStatusUpdateFailed);
                return BadRequest(ApiResponse<PaymentTransactionDTO>.ErrorResponse(ApplicationConstants.ErrorMessages.PaymentStatusUpdateFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.PaymentController.AnalyticsSP)]
        [Authorize(Roles = ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<ApiResponse<object>>> GetPaymentAnalyticsViaSP([FromQuery] DateTime? fromDate = null, [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var from = fromDate ?? DateTime.UtcNow.AddMonths(-1);
                var to = toDate ?? DateTime.UtcNow;
                var analytics = await _paymentService.GetPaymentsByDateRangeViaSPAsync(from, to);
                return Ok(ApiResponse<object>.SuccessResponse(analytics));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed));
            }
        }

        [HttpGet(ApiConstants.Routes.PaymentController.HistorySP)]
        [Authorize(Roles = ApplicationConstants.Roles.Customer + "," + ApplicationConstants.Roles.Manager)]
        public async Task<ActionResult<ApiResponse<object>>> GetPaymentHistoryViaSP(int loanAccountId)
        {
            try
            {
                var history = await _paymentService.GetPaymentHistoryViaSPAsync(loanAccountId);
                return Ok(ApiResponse<object>.SuccessResponse(history));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed);
                return BadRequest(ApiResponse<object>.ErrorResponse(ApplicationConstants.ErrorMessages.PaymentsRetrievalFailed));
            }
        }
    }
}