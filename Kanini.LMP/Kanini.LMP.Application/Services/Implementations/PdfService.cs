// using Kanini.LMP.Application.Services.Interfaces;
// using Kanini.LMP.Data.Data;
// using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
// using Kanini.LMP.Database.Enums;
// using Microsoft.EntityFrameworkCore;
// using System.Reflection.Metadata;
// using System.Text;

// namespace Kanini.LMP.Application.Services.Implementations
// {
//     public class PdfService : IPdfService
//     {
//         private readonly LmpDbContext _context;

//         public PdfService(LmpDbContext context)
//         {
//             _context = context;
//         }

//         public async Task<byte[]> GenerateLoanApplicationPdfAsync(int applicationId)
//         {
//             var application = await _context.LoanApplicationBases
//                 .Include(app => app.LoanDetails)
//                 .Include(app => app.PersonalDetails)
//                 .Include(app => app.AddressInformation)
//                 .Include(app => app.Customer)
//                 .FirstOrDefaultAsync(app => app.LoanApplicationBaseId == applicationId);

//             if (application == null)
//                 throw new ArgumentException("Application not found");

//             var pdfContent = GenerateApplicationPdfContent(application);
//             var pdfBytes = Encoding.UTF8.GetBytes(pdfContent);

//             // Store PDF in DocumentUpload table
//             var document = new DocumentUpload
//             {
//                 LoanApplicationBaseId = applicationId,
//                 CustomerId = application.CustomerId,
//                 DocumentName = $"LoanApplication_{applicationId}.pdf",
//                 DocumentType = DocumentType.ApplicationPDF,
//                 DocumentData = pdfBytes,
//                 UploadedAt = DateTime.UtcNow
//             };

//             var existing = await _context.DocumentUploads
//                 .FirstOrDefaultAsync(d => d.LoanApplicationBaseId == applicationId && d.DocumentType == DocumentType.ApplicationPDF);

//             if (existing != null)
//             {
//                 existing.DocumentData = pdfBytes;
//                 existing.UploadedAt = DateTime.UtcNow;
//             }
//             else
//             {
//                 await _context.DocumentUploads.AddAsync(document);
//             }

//             await _context.SaveChangesAsync();
//             return pdfBytes;
//         }

//         public async Task<byte[]> GeneratePaymentReceiptPdfAsync(int transactionId)
//         {
            
//         }

//         public async Task<byte[]> GenerateEMISchedulePdfAsync(int loanAccountId)
//         {
//             var loanAccount = await _context.LoanAccounts
//                 .FirstOrDefaultAsync(la => la.LoanAccountId == loanAccountId);

//             if (loanAccount == null)
//                 throw new ArgumentException("Loan account not found");

//             var scheduleContent = GenerateEMIScheduleContent(loanAccount);
//             return Encoding.UTF8.GetBytes(scheduleContent);
//         }

//         private string GenerateApplicationPdfContent(dynamic application)
//         {
//             var loanDetails = application.LoanDetails;
//             var personalDetails = application.PersonalDetails;
//             var addressInfo = application.AddressInformation;

//             return $@"
// ========================================
//     LOAN APPLICATION DOCUMENT
// ========================================

// Application ID: {application.LoanApplicationBaseId}
// Application Date: {application.SubmissionDate}
// Status: {application.Status}
// Loan Type: {application.LoanProductType}

// ----------------------------------------
// CUSTOMER INFORMATION
// ----------------------------------------
// Full Name: {personalDetails?.FullName ?? "N/A"}
// Email: {personalDetails?.Email ?? "N/A"}
// Phone: {personalDetails?.PhoneNumber ?? "N/A"}
// Date of Birth: {personalDetails?.DateOfBirth}
// Gender: {personalDetails?.Gender}

// ----------------------------------------
// ADDRESS INFORMATION
// ----------------------------------------
// Current Address: {addressInfo?.CurrentAddress ?? "N/A"}
// City: {addressInfo?.City ?? "N/A"}
// State: {addressInfo?.State ?? "N/A"}
// Pincode: {addressInfo?.Pincode ?? "N/A"}

// ----------------------------------------
// LOAN DETAILS
// ----------------------------------------
// Requested Amount: ₹{loanDetails?.RequestedAmount ?? 0:N2}
// Tenure: {loanDetails?.TenureMonths ?? 0} months
// Interest Rate: {loanDetails?.InterestRate ?? 0}%
// Monthly EMI: ₹{loanDetails?.MonthlyInstallment ?? 0:N2}
// Purpose: {loanDetails?.LoanPurpose ?? "N/A"}

// ----------------------------------------

// Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

// This is a system-generated document.

// ========================================
// ";
//         }

//         private string GeneratePaymentReceiptContent(dynamic transaction)
//         {
//             return $@"
// PAYMENT RECEIPT
// ==============

// Transaction ID: {transaction.TransactionId}
// Payment Date: {transaction.PaymentDate:dd MMM yyyy HH:mm}
// Amount: ₹{transaction.Amount:N2}
// Payment Method: {transaction.PaymentMethod}
// Status: {transaction.Status}
// Reference: {transaction.TransactionReference ?? "N/A"}
// Loan Account: {transaction.LoanAccountId}

// Thank you for your payment!

// Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
// ";
//         }

//         private string GenerateEMIScheduleContent(dynamic loanAccount)
//         {
//             return $@"
// EMI SCHEDULE
// ===========

// Loan Account ID: {loanAccount.LoanAccountId}
// Loan Amount: ₹{loanAccount.TotalLoanAmount:N2}

// PAYMENT STATUS
// -------------
// Total Paid (Principal): ₹{loanAccount.TotalPaidPrincipal:N2}
// Total Paid (Interest): ₹{loanAccount.TotalPaidInterest:N2}
// Remaining Principal: ₹{loanAccount.PrincipalRemaining:N2}
// Current Status: {loanAccount.CurrentPaymentStatus}

// Generated on: {DateTime.Now:dd MMM yyyy HH:mm}
// ";
//         }
//     }
// }