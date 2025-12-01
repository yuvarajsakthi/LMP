using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApplicationConstants.Routes.JointLoanController)]
    [ApiController]
    [Authorize]
    public class JointLoanController : ControllerBase
    {
        private readonly ILoanApplicationService _loanApplicationService;
        private readonly ILogger<JointLoanController> _logger;

        public JointLoanController(ILoanApplicationService loanApplicationService, ILogger<JointLoanController> logger)
        {
            _loanApplicationService = loanApplicationService;
            _logger = logger;
        }

        [HttpGet(ApplicationConstants.Routes.Applicants)]
        public async Task<ActionResult> GetLoanApplicants(int loanApplicationId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.ApplicantsRetrievalRequested, loanApplicationId);

                var applicants = await _loanApplicationService.GetApplicantsByLoanAsync(loanApplicationId);

                _logger.LogInformation(ApiConstants.LogMessages.ApplicantsRetrievalCompleted, applicants.Count, loanApplicationId);
                return Ok(new { LoanApplicationId = loanApplicationId, ApplicantIds = applicants });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.ApplicantsRetrievalFailed, loanApplicationId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.ApplicantsRetrievalFailed });
            }
        }

        [HttpGet(ApplicationConstants.Routes.Documents)]
        public async Task<ActionResult> GetLoanDocuments(int loanApplicationId)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.DocumentsRetrievalRequested, loanApplicationId);

                var documents = await _loanApplicationService.GetDocumentsByLoanAsync(loanApplicationId);

                _logger.LogInformation(ApiConstants.LogMessages.DocumentsRetrievalCompleted, documents.Count, loanApplicationId);
                return Ok(new { LoanApplicationId = loanApplicationId, DocumentIds = documents });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.DocumentsRetrievalFailed, loanApplicationId);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.DocumentsRetrievalFailed });
            }
        }
    }
}