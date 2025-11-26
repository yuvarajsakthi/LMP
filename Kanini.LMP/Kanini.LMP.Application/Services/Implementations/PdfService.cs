using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly IPdfRepository _pdfRepository;
        private readonly ILogger<PdfService> _logger;

        public PdfService(IPdfRepository pdfRepository, ILogger<PdfService> logger)
        {
            _pdfRepository = pdfRepository;
            _logger = logger;
        }

        public async Task<byte[]> GenerateLoanApplicationPdfAsync(int applicationId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPdfGeneration, "Loan Application", applicationId);

                var application = await _pdfRepository.GetLoanApplicationWithDetailsAsync(applicationId);

                if (application == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.PdfDataNotFound, "Application", applicationId);
                    throw new ArgumentException(string.Format(ApplicationConstants.ErrorMessages.LoanApplicationNotFound, applicationId));
                }

                var loanDetails = application.LoanDetails;
                var pdfContent = GenerateApplicationPdfContent(application, loanDetails);

                _logger.LogInformation(ApplicationConstants.Messages.PdfGenerationCompleted, "Loan Application", applicationId);
                return Encoding.UTF8.GetBytes(pdfContent);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PdfGenerationFailed, "Loan Application", applicationId);
                throw new Exception(ApplicationConstants.ErrorMessages.PdfGenerationFailed);
            }
        }

        public async Task<byte[]> GeneratePaymentReceiptPdfAsync(int transactionId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPdfGeneration, "Payment Receipt", transactionId);

                var transaction = await _pdfRepository.GetPaymentTransactionAsync(transactionId);

                if (transaction == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.PdfDataNotFound, "Transaction", transactionId);
                    throw new ArgumentException(ApplicationConstants.ErrorMessages.PaymentNotFound);
                }

                var receiptContent = GeneratePaymentReceiptContent(transaction);

                _logger.LogInformation(ApplicationConstants.Messages.PdfGenerationCompleted, "Payment Receipt", transactionId);
                return Encoding.UTF8.GetBytes(receiptContent);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PdfGenerationFailed, "Payment Receipt", transactionId);
                throw new Exception(ApplicationConstants.ErrorMessages.PdfGenerationFailed);
            }
        }

        public async Task<byte[]> GenerateEMISchedulePdfAsync(int loanAccountId)
        {
            try
            {
                _logger.LogInformation(ApplicationConstants.Messages.ProcessingPdfGeneration, "EMI Schedule", loanAccountId);

                var loanAccount = await _pdfRepository.GetLoanAccountAsync(loanAccountId);

                if (loanAccount == null)
                {
                    _logger.LogWarning(ApplicationConstants.Messages.PdfDataNotFound, "Loan Account", loanAccountId);
                    throw new ArgumentException(ApplicationConstants.ErrorMessages.LoanAccountNotFound);
                }

                var scheduleContent = GenerateEMIScheduleContent(loanAccount);

                _logger.LogInformation(ApplicationConstants.Messages.PdfGenerationCompleted, "EMI Schedule", loanAccountId);
                return Encoding.UTF8.GetBytes(scheduleContent);
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ApplicationConstants.ErrorMessages.PdfGenerationFailed, "EMI Schedule", loanAccountId);
                throw new Exception(ApplicationConstants.ErrorMessages.PdfGenerationFailed);
            }
        }

        private string GenerateApplicationPdfContent(LoanApplicationBase application, dynamic loanDetails)
        {
            return $@"
LOAN APPLICATION DOCUMENT
========================

Application ID: {application.LoanApplicationBaseId}
Application Date: {application.SubmissionDate}
Status: {application.Status}

CUSTOMER INFORMATION
-------------------
Application ID: {application.LoanApplicationBaseId}
Loan Product: {application.LoanProductType}

LOAN DETAILS
-----------
Loan Type: {application.LoanProductType}
Requested Amount: ₹{loanDetails?.RequestedAmount ?? 0:N2}
Tenure: {loanDetails?.TenureMonths ?? 0} months
Interest Rate: {loanDetails?.InterestRate ?? 0}%
Monthly EMI: ₹{loanDetails?.MonthlyInstallment ?? 0:N2}

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

This is a system-generated document.
";
        }

        private string GeneratePaymentReceiptContent(PaymentTransaction transaction)
        {
            return $@"
PAYMENT RECEIPT
==============

Transaction ID: {transaction.TransactionId}
Payment Date: {transaction.PaymentDate:dd MMM yyyy HH:mm}
Amount: ₹{transaction.Amount:N2}
Payment Method: {transaction.PaymentMethod}
Status: {transaction.Status}
Reference: {transaction.TransactionReference ?? "N/A"}
Loan Account: {transaction.LoanAccountId}

Thank you for your payment!

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
";
        }

        private string GenerateEMIScheduleContent(LoanAccount loanAccount)
        {
            return $@"
EMI SCHEDULE
===========

Loan Account ID: {loanAccount.LoanAccountId}
Loan Amount: ₹{loanAccount.TotalLoanAmount:N2}

PAYMENT STATUS
-------------
Total Paid (Principal): ₹{loanAccount.TotalPaidPrincipal:N2}
Total Paid (Interest): ₹{loanAccount.TotalPaidInterest:N2}
Remaining Principal: ₹{loanAccount.PrincipalRemaining:N2}
Current Status: {loanAccount.CurrentPaymentStatus}

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
";
        }
    }
}