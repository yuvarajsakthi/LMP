using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.Enums;
using EntityPaymentStatus = Kanini.LMP.Database.Enums.PaymentStatus;
using EntityPaymentMethod = Kanini.LMP.Database.Enums.PaymentMethod;
using DtoPaymentStatus = Kanini.LMP.Database.EntitiesDto.PaymentTransaction.PaymentStatus;
using DtoPaymentMethod = Kanini.LMP.Database.EntitiesDto.PaymentTransaction.PaymentMethod;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly ILMPRepository<PaymentTransaction, int> _paymentRepository;
        private readonly ILMPRepository<EMIPlan, int> _emiRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(
            ILMPRepository<PaymentTransaction, int> paymentRepository,
            ILMPRepository<EMIPlan, int> emiRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _emiRepository = emiRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaymentTransactionDTO> CreatePaymentAsync(PaymentTransactionCreateDTO dto)
        {
            var payment = _mapper.Map<PaymentTransaction>(dto);
            payment.PaymentDate = DateTime.UtcNow;
            payment.TransactionReference = string.IsNullOrEmpty(dto.TransactionReference) 
                ? GenerateTransactionReference() 
                : dto.TransactionReference;
            payment.Status = EntityPaymentStatus.Pending;
            payment.CreatedAt = DateTime.UtcNow;
            payment.IsActive = true;

            var created = await _paymentRepository.AddAsync(payment);
            
            // Simulate payment gateway processing
            var gatewayResult = await ProcessPaymentGatewayAsync(created);
            created.Status = gatewayResult ? EntityPaymentStatus.Success : EntityPaymentStatus.Failed;
            created.UpdatedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(created);
            
            // Update EMI status if payment successful
            if (gatewayResult && dto.EMIId > 0)
            {
                var emi = await _emiRepository.GetByIdAsync(dto.EMIId);
                if (emi != null)
                {
                    emi.Status = EMIPlanStatus.Closed;
                    await _emiRepository.UpdateAsync(emi);
                    await CreateNotificationAsync(emi.CustomerId, $"Payment of ₹{dto.Amount} processed successfully for EMI #{dto.EMIId}");
                }
            }
            else if (!gatewayResult)
            {
                var loanAccount = await _unitOfWork.LoanAccounts.GetByIdAsync(dto.LoanAccountId);
                if (loanAccount != null)
                {
                    var loanApp = await _unitOfWork.LoanApplications.GetByIdAsync(loanAccount.LoanApplicationBaseId);
                    if (loanApp != null)
                        await CreateNotificationAsync(loanApp.CustomerId, $"Payment of ₹{dto.Amount} failed. Please try again.");
                }
            }
            
            return _mapper.Map<PaymentTransactionDTO>(created);
        }

        public async Task<IReadOnlyList<PaymentTransactionDTO>> GetPaymentsByLoanAccountAsync(int loanAccountId)
        {
            var payments = await _paymentRepository.GetAllAsync(p => p.LoanAccountId == loanAccountId);
            return _mapper.Map<IReadOnlyList<PaymentTransactionDTO>>(payments);
        }

        public async Task<PaymentTransactionDTO> UpdatePaymentStatusAsync(int paymentId, EntityPaymentStatus status)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new ArgumentException("Payment not found");

            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;

            var updated = await _paymentRepository.UpdateAsync(payment);
            return _mapper.Map<PaymentTransactionDTO>(updated);
        }

        public async Task<IReadOnlyList<EMIPlanDTO>> GetEMIPlansByLoanAccountAsync(int loanAccountId)
        {
            var emiPlans = await _emiRepository.GetAllAsync(e => e.LoanApplicationBaseId == loanAccountId);
            return _mapper.Map<IReadOnlyList<EMIPlanDTO>>(emiPlans);
        }



        private string GenerateTransactionReference()
        {
            return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private async Task<bool> ProcessPaymentGatewayAsync(PaymentTransaction payment)
        {
            // Simulate payment gateway processing
            await Task.Delay(100); // Simulate network delay
            
            // 95% success rate simulation
            var random = new Random();
            return random.Next(100) < 95;
        }

        public async Task<PaymentAnalyticsResult> GetPaymentsByDateRangeViaSPAsync(DateTime fromDate, DateTime toDate)
        {
            // Mock implementation - replace with actual SP call
            var payments = await _paymentRepository.GetAllAsync(p => p.PaymentDate >= fromDate && p.PaymentDate <= toDate);
            var paymentsList = payments.ToList();

            return new PaymentAnalyticsResult
            {
                TotalAmount = paymentsList.Sum(p => p.Amount),
                Count = paymentsList.Count,
                AverageAmount = paymentsList.Any() ? paymentsList.Average(p => p.Amount) : 0,
                OnTimeAmount = paymentsList.Where(p => p.Status == EntityPaymentStatus.Success).Sum(p => p.Amount),
                LateAmount = paymentsList.Where(p => p.Status == EntityPaymentStatus.Failed).Sum(p => p.Amount),
                PrepaymentAmount = paymentsList.Where(p => p.PaymentMethod == EntityPaymentMethod.Cash).Sum(p => p.Amount),
                CollectionRate = paymentsList.Any() ? (decimal)paymentsList.Count(p => p.Status == EntityPaymentStatus.Success) / paymentsList.Count * 100 : 0
            };
        }

        public async Task<IEnumerable<PaymentTransactionDTO>> GetPaymentHistoryViaSPAsync(int loanAccountId)
        {
            // Mock implementation - replace with actual SP call
            var payments = await _paymentRepository.GetAllAsync(p => p.LoanAccountId == loanAccountId);
            return _mapper.Map<IEnumerable<PaymentTransactionDTO>>(payments).OrderByDescending(p => p.PaymentDate);
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
