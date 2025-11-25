using Kanini.LMP.Data.Data;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerEntitiesDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kanini.LMP.Data.Repositories.Implementations
{
    public class EMIRepository : LMPRepositoy<EMIPlan, int>, IEMIRepository
    {
        public EMIRepository(LmpDbContext context) : base(context) { }

        public async Task<CustomerEMIDashboardDto?> GetCustomerEMIDashboardAsync(int customerId)
        {
            var emiData = await (from emi in _context.EMIPlans
                                 join loan in _context.PersonalLoanApplications on emi.LoanApplicationBaseId equals loan.LoanApplicationBaseId
                                 join applicant in _context.LoanApplicants on loan.LoanApplicationBaseId equals applicant.LoanApplicationBaseId
                                 join account in _context.LoanAccounts on emi.LoanAccountId equals account.LoanAccountId
                                 where applicant.CustomerId == customerId &&
                                       emi.Status == EMIPlanStatus.Active &&
                                       !emi.IsCompleted
                                 select new { emi, loan, account })
                                .FirstOrDefaultAsync();

            if (emiData == null) return null;

            var payments = await GetPaymentsByEMIIdAsync(emiData.emi.EMIId);
            var totalPaid = payments.Sum(p => p.Amount);
            var pendingAmount = emiData.emi.TotalRepaymentAmount - totalPaid;
            var interestPaid = Math.Min(totalPaid, emiData.emi.TotalInterestPaid);
            var principalPaid = totalPaid - interestPaid;
            var emisPaid = payments.Count;
            var emisRemaining = emiData.emi.TermMonths - emisPaid;
            var nextDueDate = emiData.loan.SubmissionDate.ToDateTime(TimeOnly.MinValue).AddMonths(emisPaid + 1);
            var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
            var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

            return new CustomerEMIDashboardDto
            {
                EMIId = emiData.emi.EMIId,
                LoanAccountId = emiData.emi.LoanAccountId,
                TotalLoanAmount = emiData.emi.PrincipleAmount,
                MonthlyEMI = emiData.emi.MonthlyEMI,
                PendingAmount = pendingAmount,
                TotalInterest = emiData.emi.TotalInterestPaid,
                InterestPaid = interestPaid,
                PrincipalPaid = principalPaid,
                CurrentMonthEMI = emiData.emi.MonthlyEMI,
                NextDueDate = nextDueDate,
                EMIsPaid = emisPaid,
                EMIsRemaining = emisRemaining,
                Status = emiData.emi.Status.ToString(),
                IsOverdue = isOverdue,
                DaysOverdue = daysOverdue,
                LateFeeAmount = emiData.account.TotalLateFeePaidAmount,
                PaymentStatus = emiData.account.CurrentPaymentStatus.ToString()
            };
        }

        public async Task<List<CustomerEMIDashboardDto>> GetAllCustomerEMIsAsync(int customerId)
        {
            var emiData = await (from emi in _context.EMIPlans
                                 join loan in _context.PersonalLoanApplications on emi.LoanApplicationBaseId equals loan.LoanApplicationBaseId
                                 join applicant in _context.LoanApplicants on loan.LoanApplicationBaseId equals applicant.LoanApplicationBaseId
                                 where applicant.CustomerId == customerId
                                 select new { emi, loan })
                                .ToListAsync();

            var result = new List<CustomerEMIDashboardDto>();

            foreach (var data in emiData)
            {
                var payments = await GetPaymentsByEMIIdAsync(data.emi.EMIId);
                var totalPaid = payments.Sum(p => p.Amount);
                var pendingAmount = data.emi.TotalRepaymentAmount - totalPaid;
                var interestPaid = Math.Min(totalPaid, data.emi.TotalInterestPaid);
                var principalPaid = totalPaid - interestPaid;
                var emisPaid = payments.Count;
                var emisRemaining = data.emi.TermMonths - emisPaid;
                var nextDueDate = data.loan.SubmissionDate.ToDateTime(TimeOnly.MinValue).AddMonths(emisPaid + 1);
                var isOverdue = nextDueDate < DateTime.UtcNow && pendingAmount > 0;
                var daysOverdue = isOverdue ? (DateTime.UtcNow - nextDueDate).Days : 0;

                result.Add(new CustomerEMIDashboardDto
                {
                    EMIId = data.emi.EMIId,
                    LoanAccountId = data.emi.LoanAccountId,
                    TotalLoanAmount = data.emi.PrincipleAmount,
                    MonthlyEMI = data.emi.MonthlyEMI,
                    PendingAmount = pendingAmount,
                    TotalInterest = data.emi.TotalInterestPaid,
                    InterestPaid = interestPaid,
                    PrincipalPaid = principalPaid,
                    CurrentMonthEMI = data.emi.MonthlyEMI,
                    NextDueDate = nextDueDate,
                    EMIsPaid = emisPaid,
                    EMIsRemaining = emisRemaining,
                    Status = data.emi.Status.ToString(),
                    IsOverdue = isOverdue,
                    DaysOverdue = daysOverdue
                });
            }

            return result;
        }

        public async Task<List<PaymentTransaction>> GetPaymentsByEMIIdAsync(int emiId)
        {
            return await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == PaymentStatus.Success)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<DateTime> GetLoanStartDateAsync(int loanApplicationBaseId)
        {
            var loan = await _context.PersonalLoanApplications
                .FirstOrDefaultAsync(l => l.LoanApplicationBaseId == loanApplicationBaseId);
            return loan?.SubmissionDate.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
        }

        public async Task<decimal> GetTotalPaidAmountAsync(int emiId)
        {
            return await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == PaymentStatus.Success)
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetPaidInstallmentsCountAsync(int emiId)
        {
            return await _context.PaymentTransactions
                .Where(p => p.EMIId == emiId && p.Status == PaymentStatus.Success)
                .CountAsync();
        }
    }
}