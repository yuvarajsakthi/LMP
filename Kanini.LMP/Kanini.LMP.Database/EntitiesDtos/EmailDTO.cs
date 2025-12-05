using System.ComponentModel.DataAnnotations;

namespace Kanini.LMP.Database.EntitiesDto.Email
{
    public class EmailDto
    {
        [Required]
        [EmailAddress]
        public string To { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        public bool IsHtml { get; set; } = true;

        public List<EmailAttachment>? Attachments { get; set; }
    }

    public class EmailAttachment
    {
        [Required]
        public string FileName { get; set; } = null!;

        [Required]
        public byte[] Content { get; set; } = null!;

        public string ContentType { get; set; } = "application/octet-stream";
    }

    public class LoanApplicationSubmittedEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int ApplicationId { get; set; }
        public string LoanType { get; set; } = null!;
        public decimal Amount { get; set; }
        public byte[] ApplicationPdf { get; set; } = null!;
    }

    public class LoanApprovedEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int ApplicationId { get; set; }
        public decimal Amount { get; set; }
        public string LoanType { get; set; } = null!;
    }

    public class LoanRejectedEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int ApplicationId { get; set; }
        public string Reason { get; set; } = null!;
    }

    public class PaymentSuccessEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string EmiDetails { get; set; } = null!;
        public DateTime PaymentDate { get; set; }
    }

    public class PaymentFailedEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string EmiDetails { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }

    public class EMIDueReminderEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysUntilDue { get; set; }
    }

    public class OverduePaymentEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int DaysPastDue { get; set; }
    }

    public class LoanDisbursedEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public decimal Amount { get; set; }
        public int LoanAccountId { get; set; }
        public DateTime DisbursementDate { get; set; }
    }

    public class LoanFullyPaidEmailDto
    {
        public string CustomerEmail { get; set; } = null!;
        public string CustomerName { get; set; } = null!;
        public int LoanAccountId { get; set; }
        public decimal TotalAmountPaid { get; set; }
    }

    public class OTPEmailDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string OTP { get; set; } = null!;
        public string Purpose { get; set; } = null!;
    }

}