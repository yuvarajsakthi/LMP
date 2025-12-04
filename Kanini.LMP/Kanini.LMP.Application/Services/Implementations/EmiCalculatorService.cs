using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmiCalculatorService : IEmiCalculatorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EmiCalculatorService> _logger;

        public EmiCalculatorService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EmiCalculatorService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public Task<EMIPlanDTO> CalculateEmiAsync(decimal principalAmount, decimal interestRate, int termMonths)
        {
            var monthlyRate = interestRate / 12 / 100;
            var emi = (principalAmount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), termMonths)) /
                      ((decimal)Math.Pow((double)(1 + monthlyRate), termMonths) - 1);

            var totalRepayment = emi * termMonths;
            var totalInterest = totalRepayment - principalAmount;

            return Task.FromResult(new EMIPlanDTO
            {
                PrincipleAmount = principalAmount,
                TermMonths = termMonths,
                RateOfInterest = interestRate,
                MonthlyEMI = Math.Round(emi, 2),
                TotalInterestPaid = Math.Round(totalInterest, 2),
                TotalRepaymentAmount = Math.Round(totalRepayment, 2),
                Status = EMIPlanStatus.Active,
                IsCompleted = false
            });
        }

        public async Task<EMIPlanDTO> CreateEmiPlanAsync(EMIPlanCreateDTO createDto)
        {
            try
            {
                _logger.LogInformation("Processing EMI plan creation");

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var calculatedEmi = await CalculateEmiAsync(createDto.PrincipleAmount, createDto.RateOfInterest, createDto.TermMonths);

                        var emiPlan = _mapper.Map<EMIPlan>(createDto);
                        emiPlan.MonthlyEMI = calculatedEmi.MonthlyEMI;
                        emiPlan.TotalInterestPaid = calculatedEmi.TotalInterestPaid;
                        emiPlan.TotalRepaymentAmount = calculatedEmi.TotalRepaymentAmount;
                        emiPlan.Status = EMIPlanStatus.Active;
                        emiPlan.IsCompleted = false;

                        var created = await _unitOfWork.EMIPlans.AddAsync(emiPlan);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("EMI plan created successfully");
                        await CreateNotificationAsync(createDto.CustomerId, $"EMI plan created: ₹{createDto.PrincipleAmount} for {createDto.TermMonths} months");
                        return MapToDto(created);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.EMIPlanCreationFailed);
                throw new Exception(ApplicationConstants.ErrorMessages.EMIPlanCreationFailed);
            }
        }

        public async Task<EMIPlanDTO> GetEmiPlanByIdAsync(int emiId)
        {
            try
            {
                _logger.LogInformation("Processing EMI plan retrieval");

                var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(emiId);
                return emiPlan != null ? MapToDto(emiPlan) : new EMIPlanDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EMI plan retrieval failed");
                throw new Exception(ApplicationConstants.ErrorMessages.EMIPlanRetrievalFailed);
            }
        }

        public async Task<IEnumerable<EMIPlanDTO>> GetEmiPlansByLoanApplicationAsync(int loanApplicationId)
        {
            var emiPlans = await _unitOfWork.EMIPlans.GetAllAsync(e => e.LoanApplicationBaseId == loanApplicationId);
            return emiPlans.Select(MapToDto);
        }

        public async Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId)
        {
            var emiPlans = await _unitOfWork.EMIPlans.GetAllAsync(e => e.CustomerId == customerId);
            var firstEmi = emiPlans.FirstOrDefault();
            if (firstEmi == null) return null;

            var schedule = await GenerateEMIScheduleAsync(firstEmi.EMIId);
            var paidCount = schedule.Count(s => s.PaymentStatus == "Paid");
            var nextDue = schedule.FirstOrDefault(s => s.PaymentStatus == "Pending");

            var dashboard = _mapper.Map<CustomerEMIDashboardDto>(firstEmi);
            dashboard.LoanAccountId = firstEmi.LoanApplicationBaseId;
            dashboard.TotalLoanAmount = firstEmi.PrincipleAmount;
            dashboard.PendingAmount = schedule.LastOrDefault()?.OutstandingBalance ?? 0;
            dashboard.InterestPaid = schedule.Where(s => s.PaymentStatus == "Paid").Sum(s => s.InterestAmount);
            dashboard.PrincipalPaid = schedule.Where(s => s.PaymentStatus == "Paid").Sum(s => s.PrincipalAmount);
            dashboard.CurrentMonthEMI = firstEmi.MonthlyEMI;
            dashboard.NextDueDate = nextDue?.DueDate;
            dashboard.EMIsPaid = paidCount;
            dashboard.EMIsRemaining = firstEmi.TermMonths - paidCount;
            dashboard.Status = firstEmi.Status.ToString();
            dashboard.IsOverdue = schedule.Any(s => s.PaymentStatus == "Overdue");
            dashboard.DaysOverdue = schedule.Where(s => s.PaymentStatus == "Overdue").Select(s => (DateTime.UtcNow - s.DueDate).Days).DefaultIfEmpty(0).Max();
            dashboard.LateFeeAmount = schedule.Sum(s => s.LateFee);
            dashboard.PaymentStatus = firstEmi.IsCompleted ? "Completed" : "Active";
            return dashboard;
        }

        public async Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId)
        {
            var emiPlans = await _unitOfWork.EMIPlans.GetAllAsync(e => e.CustomerId == customerId);
            var dashboards = new List<CustomerEMIDashboardDto>();

            foreach (var emi in emiPlans)
            {
                var schedule = await GenerateEMIScheduleAsync(emi.EMIId);
                var paidCount = schedule.Count(s => s.PaymentStatus == "Paid");
                var nextDue = schedule.FirstOrDefault(s => s.PaymentStatus == "Pending");

                var dashboard = _mapper.Map<CustomerEMIDashboardDto>(emi);
                dashboard.LoanAccountId = emi.LoanApplicationBaseId;
                dashboard.TotalLoanAmount = emi.PrincipleAmount;
                dashboard.PendingAmount = schedule.LastOrDefault()?.OutstandingBalance ?? 0;
                dashboard.InterestPaid = schedule.Where(s => s.PaymentStatus == "Paid").Sum(s => s.InterestAmount);
                dashboard.PrincipalPaid = schedule.Where(s => s.PaymentStatus == "Paid").Sum(s => s.PrincipalAmount);
                dashboard.CurrentMonthEMI = emi.MonthlyEMI;
                dashboard.NextDueDate = nextDue?.DueDate;
                dashboard.EMIsPaid = paidCount;
                dashboard.EMIsRemaining = emi.TermMonths - paidCount;
                dashboard.Status = emi.Status.ToString();
                dashboard.IsOverdue = schedule.Any(s => s.PaymentStatus == "Overdue");
                dashboard.DaysOverdue = schedule.Where(s => s.PaymentStatus == "Overdue").Select(s => (DateTime.UtcNow - s.DueDate).Days).DefaultIfEmpty(0).Max();
                dashboard.LateFeeAmount = schedule.Sum(s => s.LateFee);
                dashboard.PaymentStatus = emi.IsCompleted ? "Completed" : "Active";
                dashboards.Add(dashboard);
            }

            return dashboards;
        }

        public async Task<List<EMIScheduleDto>> GenerateEMIScheduleAsync(int emiId)
        {
            var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(emiId);
            if (emiPlan == null) return new List<EMIScheduleDto>();

            var payments = await _unitOfWork.PaymentTransactions.GetAllAsync(p => p.EMIId == emiId);
            var schedule = new List<EMIScheduleDto>();
            var monthlyRate = emiPlan.RateOfInterest / 12 / 100;
            var outstandingBalance = emiPlan.PrincipleAmount;
            var loanApp = await _unitOfWork.LoanApplications.GetByIdAsync(emiPlan.LoanApplicationBaseId);
            var startDate = loanApp?.SubmissionDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;

            for (int i = 1; i <= emiPlan.TermMonths; i++)
            {
                var interestAmount = outstandingBalance * monthlyRate;
                var principalAmount = emiPlan.MonthlyEMI - interestAmount;
                outstandingBalance -= principalAmount;
                var dueDate = startDate.AddMonths(i);
                var payment = payments.FirstOrDefault(p => p.PaymentDate.Month == dueDate.Month && p.PaymentDate.Year == dueDate.Year);
                var lateFee = payment == null && dueDate < DateTime.UtcNow ? await CalculateLateFeeForInstallment(dueDate) : 0;

                schedule.Add(new EMIScheduleDto
                {
                    InstallmentNumber = i,
                    DueDate = dueDate,
                    EMIAmount = emiPlan.MonthlyEMI,
                    PrincipalAmount = Math.Round(principalAmount, 2),
                    InterestAmount = Math.Round(interestAmount, 2),
                    OutstandingBalance = Math.Round(Math.Max(0, outstandingBalance), 2),
                    PaymentStatus = payment != null ? "Paid" : (dueDate < DateTime.UtcNow ? "Overdue" : "Pending"),
                    PaidDate = payment?.PaymentDate,
                    LateFee = lateFee
                });
            }

            return schedule;
        }

        public async Task<PrepaymentCalculationDto> CalculatePrepaymentAsync(int emiId, decimal prepaymentAmount)
        {
            var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(emiId);
            if (emiPlan == null) throw new ArgumentException(ApplicationConstants.ErrorMessages.EMIPlanNotFound);

            var payments = await _unitOfWork.PaymentTransactions.GetAllAsync(p => p.EMIId == emiId);
            var paidAmount = payments.Where(p => p.Status == Database.Enums.PaymentStatus.Success).Sum(p => p.Amount);
            var currentOutstanding = emiPlan.TotalRepaymentAmount - paidAmount;
            var prepaymentCharges = prepaymentAmount * 0.02m;
            var newOutstanding = Math.Max(0, currentOutstanding - prepaymentAmount);

            var monthlyRate = emiPlan.RateOfInterest / 12 / 100;
            var remainingMonths = emiPlan.TermMonths - (paidAmount / emiPlan.MonthlyEMI);

            var originalInterest = (emiPlan.MonthlyEMI * (decimal)remainingMonths) - (currentOutstanding - (currentOutstanding * monthlyRate * (decimal)remainingMonths));
            var newInterest = newOutstanding > 0 ? CalculateInterestForBalance(newOutstanding, monthlyRate, (int)remainingMonths) : 0;
            var interestSaved = originalInterest - newInterest;

            var newEMI = newOutstanding > 0 ? CalculateEMIAmount(newOutstanding, emiPlan.RateOfInterest, (int)remainingMonths) : 0;
            var reducedTenure = newOutstanding > 0 ? 0 : (int)(remainingMonths - (newOutstanding / emiPlan.MonthlyEMI));

            return new PrepaymentCalculationDto
            {
                CurrentOutstanding = Math.Round(currentOutstanding, 2),
                PrepaymentAmount = prepaymentAmount,
                NewOutstanding = Math.Round(newOutstanding, 2),
                InterestSaved = Math.Round(interestSaved, 2),
                NewEMIAmount = Math.Round(newEMI, 2),
                ReducedTenure = reducedTenure,
                PrepaymentCharges = Math.Round(prepaymentCharges, 2),
                NetSavings = Math.Round(interestSaved - prepaymentCharges, 2)
            };
        }

        public async Task<decimal> CalculateLateFeeAsync(int emiId, DateTime currentDate)
        {
            var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(emiId);
            if (emiPlan == null) return 0;

            var payments = await _unitOfWork.PaymentTransactions.GetAllAsync(p => p.EMIId == emiId);
            var paymentsCount = payments.Count(p => p.Status == Database.Enums.PaymentStatus.Success);
            var loanApp = await _unitOfWork.LoanApplications.GetByIdAsync(emiPlan.LoanApplicationBaseId);
            var startDate = loanApp?.SubmissionDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
            var nextDueDate = startDate.AddMonths(paymentsCount + 1);

            if (currentDate <= nextDueDate) return 0;

            var daysOverdue = (currentDate - nextDueDate).Days;
            var lateFeeRate = 0.02m;
            var lateFee = emiPlan.MonthlyEMI * lateFeeRate * (daysOverdue / 30m);

            return Math.Round(Math.Min(lateFee, emiPlan.MonthlyEMI * 0.1m), 2);
        }

        public async Task<EMIRestructureResultDto> CalculateEMIRestructureAsync(EMIRestructureDto restructureDto)
        {
            var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(restructureDto.EMIId);
            if (emiPlan == null) throw new ArgumentException(ApplicationConstants.ErrorMessages.EMIPlanNotFound);

            var payments = await _unitOfWork.PaymentTransactions.GetAllAsync(p => p.EMIId == restructureDto.EMIId);
            var paidAmount = payments.Where(p => p.Status == Database.Enums.PaymentStatus.Success).Sum(p => p.Amount);
            var currentOutstanding = emiPlan.TotalRepaymentAmount - paidAmount;
            var newTenure = restructureDto.NewTenureMonths ?? emiPlan.TermMonths;
            var newRate = restructureDto.NewInterestRate ?? emiPlan.RateOfInterest;
            var restructureCharges = currentOutstanding * 0.005m;

            var newEMI = CalculateEMIAmount(currentOutstanding, newRate, newTenure);
            var newTotalAmount = newEMI * newTenure;
            var additionalInterest = newTotalAmount - currentOutstanding;

            var newSchedule = await GenerateNewScheduleAsync(currentOutstanding, newEMI, newRate, newTenure, restructureDto.MoratoriumMonths);

            return new EMIRestructureResultDto
            {
                OriginalEMI = emiPlan.MonthlyEMI,
                NewEMI = Math.Round(newEMI, 2),
                OriginalTenure = emiPlan.TermMonths,
                NewTenure = newTenure + restructureDto.MoratoriumMonths,
                AdditionalInterest = Math.Round(additionalInterest, 2),
                RestructureCharges = Math.Round(restructureCharges, 2),
                NewSchedule = newSchedule
            };
        }

        public async Task<EMIPlanDTO> ApplyEMIRestructureAsync(EMIRestructureDto restructureDto)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingEMIRestructure, restructureDto.EMIId);

                using (var transaction = await _unitOfWork.BeginTransactionAsync())
                {
                    try
                    {
                        var emiPlan = await _unitOfWork.EMIPlans.GetByIdAsync(restructureDto.EMIId);
                        if (emiPlan == null)
                        {
                            _logger.LogWarning("EMI plan not found");
                            throw new ArgumentException(ApplicationConstants.ErrorMessages.EMIPlanNotFound);
                        }

                        var calculation = await CalculateEMIRestructureAsync(restructureDto);

                        emiPlan.TermMonths = calculation.NewTenure;
                        emiPlan.MonthlyEMI = calculation.NewEMI;
                        emiPlan.RateOfInterest = restructureDto.NewInterestRate ?? emiPlan.RateOfInterest;
                        emiPlan.TotalRepaymentAmount = calculation.NewEMI * calculation.NewTenure;
                        emiPlan.TotalInterestPaid = emiPlan.TotalRepaymentAmount - emiPlan.PrincipleAmount;

                        var updated = await _unitOfWork.EMIPlans.UpdateAsync(emiPlan);
                        await _unitOfWork.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _logger.LogInformation("EMI restructure applied successfully");
                        await CreateNotificationAsync(emiPlan.CustomerId, $"EMI restructured: New EMI ₹{calculation.NewEMI} for {calculation.NewTenure} months");
                        return MapToDto(updated);
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                _logger.LogError(ex, "EMI restructure application failed");
                throw new Exception(ApplicationConstants.ErrorMessages.EMIRestructureApplicationFailed);
            }
        }

        public async Task<object> GetCompleteEMIDetailsAsync(int emiId)
        {
            var emiPlan = await GetEmiPlanByIdAsync(emiId);
            if (emiPlan == null) return new { };

            var schedule = await GenerateEMIScheduleAsync(emiId);
            var lateFee = await CalculateLateFeeAsync(emiId, DateTime.UtcNow);

            return new
            {
                EMIPlan = emiPlan,
                Schedule = schedule,
                CurrentLateFee = lateFee,
                ScheduleSummary = new
                {
                    TotalInstallments = schedule.Count,
                    PaidInstallments = schedule.Count(s => s.PaymentStatus == "Paid"),
                    OverdueInstallments = schedule.Count(s => s.PaymentStatus == "Overdue"),
                    TotalLateFees = schedule.Sum(s => s.LateFee),
                    NextDueDate = schedule.FirstOrDefault(s => s.PaymentStatus == "Pending")?.DueDate,
                    RemainingPrincipal = schedule.LastOrDefault()?.OutstandingBalance ?? 0
                }
            };
        }

        private EMIPlanDTO MapToDto(EMIPlan emiPlan)
        {
            return _mapper.Map<EMIPlanDTO>(emiPlan);
        }

        private Task<decimal> CalculateLateFeeForInstallment(DateTime dueDate)
        {
            var daysOverdue = (DateTime.UtcNow - dueDate).Days;
            return Task.FromResult(daysOverdue > 0 ? Math.Min(daysOverdue * 50m, 2000m) : 0m);
        }

        private decimal CalculateInterestForBalance(decimal balance, decimal monthlyRate, int months)
        {
            return balance * monthlyRate * months;
        }

        private decimal CalculateEMIAmount(decimal principal, decimal annualRate, int months)
        {
            var monthlyRate = annualRate / 12 / 100;
            return (principal * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), months)) /
                   ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1);
        }

        private Task<List<EMIScheduleDto>> GenerateNewScheduleAsync(decimal principal, decimal emi, decimal rate, int tenure, int moratoriumMonths)
        {
            var schedule = new List<EMIScheduleDto>();
            var monthlyRate = rate / 12 / 100;
            var outstandingBalance = principal;
            var startDate = DateTime.UtcNow;

            for (int i = 1; i <= tenure + moratoriumMonths; i++)
            {
                var dueDate = startDate.AddMonths(i);

                if (i <= moratoriumMonths)
                {
                    var interestOnly = outstandingBalance * monthlyRate;
                    schedule.Add(new EMIScheduleDto
                    {
                        InstallmentNumber = i,
                        DueDate = dueDate,
                        EMIAmount = Math.Round(interestOnly, 2),
                        PrincipalAmount = 0,
                        InterestAmount = Math.Round(interestOnly, 2),
                        OutstandingBalance = Math.Round(outstandingBalance, 2),
                        PaymentStatus = "Pending"
                    });
                }
                else
                {
                    var interestAmount = outstandingBalance * monthlyRate;
                    var principalAmount = emi - interestAmount;
                    outstandingBalance -= principalAmount;

                    schedule.Add(new EMIScheduleDto
                    {
                        InstallmentNumber = i,
                        DueDate = dueDate,
                        EMIAmount = emi,
                        PrincipalAmount = Math.Round(principalAmount, 2),
                        InterestAmount = Math.Round(interestAmount, 2),
                        OutstandingBalance = Math.Round(Math.Max(0, outstandingBalance), 2),
                        PaymentStatus = "Pending"
                    });
                }
            }

            return Task.FromResult(schedule);
        }

        public async Task<EMIPlanDTO> CalculateEmiViaSPAsync(decimal principalAmount, decimal interestRate, int termMonths)
        {
            return await CalculateEmiAsync(principalAmount, interestRate, termMonths);
        }

        public async Task<IEnumerable<EMIScheduleDto>> GenerateEMIScheduleViaSPAsync(int emiId)
        {
            return await GenerateEMIScheduleAsync(emiId);
        }

        private async Task CreateNotificationAsync(int customerId, string message)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer != null)
            {
                var notification = new Notification
                {
                    UserId = customer.UserId,
                    NotificationMessage = message
                };
                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
