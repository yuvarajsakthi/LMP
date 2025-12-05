using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Database.EntitiesDto.Email;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<BoolDTO> SendEmailAsync(EmailDto emailDto)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                using var client = new SmtpClient(smtpSettings["Host"] ?? "smtp.gmail.com", int.Parse(smtpSettings["Port"] ?? "587"))
                {
                    Credentials = new NetworkCredential(smtpSettings["Username"] ?? "", smtpSettings["Password"] ?? ""),
                    EnableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true")
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"] ?? "noreply@lmp.com", smtpSettings["FromName"] ?? "LMP"),
                    Subject = emailDto.Subject,
                    Body = emailDto.Body,
                    IsBodyHtml = emailDto.IsHtml
                };

                message.To.Add(new MailAddress(emailDto.To));

                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {emailDto.To}");
                return new BoolDTO { Value = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {emailDto.To}");
                return new BoolDTO { Value = false };
            }
        }

        public async Task<BoolDTO> SendLoanApplicationSubmittedEmailAsync(LoanApplicationSubmittedEmailDto dto)
        {
            var subject = $"Loan Application Submitted - Application #{dto.ApplicationId}";
            var body = $"Dear {dto.CustomerName},\n\nYour {dto.LoanType} application for ₹{dto.Amount:N2} has been submitted successfully.\n\nApplication ID: {dto.ApplicationId}\n\nWe will review and get back to you within 3-5 business days.\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto
            {
                To = dto.CustomerEmail,
                Subject = subject,
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendLoanApprovedEmailAsync(LoanApprovedEmailDto dto)
        {
            var subject = string.Format(EmailTemplates.LoanApprovedSubject, dto.ApplicationId);
            var body = string.Format(EmailTemplates.LoanApprovedBody, dto.CustomerName, dto.ApplicationId, dto.LoanType, dto.Amount, DateTime.Now.ToString("dd MMM yyyy"));

            var emailDto = new EmailDto
            {
                To = dto.CustomerEmail,
                Subject = subject,
                Body = body
            };

            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendLoanRejectedEmailAsync(LoanRejectedEmailDto dto)
        {
            var subject = $"Loan Application Update - Application #{dto.ApplicationId}";
            var body = $"Dear {dto.CustomerName},\n\nWe regret to inform you that your loan application #{dto.ApplicationId} has been rejected.\n\nReason: {dto.Reason}\n\nYou may reapply after addressing the concerns.\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.CustomerEmail, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendEMIDueReminderEmailAsync(EMIDueReminderEmailDto dto)
        {
            var subject = $"📅 EMI Payment Reminder - Due {dto.DueDate:dd MMM yyyy}";
            var urgency = dto.DaysUntilDue <= 0 ? "is due today" : $"is due in {dto.DaysUntilDue} days";
            var body = $"Dear {dto.CustomerName},\n\nYour EMI payment of ₹{dto.Amount:N2} {urgency}.\n\nDue Date: {dto.DueDate:dd MMM yyyy}\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.CustomerEmail, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendOverduePaymentEmailAsync(OverduePaymentEmailDto dto)
        {
            var subject = $"⚠️ Urgent: Overdue Payment - {dto.DaysPastDue} Days Past Due";
            var body = $"Dear {dto.CustomerName},\n\nYour EMI payment of ₹{dto.Amount:N2} is {dto.DaysPastDue} days overdue.\n\nPlease make payment immediately.\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.CustomerEmail, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendLoanDisbursedEmailAsync(LoanDisbursedEmailDto dto)
        {
            var subject = "💰 Loan Disbursed Successfully";
            var body = $"Dear {dto.CustomerName},\n\nYour loan of ₹{dto.Amount:N2} has been disbursed successfully.\n\nLoan Account ID: {dto.LoanAccountId}\n\nDisbursement Date: {dto.DisbursementDate:dd MMM yyyy}\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.CustomerEmail, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendLoanFullyPaidEmailAsync(LoanFullyPaidEmailDto dto)
        {
            var subject = "🎉 Congratulations! Loan Fully Paid";
            var body = $"Dear {dto.CustomerName},\n\nCongratulations! You have fully paid your loan.\n\nLoan Account ID: {dto.LoanAccountId}\n\nTotal Amount Paid: ₹{dto.TotalAmountPaid:N2}\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.CustomerEmail, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }

        public async Task<BoolDTO> SendOTPEmailAsync(OTPEmailDto dto)
        {
            var subject = $"Your OTP for {dto.Purpose}";
            var body = $"Dear {dto.Name},\n\nYour OTP is: {dto.OTP}\n\nThis OTP is valid for 5 minutes.\n\nBest regards,\nLMP Team";

            var emailDto = new EmailDto { To = dto.Email, Subject = subject, Body = body };
            return await SendEmailAsync(emailDto);
        }
    }
}