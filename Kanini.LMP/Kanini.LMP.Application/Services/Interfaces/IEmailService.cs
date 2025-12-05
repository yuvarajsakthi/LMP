using Kanini.LMP.Database.EntitiesDto.Email;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEmailService
    {
        Task<BoolDTO> SendEmailAsync(EmailDto emailDto);
        Task<BoolDTO> SendLoanApplicationSubmittedEmailAsync(LoanApplicationSubmittedEmailDto dto);
        Task<BoolDTO> SendLoanApprovedEmailAsync(LoanApprovedEmailDto dto);
        Task<BoolDTO> SendLoanRejectedEmailAsync(LoanRejectedEmailDto dto);
        Task<BoolDTO> SendEMIDueReminderEmailAsync(EMIDueReminderEmailDto dto);
        Task<BoolDTO> SendOverduePaymentEmailAsync(OverduePaymentEmailDto dto);
        Task<BoolDTO> SendLoanDisbursedEmailAsync(LoanDisbursedEmailDto dto);
        Task<BoolDTO> SendLoanFullyPaidEmailAsync(LoanFullyPaidEmailDto dto);
        Task<BoolDTO> SendOTPEmailAsync(OTPEmailDto dto);
    }
}