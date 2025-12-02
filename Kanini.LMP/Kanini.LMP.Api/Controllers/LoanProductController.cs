using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.AspNetCore.Mvc;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanProductController : ControllerBase
    {
        private readonly ILoanProductService _loanProductService;
        private readonly ILogger<LoanProductController> _logger;

        public LoanProductController(
            ILoanProductService loanProductService,
            ILogger<LoanProductController> logger)
        {
            _loanProductService = loanProductService;
            _logger = logger;
        }

        [HttpGet("active")]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<LoanProductDto>>>> GetActiveLoanProducts()
        {
            try
            {
                _logger.LogInformation("Retrieving all active loan products for loan category selection");

                var loanProducts = await _loanProductService.GetActiveLoanProductsAsync();

                _logger.LogInformation("Successfully retrieved {Count} active loan products", loanProducts.Count);
                return Ok(ApiResponse<IReadOnlyList<LoanProductDto>>.SuccessResponse(loanProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active loan products");
                return BadRequest(ApiResponse<IReadOnlyList<LoanProductDto>>.ErrorResponse(
                    "Failed to retrieve loan products. Please try again later."));
            }
        }
    }
}
