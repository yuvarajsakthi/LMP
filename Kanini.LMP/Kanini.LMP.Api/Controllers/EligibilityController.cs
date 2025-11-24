using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;

using Kanini.LMP.Database.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kanini.LMP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EligibilityController : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService;

        public EligibilityController(IEligibilityService eligibilityService)
        {
            _eligibilityService = eligibilityService;
        }



        [HttpGet("check")]
        public async Task<ActionResult> CheckEligibility()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

                // Calculate eligibility using customer profile data only
                var eligibility = await _eligibilityService.CalculateEligibilityAsync(userId, 0);
                var eligibleProductIds = await _eligibilityService.GetEligibleProductsAsync(userId);

                var allProducts = new[]
                {
                    new { ProductId = 1, ProductName = "Personal Loan", Available = eligibleProductIds.Contains(1), MinScore = 55, MaxAmount = "₹25L", InterestRate = "10.5-18%" },
                    new { ProductId = 2, ProductName = "Vehicle Loan", Available = eligibleProductIds.Contains(2), MinScore = 55, MaxAmount = "₹50L", InterestRate = "8.5-15%" },
                    new { ProductId = 3, ProductName = "Home Loan", Available = eligibleProductIds.Contains(3), MinScore = 65, MaxAmount = "₹5Cr", InterestRate = "8.0-12%" }
                };

                var message = eligibility.EligibilityScore switch
                {
                    >= 65 => "🎉 Excellent! You qualify for all loan products with competitive rates.",
                    >= 55 => "✅ Good! You qualify for Personal and Vehicle loans. Improve score to 65+ for Home Loan.",
                    _ => $"📈 Score {eligibility.EligibilityScore}/100. Need 55+ to apply. Update your profile for better eligibility."
                };

                var improvementTips = eligibility.EligibilityScore < 65 ? new[]
                {
                    "💰 Increase annual income",
                    "🏢 Update to stable occupation (Government/IT/Banking)",
                    "🏠 Consider home ownership",
                    "📅 Maintain longer employment history"
                } : new string[0];

                return Ok(new
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
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
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
    }
}