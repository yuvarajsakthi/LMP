using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EligibilityService : IEligibilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EligibilityService> _logger;

        public EligibilityService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EligibilityService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }



        public async Task<EligibilityScoreDto> CalculateEligibilityAsync(int customerId, int loanProductId)
        {
            try
            {
                _logger.LogInformation("Processing eligibility calculation");

                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer not found");
                    throw new ArgumentException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                // Calculate and update customer's eligibility score
                var creditScore = CalculateCreditScore(customer);
                customer.EligibilityScore = creditScore;
                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                var monthlyIncome = customer.AnnualIncome / 12;
                var eligibilityScore = CalculateEligibilityScore(customer, loanProductId);
                var status = DetermineEligibilityStatus(eligibilityScore, loanProductId);

                _logger.LogInformation("Eligibility calculation completed");
                var result = _mapper.Map<EligibilityScoreDto>(customer);
                result.LoanProductId = loanProductId;
                result.MonthlyIncome = monthlyIncome;
                result.EligibilityScore = eligibilityScore;
                result.EligibilityStatus = status;
                result.CalculatedOn = DateTime.UtcNow;
                return result;
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                _logger.LogError(ex, "Eligibility calculation failed");
                throw new Exception(ApplicationConstants.ErrorMessages.EligibilityCalculationFailed);
            }
        }

        public async Task<bool> IsEligibleForLoanAsync(int customerId, int loanProductId = 0)
        {
            var eligibility = await CalculateEligibilityAsync(customerId, loanProductId);
            return eligibility.EligibilityScore >= 55; // Minimum threshold
        }

        public async Task<List<int>> GetEligibleProductsAsync(int customerId)
        {
            var eligibility = await CalculateEligibilityAsync(customerId, 0);
            var eligibleProducts = new List<int>();

            if (eligibility.EligibilityScore >= 55)
            {
                eligibleProducts.Add(1); // Personal Loan
                eligibleProducts.Add(2); // Vehicle Loan
            }

            if (eligibility.EligibilityScore >= 65)
            {
                eligibleProducts.Add(3); // Home Loan
            }

            return eligibleProducts;
        }

        public async Task UpdateCustomerProfileAsync(int customerId, EligibilityProfileRequest request)
        {
            try
            {
                _logger.LogInformation("Processing customer profile update");

                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer == null)
                {
                    throw new ArgumentException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                // Update customer attributes
                if (request.AnnualIncome.HasValue)
                    customer.AnnualIncome = request.AnnualIncome.Value;
                
                if (!string.IsNullOrEmpty(request.Occupation))
                    customer.Occupation = request.Occupation;
                
                if (request.HomeOwnershipStatus.HasValue)
                    customer.HomeOwnershipStatus = request.HomeOwnershipStatus.Value;

                customer.UpdatedAt = DateTime.UtcNow;

                // Recalculate eligibility score with updated data
                var creditScore = CalculateCreditScore(customer);
                customer.EligibilityScore = creditScore;

                await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer profile updated successfully");
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                _logger.LogError(ex, "Customer profile update failed");
                throw new Exception(ApplicationConstants.ErrorMessages.CustomerProfileUpdateFailed);
            }
        }

        /// <summary>
        /// Calculate credit score using only Customer model attributes
        /// </summary>
        private decimal CalculateCreditScore(Customer customer)
        {
            decimal score = 300; // Base score

            // Age factor (25% weight)
            if (customer.Age >= 25 && customer.Age <= 60)
                score += 150;
            else if (customer.Age >= 21 && customer.Age <= 65)
                score += 100;
            else
                score += 50;

            // Income factor (35% weight)
            if (customer.AnnualIncome >= 1000000) // 10L+
                score += 200;
            else if (customer.AnnualIncome >= 500000) // 5L+
                score += 150;
            else if (customer.AnnualIncome >= 300000) // 3L+
                score += 100;
            else
                score += 50;

            // Occupation factor (20% weight)
            var occupation = customer.Occupation.ToLower();
            if (occupation.Contains("engineer") || occupation.Contains("doctor") || 
                occupation.Contains("manager") || occupation.Contains("officer"))
                score += 120;
            else if (occupation.Contains("teacher") || occupation.Contains("analyst") || 
                     occupation.Contains("consultant"))
                score += 100;
            else if (occupation.Contains("business") || occupation.Contains("self"))
                score += 80;
            else
                score += 60;

            // Home ownership factor (20% weight)
            if (customer.HomeOwnershipStatus == HomeOwnershipStatus.Owned)
                score += 130;
            else if (customer.HomeOwnershipStatus == HomeOwnershipStatus.Rented)
                score += 80;
            else
                score += 50;

            // Ensure score is within valid range (300-900)
            return Math.Max(300, Math.Min(900, score));
        }



        private int CalculateEligibilityScore(Customer customer, int loanProductId)
        {
            // Simple eligibility calculation based on eligibility score and income
            var creditScore = (int)customer.EligibilityScore;
            var monthlyIncome = customer.AnnualIncome / 12;

            int eligibilityScore = 0;

            // Credit score component (60%)
            eligibilityScore += (int)(creditScore * 0.06);

            // Income component (40%)
            if (monthlyIncome >= 100000) eligibilityScore += 40;
            else if (monthlyIncome >= 50000) eligibilityScore += 30;
            else if (monthlyIncome >= 25000) eligibilityScore += 20;
            else eligibilityScore += 10;

            return Math.Min(100, eligibilityScore);
        }

        private EligibilityStatus DetermineEligibilityStatus(int eligibilityScore, int loanProductId)
        {
            return eligibilityScore switch
            {
                >= 75 => EligibilityStatus.HighlyEligible,
                >= 60 => EligibilityStatus.Eligible,
                >= 45 => EligibilityStatus.ConditionallyEligible,
                _ => EligibilityStatus.NotEligible
            };
        }
    }
}