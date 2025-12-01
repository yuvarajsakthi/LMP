using Kanini.LMP.Api.Constants;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
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
        private readonly ILogger<EligibilityController> _logger;

        public EligibilityController(IEligibilityService eligibilityService, ILogger<EligibilityController> logger)
        {
            _eligibilityService = eligibilityService;
            _logger = logger;
        }



        [HttpPost(ApiConstants.Routes.Check)]
        public async Task<ActionResult> CheckEligibility([FromBody] EligibilityProfileRequest request)
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

                // Update customer profile and calculate credit score
                await _eligibilityService.UpdateCustomerProfileAsync(userId, request);

                // Calculate eligibility based on updated profile
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(userId, 0);
                var eligibleProductIds = await _eligibilityService.GetEligibleProductsAsync(userId);

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

        private object BuildEligibilityResponse(EligibilityScoreDto eligibility, List<int> eligibleProductIds, bool isExistingBorrower = false)
        {
            var allProducts = new[]
            {
                new { ProductId = 1, ProductName = "Personal Loan", Available = eligibleProductIds.Contains(1), MinScore = 55, MaxAmount = "₹25L", InterestRate = "10.5-18%" },
                new { ProductId = 2, ProductName = "Vehicle Loan", Available = eligibleProductIds.Contains(2), MinScore = 55, MaxAmount = "₹50L", InterestRate = "8.5-15%" },
                new { ProductId = 3, ProductName = "Home Loan", Available = eligibleProductIds.Contains(3), MinScore = 65, MaxAmount = "₹5Cr", InterestRate = "8.0-12%" }
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
                tips.Add("⏰ Maintain consistent on-time payments");
                tips.Add("📉 Reduce existing debt burden");
                tips.Add("📅 Build longer employment history");
            }
            else if (!isExistingBorrower && eligibilityScore < 650)
            {
                tips.Add("📈 Build credit history with smaller loans first");
                tips.Add("💼 Gain more work experience in current role");
            }

            return tips.ToArray();
        }
    }
}