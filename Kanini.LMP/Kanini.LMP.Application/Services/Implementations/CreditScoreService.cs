using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CreditDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Caching.Memory;



namespace Kanini.LMP.Application.Services.Implementations
{
    public class CreditScoreService : ICreditScoreService
    {
        private readonly ILMPRepository<Customer, int> _customerRepository;
        private readonly IMemoryCache _cache;

        public CreditScoreService(
            ILMPRepository<Customer, int> customerRepository,
            IMemoryCache cache)
        {
            _customerRepository = customerRepository;
            _cache = cache;
        }

        public async Task<CreditScoreDto> GetCreditScoreAsync(int customerId)
        {
            var cacheKey = $"credit_score_{customerId}";

            if (_cache.TryGetValue(cacheKey, out CreditScoreDto? cachedScore))
                return cachedScore!;

            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var creditScore = await FetchCreditScoreFromCIBIL(customer);

            _cache.Set(cacheKey, creditScore, TimeSpan.FromHours(24));

            return creditScore;
        }

        public async Task<CreditScoreDto> RefreshCreditScoreAsync(int customerId, string pan)
        {
            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                throw new ArgumentException("Customer not found");

            var creditScore = await FetchCreditScoreFromCIBIL(customer, pan);

            // Update customer's credit score
            customer.CreditScore = creditScore.Score;
            await _customerRepository.UpdateAsync(customer);

            // Update cache
            var cacheKey = $"credit_score_{customerId}";
            _cache.Set(cacheKey, creditScore, TimeSpan.FromHours(24));

            return creditScore;
        }

        public async Task<bool> UpdateCustomerCreditScoreAsync(int customerId, int creditScore)
        {
            // Find customer by UserId (not CustomerId)
            var customer = await _customerRepository.GetAsync(c => c.UserId == customerId);
            if (customer == null)
                return false;

            customer.CreditScore = creditScore;
            await _customerRepository.UpdateAsync(customer);
            return true;
        }

        private async Task<CreditScoreDto> FetchCreditScoreFromCIBIL(Customer customer, string? pan = null)
        {
            // Using mock data for demonstration (no API key required)
            await Task.Delay(500); // Simulate API call delay

            var score = GenerateMockCreditScore(customer);
            var scoreRange = GetScoreRange(score);

            return new CreditScoreDto
            {
                CustomerId = customer.CustomerId,
                PAN = pan ?? GenerateMockPAN(),
                Bureau = CreditBureau.CIBIL,
                Score = score,
                ScoreRange = scoreRange,
                FetchedAt = DateTime.UtcNow,
                ReportSummary = GenerateReportSummary(score, scoreRange),
                CreditHistory = GenerateMockCreditHistory(score)
            };
        }

        private int GenerateMockCreditScore(Customer customer)
        {
            var score = 300; // Start with minimum score
            var random = new Random(customer.CustomerId); // Consistent randomness per customer

            // 1. Income Factor (25% weight) - 0 to 200 points
            var incomeScore = CalculateIncomeScore(customer.AnnualIncome);
            score += incomeScore;

            // 2. Age/Experience Factor (15% weight) - 0 to 120 points
            var ageScore = CalculateAgeScore(customer.Age);
            score += ageScore;

            // 3. Occupation Stability (15% weight) - 0 to 120 points
            var occupationScore = CalculateOccupationScore(customer.Occupation);
            score += occupationScore;

            // 4. Home Ownership (10% weight) - 0 to 80 points
            var homeScore = CalculateHomeOwnershipScore(customer.HomeOwnershipStatus);
            score += homeScore;

            // 5. Gender Factor (5% weight) - 0 to 40 points (statistical risk assessment)
            var genderScore = CalculateGenderScore(customer.Gender);
            score += genderScore;

            // 6. Existing Credit Score (if available) (20% weight) - 0 to 160 points
            var existingScore = CalculateExistingCreditScore(customer.CreditScore);
            score += existingScore;

            // 7. Account Age Factor (10% weight) - 0 to 80 points
            var accountAgeScore = CalculateAccountAgeScore(customer.UpdatedAt);
            score += accountAgeScore;

            // Apply small random variation (-20 to +20)
            var variation = random.Next(-20, 21);
            score += variation;

            // Ensure score is within valid range
            return Math.Max(300, Math.Min(850, score));
        }

        private int CalculateIncomeScore(decimal income)
        {
            return income switch
            {
                >= 2000000 => 200, // 20L+ = Excellent
                >= 1500000 => 180, // 15L+ = Very Good
                >= 1000000 => 160, // 10L+ = Good
                >= 800000 => 140,  // 8L+ = Above Average
                >= 600000 => 120,  // 6L+ = Average
                >= 400000 => 100,  // 4L+ = Below Average
                >= 250000 => 80,   // 2.5L+ = Low
                >= 150000 => 60,   // 1.5L+ = Very Low
                _ => 40            // Below 1.5L = Poor
            };
        }

        private int CalculateAgeScore(int age)
        {
            return age switch
            {
                >= 45 => 120,      // Peak earning years
                >= 35 => 110,      // Established career
                >= 30 => 100,      // Career growth
                >= 25 => 85,       // Early career
                >= 21 => 70,       // Entry level
                _ => 50            // Very young
            };
        }

        private int CalculateOccupationScore(string occupation)
        {
            var occupationLower = occupation.ToLower();

            // High stability professions
            if (occupationLower.Contains("government") || occupationLower.Contains("civil service") ||
                occupationLower.Contains("teacher") || occupationLower.Contains("professor") ||
                occupationLower.Contains("doctor") || occupationLower.Contains("engineer"))
                return 120;

            // Medium-high stability
            if (occupationLower.Contains("manager") || occupationLower.Contains("analyst") ||
                occupationLower.Contains("consultant") || occupationLower.Contains("accountant") ||
                occupationLower.Contains("lawyer") || occupationLower.Contains("banker"))
                return 100;

            // Medium stability
            if (occupationLower.Contains("sales") || occupationLower.Contains("marketing") ||
                occupationLower.Contains("technician") || occupationLower.Contains("supervisor"))
                return 85;

            // Lower stability
            if (occupationLower.Contains("freelance") || occupationLower.Contains("contractor") ||
                occupationLower.Contains("driver") || occupationLower.Contains("retail"))
                return 65;

            // Default for other occupations
            return 75;
        }

        private int CalculateHomeOwnershipScore(HomeOwnershipStatus? homeStatus)
        {
            return homeStatus switch
            {
                HomeOwnershipStatus.Owned => 80,      // Own home = Excellent
                HomeOwnershipStatus.Mortage => 65,     // Mortgage = Good
                HomeOwnershipStatus.Rented => 45,      // Rented = Average
                _ => 40                                // Unknown = Below Average
            };
        }

        private int CalculateGenderScore(Gender gender)
        {
            // Statistical risk assessment (not discriminatory, based on historical data)
            return gender switch
            {
                Gender.Female => 40,  // Statistically lower default rates
                Gender.Male => 35,    // Standard assessment
                _ => 35
            };
        }

        private int CalculateExistingCreditScore(decimal existingScore)
        {
            if (existingScore == 0) return 80; // No previous credit = neutral

            return existingScore switch
            {
                >= 800 => 160,  // Excellent existing score
                >= 750 => 140,  // Very good existing score
                >= 700 => 120,  // Good existing score
                >= 650 => 100,  // Fair existing score
                >= 600 => 80,   // Below average existing score
                >= 550 => 60,   // Poor existing score
                _ => 40         // Very poor existing score
            };
        }

        private int CalculateAccountAgeScore(DateTime? updatedAt)
        {
            if (!updatedAt.HasValue) return 40; // New account

            var accountAge = DateTime.UtcNow - updatedAt.Value;
            var monthsOld = (int)accountAge.TotalDays / 30;

            return monthsOld switch
            {
                >= 24 => 80,   // 2+ years = Excellent
                >= 12 => 70,   // 1+ year = Good
                >= 6 => 60,    // 6+ months = Average
                >= 3 => 50,    // 3+ months = Below Average
                _ => 40        // New account = Poor
            };
        }

        private CreditScoreRange GetScoreRange(int score)
        {
            return score switch
            {
                >= 800 => CreditScoreRange.Excellent,
                >= 750 => CreditScoreRange.VeryGood,
                >= 650 => CreditScoreRange.Good,
                >= 550 => CreditScoreRange.Fair,
                _ => CreditScoreRange.Poor
            };
        }

        private string GenerateReportSummary(int score, CreditScoreRange range)
        {
            var factors = new List<string>();

            // Add specific improvement suggestions based on score range
            var suggestions = range switch
            {
                CreditScoreRange.Excellent => "Maintain current financial habits. You qualify for premium loan products with the lowest interest rates.",
                CreditScoreRange.VeryGood => "Strong credit profile. You qualify for most loan products with competitive rates.",
                CreditScoreRange.Good => "Good credit standing. Consider increasing income or improving payment history for better rates.",
                CreditScoreRange.Fair => "Room for improvement. Focus on timely payments and reducing existing debt obligations.",
                _ => "Credit score needs significant improvement. Consider debt consolidation and establishing regular payment patterns."
            };

            return $"Credit Score: {score}/850 ({range}). {suggestions}";
        }

        private List<CreditHistoryItem> GenerateMockCreditHistory(int score)
        {
            var history = new List<CreditHistoryItem>();
            var random = new Random(score); // Consistent history per score

            // Generate credit history based on score ranges
            if (score >= 300)
            {
                // Everyone gets at least one credit card (even if closed/poor history)
                var cardLimit = score switch
                {
                    >= 800 => random.Next(500000, 1000000),
                    >= 750 => random.Next(300000, 500000),
                    >= 700 => random.Next(200000, 300000),
                    >= 650 => random.Next(100000, 200000),
                    >= 600 => random.Next(50000, 100000),
                    _ => random.Next(25000, 50000)
                };

                var outstanding = (decimal)(cardLimit * (score >= 700 ? 0.1 : score >= 600 ? 0.3 : 0.7));

                history.Add(new CreditHistoryItem
                {
                    AccountType = "Credit Card",
                    Institution = GetRandomBank(random),
                    CreditLimit = cardLimit,
                    OutstandingAmount = outstanding,
                    Status = score >= 600 ? CreditStatus.Active : CreditStatus.Closed,
                    OpenedDate = DateTime.UtcNow.AddYears(-random.Next(1, 5)),
                    PaymentHistory = Math.Min(99, Math.Max(60, score / 10 + random.Next(-5, 6)))
                });
            }

            // Add personal loan for higher scores
            if (score >= 650)
            {
                var loanAmount = score switch
                {
                    >= 800 => random.Next(1000000, 2000000),
                    >= 750 => random.Next(500000, 1000000),
                    >= 700 => random.Next(300000, 500000),
                    _ => random.Next(200000, 300000)
                };

                var outstanding = (decimal)(loanAmount * random.NextDouble() * 0.6); // 0-60% outstanding

                history.Add(new CreditHistoryItem
                {
                    AccountType = "Personal Loan",
                    Institution = GetRandomBank(random),
                    CreditLimit = loanAmount,
                    OutstandingAmount = outstanding,
                    Status = CreditStatus.Active,
                    OpenedDate = DateTime.UtcNow.AddYears(-random.Next(1, 3)),
                    PaymentHistory = Math.Min(99, Math.Max(75, score / 10 + random.Next(-3, 4)))
                });
            }

            // Add home loan for excellent scores
            if (score >= 750)
            {
                var homeLoanAmount = random.Next(2000000, 5000000);
                var outstanding = (decimal)(homeLoanAmount * (0.4 + random.NextDouble() * 0.5)); // 40-90% outstanding

                history.Add(new CreditHistoryItem
                {
                    AccountType = "Home Loan",
                    Institution = GetRandomBank(random),
                    CreditLimit = homeLoanAmount,
                    OutstandingAmount = outstanding,
                    Status = CreditStatus.Active,
                    OpenedDate = DateTime.UtcNow.AddYears(-random.Next(2, 8)),
                    PaymentHistory = Math.Min(99, Math.Max(85, score / 10 + random.Next(-2, 3)))
                });
            }

            // Add vehicle loan for good scores
            if (score >= 700 && random.Next(0, 2) == 0) // 50% chance
            {
                var vehicleLoanAmount = random.Next(300000, 1200000);
                var outstanding = (decimal)(vehicleLoanAmount * random.NextDouble() * 0.8); // 0-80% outstanding

                history.Add(new CreditHistoryItem
                {
                    AccountType = "Vehicle Loan",
                    Institution = GetRandomBank(random),
                    CreditLimit = vehicleLoanAmount,
                    OutstandingAmount = outstanding,
                    Status = outstanding > 0 ? CreditStatus.Active : CreditStatus.Closed,
                    OpenedDate = DateTime.UtcNow.AddYears(-random.Next(1, 4)),
                    PaymentHistory = Math.Min(99, Math.Max(80, score / 10 + random.Next(-2, 3)))
                });
            }

            return history.OrderByDescending(h => h.OpenedDate).ToList();
        }

        private string GetRandomBank(Random random)
        {
            var banks = new[] { "HDFC Bank", "ICICI Bank", "SBI", "Axis Bank", "Kotak Mahindra", "IndusInd Bank", "Yes Bank", "IDFC First Bank" };
            return banks[random.Next(banks.Length)];
        }



        private string GenerateMockPAN()
        {
            var random = new Random();
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var digits = "0123456789";

            return $"{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{letters[random.Next(letters.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{digits[random.Next(digits.Length)]}{letters[random.Next(letters.Length)]}";
        }


    }
}