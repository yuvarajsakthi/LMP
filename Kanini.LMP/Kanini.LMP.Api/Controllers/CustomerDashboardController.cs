// using Kanini.LMP.Application.Constants;
// using Kanini.LMP.Application.Services.Interfaces;
// using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
// using Kanini.LMP.Database.EntitiesDtos;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;

// namespace Kanini.LMP.Api.Controllers
// {
//     [Route("customerdashboard")]
//     [ApiController]
//     [Authorize(Roles = ApplicationConstants.Roles.Customer)]
//     public class CustomerDashboardController : ControllerBase
//     {
//         private readonly ILoanProductService _loanProductService;
//         private readonly ILoanApplicationService _loanApplicationService;
//         // private readonly IEligibilityService _eligibilityService;
//         private readonly ICustomerService _customerService;

//         public CustomerDashboardController(
//             ILoanProductService loanProductService,
//             ILoanApplicationService loanApplicationService,
//             // IEligibilityService eligibilityService,
//             ICustomerService customerService)
//         {
//             _loanProductService = loanProductService;
//             _loanApplicationService = loanApplicationService;
//             // _eligibilityService = eligibilityService;
//             _customerService = customerService;
//         }

//         [HttpGet("loanproducts")]
//         [AllowAnonymous]
//         public async Task<ActionResult<ApiResponse<IReadOnlyList<LoanProductDto>>>> GetLoanProducts()
//         {
//             try
//             {
//                 var products = await _loanProductService.GetActiveLoanProductsAsync();
//                 return Ok(ApiResponse<IReadOnlyList<LoanProductDto>>.SuccessResponse(products));
//             }
//             catch (Exception)
//             {
//                 return BadRequest(ApiResponse<IReadOnlyList<LoanProductDto>>.ErrorResponse("Failed to retrieve loan products"));
//             }
//         }

//         [HttpGet("recent-applied-loans")]
//         public async Task<ActionResult<ApiResponse<object>>> GetRecentAppliedLoans()
//         {
//             try
//             {
//                 var customerId = GetCustomerId();
//                 var recentLoans = await _loanApplicationService.GetRecentApplicationsAsync(customerId, 2);
                
//                 var result = recentLoans.Select(loan => new
//                 {
//                     LoanId = loan.LoanApplicationBaseId,
//                     LoanName = loan.LoanType.ToString(),
//                     AmountToBePaid = loan.LoanAmount,
//                     EMI = loan.MonthlyEMI,
//                     YearsRemaining = CalculateYearsRemaining(loan.LoanTerm),
//                     Status = loan.Status.ToString(),
//                     AppliedDate = loan.ApplicationDate
//                 });

//                 return Ok(ApiResponse<object>.SuccessResponse(result));
//             }
//             catch (Exception)
//             {
//                 return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve recent applications"));
//             }
//         }

//         // [HttpGet("eligibilityscore")]
//         // public async Task<ActionResult<ApiResponse<object>>> GetEligibilityScore()
//         // {
//         //     try
//         //     {
//         //         var customerId = GetCustomerId();
//         //         var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, 0);
                
//         //         var result = new
//         //         {
//         //             CustomerId = customerId,
//         //             EligibilityScore = eligibility?.EligibilityScore ?? 0,
//         //             CreditScore = eligibility?.CreditScore ?? 0,
//         //             Status = eligibility?.EligibilityStatus?.ToString() ?? "Unknown",
//         //             Rating = GetCreditRating(eligibility?.CreditScore ?? 0),
//         //             LastUpdated = eligibility?.CalculatedOn ?? DateTime.UtcNow
//         //         };

//         //         return Ok(ApiResponse<object>.SuccessResponse(result));
//         //     }
//         //     catch (Exception)
//         //     {
//         //         return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve eligibility score"));
//         //     }
//         // }

//         // [HttpPut("eligibilityscore")]
//         // public async Task<ActionResult<ApiResponse<object>>> UpdateEligibilityScore([FromForm] EligibilityProfileRequest request)
//         // {
//         //     try
//         //     {
//         //         var customerId = GetCustomerId();
//         //         await _eligibilityService.UpdateCustomerProfileAsync(customerId, request);
                
//         //         var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, 0);
                
//         //         var result = new
//         //         {
//         //             CustomerId = customerId,
//         //             EligibilityScore = eligibility?.EligibilityScore ?? 0,
//         //             CreditScore = eligibility?.CreditScore ?? 0,
//         //             Status = eligibility?.EligibilityStatus?.ToString() ?? "Unknown",
//         //             Rating = GetCreditRating(eligibility?.CreditScore ?? 0),
//         //             LastUpdated = eligibility?.CalculatedOn ?? DateTime.UtcNow
//         //         };

//         //         return Ok(ApiResponse<object>.SuccessResponse(result));
//         //     }
//         //     catch (Exception)
//         //     {
//         //         return BadRequest(ApiResponse<object>.ErrorResponse("Failed to update eligibility score"));
//         //     }
//         // }

//         [HttpGet("applicationstatus")]
//         public async Task<ActionResult<ApiResponse<object>>> GetApplicationStatus()
//         {
//             try
//             {
//                 var customerId = GetCustomerId();
//                 var applications = await _loanApplicationService.GetCustomerApplicationsAsync(customerId);
                
//                 var result = applications.Select(app => new
//                 {
//                     ApplicationId = app.LoanApplicationBaseId,
//                     LoanType = app.LoanType.ToString(),
//                     Amount = app.LoanAmount,
//                     Status = app.Status.ToString(),
//                     AppliedDate = app.ApplicationDate,
//                     LastUpdated = app.LastModified
//                 });

//                 return Ok(ApiResponse<object>.SuccessResponse(result));
//             }
//             catch (Exception)
//             {
//                 return BadRequest(ApiResponse<object>.ErrorResponse("Failed to retrieve application status"));
//             }
//         }

//         private int GetCustomerId()
//         {
//             var customerIdClaim = User.FindFirst(ApplicationConstants.Claims.CustomerId)?.Value;
//             if (!string.IsNullOrEmpty(customerIdClaim))
//                 return int.Parse(customerIdClaim);

//             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             if (!string.IsNullOrEmpty(userIdClaim))
//             {
//                 var userId = int.Parse(userIdClaim);
//                 var customer = _customerService.GetByUserIdAsync(userId).Result;
//                 return customer?.CustomerId ?? 0;
//             }

//             return 0;
//         }

//         private int CalculateYearsRemaining(int loanTermMonths)
//         {
//             return (int)Math.Ceiling(loanTermMonths / 12.0);
//         }

//         private string GetCreditRating(int creditScore)
//         {
//             return creditScore switch
//             {
//                 >= 800 => "Excellent",
//                 >= 750 => "Very Good",
//                 >= 650 => "Good",
//                 >= 550 => "Fair",
//                 _ => "Poor"
//             };
//         }
//     }
// }