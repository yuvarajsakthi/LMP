using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route(ApiConstants.Routes.ApiController)]
    [ApiController]
    [Authorize]
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<EligibilityController> _logger;

        public EligibilityController(IEligibilityService eligibilityService, ICustomerService customerService, ILogger<EligibilityController> logger)
        {
            _eligibilityService = eligibilityService;
            _customerService = customerService;
            _logger = logger;
        }

        [HttpGet(ApiConstants.Routes.EligibilityController.GetScore)]
        public async Task<ActionResult> GetEligibilityScore(int customerId)
        {
            try
            {
                _logger.LogInformation("Getting eligibility score for customer {CustomerId}", customerId);
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(new IdDTO { Id = customerId }, new IdDTO { Id = 0 });
                return Ok(new { CustomerId = customerId, EligibilityScore = eligibility.CreditScore, Status = eligibility.EligibilityStatus });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Customer not found: {CustomerId}", customerId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get eligibility score for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "Failed to get eligibility score", error = ex.Message });
            }
        }

        [HttpPost(ApiConstants.Routes.EligibilityController.Calculate)]
        public async Task<ActionResult> CalculateEligibilityScore(int customerId)
        {
            try
            {
                _logger.LogInformation("Calculating eligibility score for customer {CustomerId}", customerId);
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(new IdDTO { Id = customerId }, new IdDTO { Id = 0 });
                return Ok(eligibility);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Customer not found: {CustomerId}", customerId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate eligibility score for customer {CustomerId}", customerId);
                return StatusCode(500, new { message = "Failed to calculate eligibility score", error = ex.Message });
            }
        }

        [HttpPut(ApiConstants.Routes.EligibilityController.Update)]
        public async Task<ActionResult> UpdateEligibilityProfile(int customerId, [FromBody] EligibilityProfileRequest request)
        {
            try
            {
                await _eligibilityService.UpdateCustomerProfileAsync(new IdDTO { Id = customerId }, request);
                return Ok(new { message = "Profile updated and eligibility score recalculated" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Failed to update profile" });
            }
        }

        [HttpPost(ApiConstants.Routes.EligibilityController.Check)]
        public async Task<ActionResult> CheckEligibility([FromForm] EligibilityProfileRequest request)
        {
            try
            {
                _logger.LogInformation(ApiConstants.LogMessages.EligibilityCheckRequested, request.IsExistingBorrower);

                // Validate required fields based on user type
                if (!request.IsExistingBorrower)
                {
                    if (string.IsNullOrEmpty(request.PAN) || !request.Age.HasValue ||
                        !request.AnnualIncome.HasValue || string.IsNullOrEmpty(request.Occupation) ||
                        !request.HomeOwnershipStatus.HasValue)
                    {
                        _logger.LogWarning(ApplicationConstants.ErrorMessages.EligibilityValidationFailed);
                        return BadRequest(new { message = ApplicationConstants.ErrorMessages.EligibilityValidationFailed });
                    }
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                // Validate PAN number matches customer's existing PAN
                if (!request.IsExistingBorrower && !string.IsNullOrEmpty(request.PAN))
                {
                    var customer = await _customerService.GetByUserIdAsync(new IdDTO { Id = userId });
                    if (customer != null && !string.IsNullOrEmpty(customer.PANNumber))
                    {
                        if (!customer.PANNumber.Equals(request.PAN, StringComparison.OrdinalIgnoreCase))
                        {
                            _logger.LogWarning("PAN number mismatch for user {UserId}", userId);
                            return BadRequest(new { message = "PAN number does not match with registered PAN" });
                        }
                    }
                }

                // Update customer profile and calculate credit score
                await _eligibilityService.UpdateCustomerProfileAsync(new IdDTO { Id = userId }, request);

                // Calculate eligibility based on updated profile
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(new IdDTO { Id = userId }, new IdDTO { Id = 0 });
                var eligibleProductIds = await _eligibilityService.GetEligibleProductsAsync(new IdDTO { Id = userId });

                var response = BuildEligibilityResponse(eligibility, eligibleProductIds, request.IsExistingBorrower);
                _logger.LogInformation(ApiConstants.LogMessages.EligibilityCheckCompleted, userId, eligibility.EligibilityScore);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, ApplicationConstants.ErrorMessages.EligibilityCheckFailed);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EligibilityCheckFailed);
                return BadRequest(new { message = ApplicationConstants.ErrorMessages.EligibilityCheckFailed });
            }
        }

        private object BuildEligibilityResponse(EligibilityScoreDto eligibility, List<IdDTO> eligibleProductIds, bool isExistingBorrower = false)
        {
            var allProducts = new[]
            {
                new { ProductId = 1, ProductName = "Personal Loan", Available = eligibleProductIds.Any(p => p.Id == 1), MinScore = 55, MaxAmount = "₹25L", InterestRate = "10.5-18%" },
                new { ProductId = 2, ProductName = "Vehicle Loan", Available = eligibleProductIds.Any(p => p.Id == 2), MinScore = 55, MaxAmount = "₹50L", InterestRate = "8.5-15%" },
                new { ProductId = 3, ProductName = "Home Loan", Available = eligibleProductIds.Any(p => p.Id == 3), MinScore = 65, MaxAmount = "₹5Cr", InterestRate = "8.0-12%" }
            };

            var userType = isExistingBorrower ? "existing borrower" : "new applicant";
            var message = eligibility.EligibilityScore switch
            {
                >= 800 => $"🎉 Exceptional! As an {userType}, you qualify for all premium loan products with the best rates.",
                >= 750 => $"⭐ Excellent! You qualify for all loan products with competitive rates.",
                >= 650 => $"✅ Very Good! You qualify for most loan products with good rates.",
                >= 550 => $"👍 Good! You qualify for Personal and Vehicle loans.",
                >= 450 => $"📈 Fair credit score. Limited loan options available. Consider improving your profile.",
                _ => $"📉 Credit score needs improvement. Focus on building credit history and financial stability."
            };

            var improvementTips = GetImprovementTips(eligibility.EligibilityScore, isExistingBorrower);

            return new
            {
                // Customer Info
                CustomerId = eligibility.CustomerId,

                // Scores
                CreditScore = new { Score = eligibility.CreditScore, Range = GetCreditRange(eligibility.CreditScore) },
                EligibilityScore = eligibility.EligibilityScore,
                Status = eligibility.EligibilityStatus,

                // Products
                EligibleProductCount = eligibleProductIds.Count,
                Products = allProducts,

                // Guidance
                Message = message,
                ImprovementTips = improvementTips,

                // Metadata
                LastUpdated = eligibility.CalculatedOn,
                NextSteps = eligibleProductIds.Count > 0 ? "Click 'Apply Now' for any eligible loan" : "Update profile to improve eligibility"
            };
        }

        private string GetCreditRange(int score)
        {
            return score switch
            {
                >= 800 => "Excellent",
                >= 750 => "Very Good",
                >= 650 => "Good",
                >= 550 => "Fair",
                _ => "Poor"
            };
        }

        private string[] GetImprovementTips(double eligibilityScore, bool isExistingBorrower)
        {
            if (eligibilityScore >= 750) return new string[0]; // No tips needed for excellent scores

            var tips = new List<string>();

            if (eligibilityScore < 650)
            {
                tips.Add("💰 Increase your annual income");
                tips.Add("🏢 Consider stable employment (Government/IT/Banking/Healthcare)");
            }

            if (eligibilityScore < 600)
            {
                tips.Add("🏠 Home ownership improves credit profile");
            }

            if (isExistingBorrower && eligibilityScore < 700)
            {
                tips.Add("📊 Maintain good repayment history");
            }

            return tips.ToArray();
        }
    }
}