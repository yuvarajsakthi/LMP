using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.RazorpayController.Base)]
    [ApiController]
    [Authorize]
    public class RazorpayController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;
        private readonly ILogger<RazorpayController> _logger;

        public RazorpayController(IRazorpayService razorpayService, ILogger<RazorpayController> logger)
        {
            _razorpayService = razorpayService;
            _logger = logger;
        }

        [HttpPost(ApiConstants.Routes.RazorpayController.CreateOrder)]
        public async Task<ActionResult<RazorpayOrderResponseDto>> CreateOrder([FromBody] RazorpayOrderCreateDto orderDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRazorpayOrderCreation, orderDto.Amount);
                var order = await _razorpayService.CreateOrderAsync(orderDto);
                _logger.LogInformation(ApplicationConstants.Messages.RazorpayOrderCreationCompleted, order.Id);
                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RazorpayOrderCreationFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RazorpayOrderCreationFailed });
            }
        }

        [HttpPost(ApiConstants.Routes.RazorpayController.ProcessPayment)]
        public async Task<ActionResult<PaymentTransactionResponseDTO>> ProcessPayment([FromBody] RazorpayPaymentDto paymentDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRazorpayPayment, paymentDto.RazorpayPaymentId);
                var result = await _razorpayService.ProcessPaymentAsync(paymentDto);
                _logger.LogInformation(ApplicationConstants.Messages.RazorpayPaymentProcessingCompleted, paymentDto.RazorpayPaymentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RazorpayPaymentProcessingFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RazorpayPaymentProcessingFailed });
            }
        }

        [HttpPost(ApiConstants.Routes.RazorpayController.VerifySignature)]
        public async Task<ActionResult<bool>> VerifySignature([FromBody] RazorpayPaymentDto paymentDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRazorpaySignatureVerification, paymentDto.RazorpayPaymentId);
                var isValid = await _razorpayService.VerifyPaymentSignatureAsync(
                    paymentDto.RazorpayOrderId,
                    paymentDto.RazorpayPaymentId,
                    paymentDto.RazorpaySignature);
                _logger.LogInformation(ApplicationConstants.Messages.RazorpaySignatureVerificationCompleted, paymentDto.RazorpayPaymentId, isValid);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RazorpaySignatureVerificationFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RazorpaySignatureVerificationFailed });
            }
        }

        [HttpPost(ApiConstants.Routes.RazorpayController.DisburseLoan)]
        public async Task<ActionResult<DisbursementResponseDto>> DisburseLoan([FromBody] DisbursementDto disbursementDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRazorpayDisbursement, disbursementDto.Amount);
                var disbursement = await _razorpayService.CreateDisbursementAsync(disbursementDto);
                _logger.LogInformation(ApplicationConstants.Messages.RazorpayDisbursementCompleted, disbursement.Id);
                return Ok(disbursement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RazorpayDisbursementFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RazorpayDisbursementFailed });
            }
        }

        [HttpGet(ApiConstants.Routes.RazorpayController.GetDisbursementStatus)]
        public async Task<ActionResult<DisbursementResponseDto>> GetDisbursementStatus(string disbursementId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingRazorpayDisbursementStatus, disbursementId);
                var status = await _razorpayService.GetDisbursementStatusAsync(disbursementId);
                _logger.LogInformation(ApplicationConstants.Messages.RazorpayDisbursementStatusCompleted, disbursementId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.RazorpayDisbursementStatusFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.RazorpayDisbursementStatusFailed });
            }
        }
    }
}