using Kanini.LMP.Application.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class MockWhatsAppService : IWhatsAppService
    {
        private readonly ILogger<MockWhatsAppService> _logger;

        public MockWhatsAppService(ILogger<MockWhatsAppService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendWhatsAppMessageAsync(string phoneNumber, string message)
        {
            _logger.LogInformation($"MOCK: WhatsApp message sent to {phoneNumber}: {message}");
            await Task.Delay(100); // Simulate API call
            return true;
        }

        public async Task<bool> SendLoanApplicationWhatsAppAsync(string phoneNumber, string customerName, int applicationId, string loanType)
        {
            return await SendWhatsAppMessageAsync(phoneNumber, $"Loan application {applicationId} submitted for {customerName}");
        }

        public async Task<bool> SendLoanApprovedWhatsAppAsync(string phoneNumber, string customerName, int applicationId, decimal amount)
        {
            return await SendWhatsAppMessageAsync(phoneNumber, $"Loan {applicationId} approved for {customerName}: ₹{amount}");
        }

        public async Task<bool> SendPaymentDueWhatsAppAsync(string phoneNumber, string customerName, decimal amount, DateTime dueDate)
        {
            return await SendWhatsAppMessageAsync(phoneNumber, $"Payment due for {customerName}: ₹{amount} on {dueDate:dd-MMM-yyyy}");
        }

        public async Task<bool> SendPaymentSuccessWhatsAppAsync(string phoneNumber, string customerName, decimal amount)
        {
            return await SendWhatsAppMessageAsync(phoneNumber, $"Payment successful for {customerName}: ₹{amount}");
        }

        public async Task<bool> SendLoanDisbursedWhatsAppAsync(string phoneNumber, string customerName, decimal amount)
        {
            return await SendWhatsAppMessageAsync(phoneNumber, $"Loan disbursed to {customerName}: ₹{amount}");
        }
    }
}