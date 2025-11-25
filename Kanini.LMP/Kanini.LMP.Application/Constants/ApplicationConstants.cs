namespace Kanini.LMP.Application.Constants
{
    public static class ApplicationConstants
    {
        public static class Roles
        {
            public const string Manager = "Manager";
            public const string Customer = "Customer";
        }

        public static class Claims
        {
            public const string UserId = "UserId";
            public const string Role = "Role";
            public const string Email = "Email";
            public const string CustomerId = "CustomerId";
        }

        public static class Messages
        {
            // Common Messages
            public const string Success = "Operation completed successfully";
            public const string Created = "Resource created successfully";
            public const string Updated = "Resource updated successfully";
            public const string Deleted = "Resource deleted successfully";
            public const string Unknown = "Unknown";

            // Document Messages
            public const string ProcessingDocumentUpload = "Processing document upload: {0} for user ID: {1}";
            public const string DocumentUploadCompleted = "Document upload completed with ID: {0}";
            public const string ProcessingDocumentDownload = "Processing document download for document ID: {0} by user ID: {1}";
            public const string DocumentDownloadCompleted = "Document download completed for ID: {0}";
            public const string ProcessingDocumentVerification = "Processing document verification for document ID: {0} by manager ID: {1}";
            public const string DocumentVerificationCompleted = "Document verification completed for document ID: {0}";
            public const string ProcessingDocumentDeletion = "Processing document deletion for document ID: {0} by user ID: {1}";
            public const string DocumentDeletionCompleted = "Document deletion completed for document ID: {0}";
            public const string ProcessingDocumentsRetrieval = "Processing documents retrieval for loan application ID: {0}";
            public const string DocumentsRetrievalCompleted = "Retrieved {0} documents for loan application ID: {1}";
            public const string ProcessingPendingDocuments = "Processing pending documents request";
            public const string PendingDocumentsCompleted = "Retrieved {0} pending documents";

            // Eligibility Messages
            public const string ProcessingEligibilityCalculation = "Processing eligibility calculation for customer ID: {0}, product ID: {1}";
            public const string EligibilityCalculationCompleted = "Eligibility calculation completed for customer ID: {0} with score: {1}";
            public const string ProcessingCustomerProfileUpdate = "Processing customer profile update for user ID: {0}";
            public const string CustomerProfileUpdatedSuccessfully = "Customer profile updated successfully for user ID: {0}";

            // EMI Messages
            public const string ProcessingEMICalculation = "Processing EMI calculation for amount: {0}, term: {1} months";
            public const string EMICalculationCompleted = "EMI calculation completed with monthly EMI: {0}";
            public const string ProcessingEMIDashboard = "Processing EMI dashboard request";
            public const string EMIDashboardCompleted = "EMI dashboard completed for customer ID: {0}";
            public const string ProcessingEMIRestructure = "Processing EMI restructure for EMI ID: {0}";
            public const string EMIRestructureCompleted = "EMI restructure completed for EMI ID: {0}";

            // KYC Messages
            public const string ProcessingKYCSubmission = "Processing KYC document submission for customer ID: {0}";
            public const string KYCSubmissionCompleted = "KYC document submitted successfully with ID: {0}";
            public const string ProcessingKYCStatusRetrieval = "Processing KYC status retrieval for customer ID: {0}";
            public const string KYCStatusRetrievalCompleted = "KYC status retrieved for customer ID: {0}";
            public const string ProcessingKYCVerification = "Processing KYC document verification for document ID: {0}";
            public const string KYCVerificationCompleted = "KYC document verified successfully for document ID: {0}";
            public const string ProcessingKYCRejection = "Processing KYC document rejection for document ID: {0}";
            public const string KYCRejectionCompleted = "KYC document rejected for document ID: {0}";
            public const string ProcessingKYCScoreCalculation = "Processing KYC score calculation for customer ID: {0}";
            public const string KYCScoreCalculationCompleted = "KYC score calculated for customer ID: {0} with score: {1}";

            // Loan Application Messages
            public const string ProcessingLoanCreation = "Processing loan creation for customer ID: {0}";
            public const string LoanCreationCompleted = "Loan created successfully with ID: {0}";
            public const string ProcessingLoanRetrieval = "Processing loan retrieval for ID: {0}";
            public const string LoanRetrievalCompleted = "Loan retrieved successfully for ID: {0}";
            public const string ProcessingLoanStatusUpdate = "Processing loan status update for ID: {0} to status: {1}";
            public const string LoanStatusUpdateCompleted = "Loan status updated successfully for ID: {0}";
            public const string LoanApplicationSubmittedSuccessfully = "Loan application submitted successfully. You will be notified about the status.";

            // Manager Dashboard Messages
            public const string ProcessingOverallMetrics = "Processing overall metrics request";
            public const string OverallMetricsCompleted = "Overall metrics retrieved successfully";
            public const string ProcessingApplicationStatusSummary = "Processing application status summary request";
            public const string ApplicationStatusSummaryCompleted = "Application status summary retrieved successfully";
            public const string ProcessingApplicationTrends = "Processing application trends request";
            public const string ApplicationTrendsCompleted = "Application trends retrieved successfully";
            public const string ProcessingLoanTypePerformance = "Processing loan type performance request";
            public const string LoanTypePerformanceCompleted = "Loan type performance retrieved successfully";
            public const string ProcessingNewApplicationsSummary = "Processing new applications summary request";
            public const string NewApplicationsSummaryCompleted = "New applications summary retrieved successfully";

            // Manager Workflow Messages
            public const string ProcessingPendingApplications = "Processing pending applications request";
            public const string PendingApplicationsCompleted = "Pending applications retrieved successfully";
            public const string ProcessingApplicationDetails = "Processing application details request for ID: {0}";
            public const string ApplicationDetailsCompleted = "Application details retrieved successfully for ID: {0}";
            public const string ProcessingWorkflowStart = "Processing workflow start for application ID: {0}, manager ID: {1}";
            public const string WorkflowStartCompleted = "Workflow started successfully for application ID: {0}";
            public const string ProcessingWorkflowStepUpdate = "Processing workflow step update for application ID: {0}";
            public const string WorkflowStepUpdateCompleted = "Workflow step updated successfully for application ID: {0}";
            public const string ProcessingWorkflowStatus = "Processing workflow status request for application ID: {0}";
            public const string WorkflowStatusCompleted = "Workflow status retrieved successfully for application ID: {0}";
            public const string ProcessingApplicationApproval = "Processing application approval for ID: {0}";
            public const string ApplicationApprovalCompleted = "Application approved successfully for ID: {0}";
            public const string ProcessingApplicationRejection = "Processing application rejection for ID: {0}";
            public const string ApplicationRejectionCompleted = "Application rejected successfully for ID: {0}";
            public const string ProcessingApplicationRevision = "Processing application revision for ID: {0}";
            public const string ApplicationRevisionCompleted = "Application revised successfully for ID: {0}";
            public const string ProcessingApplicationDisbursement = "Processing application disbursement for ID: {0}";
            public const string ApplicationDisbursementCompleted = "Application disbursed successfully for ID: {0}";

            // Razorpay Messages
            public const string ProcessingRazorpayOrderCreation = "Processing Razorpay order creation for amount: {0}";
            public const string RazorpayOrderCreationCompleted = "Razorpay order created successfully with ID: {0}";
            public const string ProcessingRazorpayPayment = "Processing Razorpay payment for payment ID: {0}";
            public const string RazorpayPaymentProcessingCompleted = "Razorpay payment processed successfully for payment ID: {0}";
            public const string ProcessingRazorpaySignatureVerification = "Processing Razorpay signature verification for payment ID: {0}";
            public const string RazorpaySignatureVerificationCompleted = "Razorpay signature verification completed for payment ID: {0}, result: {1}";
            public const string ProcessingRazorpayDisbursement = "Processing Razorpay disbursement for amount: {0}";
            public const string RazorpayDisbursementCompleted = "Razorpay disbursement completed with ID: {0}";
            public const string ProcessingRazorpayDisbursementStatus = "Processing Razorpay disbursement status for ID: {0}";
            public const string RazorpayDisbursementStatusCompleted = "Razorpay disbursement status retrieved for ID: {0}";

            // Missing Messages
            public const string NotFound = "Resource not found";
            public const string Unauthorized = "Unauthorized access";
            public const string BadRequest = "Invalid request";
            public const string InternalServerError = "Internal server error";
            public const string DocumentNotFound = "Document not found";
            public const string DocumentDeletedSuccessfully = "Document deleted successfully";
            public const string DocumentUploadedSuccessfully = "Document uploaded successfully with ID: {0}";
            public const string DocumentVerifiedSuccessfully = "Document verified successfully with ID: {0}";
            public const string PendingDocumentsRetrieved = "Retrieved {0} pending documents successfully";
            public const string RetrievingDocumentsByApplication = "Retrieving documents for application ID: {0}";
            public const string RetrievingPendingDocuments = "Retrieving pending documents";
            public const string ProcessingApplicantsRetrieval = "Processing applicants retrieval for loan application ID: {0}";
            public const string ApplicantsRetrievalCompleted = "Retrieved {0} applicants for loan application ID: {1}";
            public const string ProcessingEMIPlanCreation = "Processing EMI plan creation for loan application ID: {0}";
            public const string EMIPlanCreatedSuccessfully = "EMI plan created successfully with ID: {0}";
            public const string ProcessingEMIPlanRetrieval = "Processing EMI plan retrieval for ID: {0}";
            public const string EMIRestructureAppliedSuccessfully = "EMI restructure applied successfully for EMI ID: {0}";
            public const string ProcessingPendingKYCRetrieval = "Processing pending KYC documents retrieval";
            public const string PendingKYCRetrievalCompleted = "Retrieved {0} pending KYC documents";
            public const string ProcessingPersonalLoanCreation = "Processing personal loan creation for customer ID: {0}";
            public const string PersonalLoanCreationCompleted = "Personal loan created successfully with ID: {0}";
            public const string ProcessingHomeLoanCreation = "Processing home loan creation for customer ID: {0}";
            public const string HomeLoanCreationCompleted = "Home loan created successfully with ID: {0}";
            public const string ProcessingVehicleLoanCreation = "Processing vehicle loan creation for customer ID: {0}";
            public const string VehicleLoanCreationCompleted = "Vehicle loan created successfully with ID: {0}";
            public const string DocumentUploadRequested = "Document upload requested";
            public const string DocumentDownloadRequested = "Document download requested";
            public const string DocumentVerificationRequested = "Document verification requested";
            public const string DocumentDeletionRequested = "Document deletion requested";
            public const string DocumentViewRequested = "Document view requested";
            public const string DocumentViewCompleted = "Document view completed";
            public const string DocumentsByApplicationRequested = "Documents by application requested";
            public const string DocumentsByApplicationCompleted = "Documents by application completed";
            public const string PendingDocumentsRequested = "Pending documents requested";
            public const string EligibilityCheckRequested = "Eligibility check requested";
            public const string EligibilityCheckCompleted = "Eligibility check completed";
            public const string EMICalculationRequested = "EMI calculation requested";
            public const string EMIDashboardRequested = "EMI dashboard requested";
            public const string EMIRestructureRequested = "EMI restructure requested";
            public const string ProcessingEMIPlansRetrieval = "Processing EMI plans retrieval";
            public const string EMIPlansRetrievalCompleted = "EMI plans retrieval completed";
            public const string ProcessingNotificationsRetrieval = "Processing notifications retrieval";
            public const string NotificationsRetrievalCompleted = "Notifications retrieval completed";
            public const string ProcessingMarkAsRead = "Processing mark as read";
            public const string MarkAsReadCompleted = "Mark as read completed";
            public const string ProcessingMarkAllAsRead = "Processing mark all as read";
            public const string MarkAllAsReadCompleted = "Mark all as read completed";
            public const string ProcessingUnreadCount = "Processing unread count";
            public const string UnreadCountCompleted = "Unread count completed";
            public const string ProcessingUnreadNotifications = "Processing unread notifications";
            public const string UnreadNotificationsCompleted = "Unread notifications completed";
            public const string ProcessingPreferencesRetrieval = "Processing preferences retrieval";
            public const string PreferencesRetrievalCompleted = "Preferences retrieval completed";
            public const string ProcessingPreferencesUpdate = "Processing preferences update";
            public const string PreferencesUpdateCompleted = "Preferences update completed";
            public const string ProcessingTestNotification = "Processing test notification";
            public const string TestNotificationCompleted = "Test notification completed";
            public const string ProcessingBulkNotification = "Processing bulk notification";
            public const string BulkNotificationCompleted = "Bulk notification completed";
            public const string ProcessingOrderCreation = "Processing order creation";
            public const string OrderCreationCompleted = "Order creation completed";
            public const string ProcessingPaymentCreation = "Processing payment creation";
            public const string PaymentCreationCompleted = "Payment creation completed";
            public const string ProcessingPaymentProcessing = "Processing payment processing";
            public const string PaymentProcessingCompleted = "Payment processing completed";
            public const string ProcessingSignatureVerification = "Processing signature verification";
            public const string SignatureVerificationCompleted = "Signature verification completed";
            public const string ProcessingPaymentsRetrieval = "Processing payments retrieval";
            public const string PaymentsRetrievalCompleted = "Payments retrieval completed";
            public const string ProcessingPaymentStatusUpdate = "Processing payment status update";
            public const string PaymentStatusUpdateCompleted = "Payment status update completed";
            public const string DocumentNotFoundOrUnauthorized = "Document not found or unauthorized";

            // Reports Messages
            public const string ProcessingLoanPerformanceReport = "Processing loan performance report from {0} to {1}";
            public const string LoanPerformanceReportCompleted = "Loan performance report completed";
            public const string ProcessingRiskAssessmentReport = "Processing risk assessment report from {0} to {1}";
            public const string RiskAssessmentReportCompleted = "Risk assessment report completed";
            public const string ProcessingComplianceReport = "Processing compliance report";
            public const string ComplianceReportCompleted = "Compliance report completed";
            public const string ProcessingCustomerAnalytics = "Processing customer analytics dashboard";
            public const string CustomerAnalyticsCompleted = "Customer analytics dashboard completed";
            public const string ProcessingAnalyticsDashboard = "Processing analytics dashboard";
            public const string AnalyticsDashboardCompleted = "Analytics dashboard completed";
            public const string ProcessingReportExport = "Processing report export for {0} in {1} format";
            public const string ReportExportCompleted = "Report export completed for {0}";

            // Token Messages
            public const string ProcessingLogin = "Processing login for user: {0}";
            public const string LoginCompleted = "Login completed for user: {0}";
            public const string LoginFailed = "Login failed for user: {0}";
            public const string LoginValidationFailed = "Login validation failed";
            public const string ProcessingRegistration = "Processing registration for email: {0}";
            public const string RegistrationCompleted = "Registration completed for email: {0}";
            public const string RegistrationValidationFailed = "Registration validation failed";
            public const string CustomerRegisteredSuccessfully = "Customer registered successfully";
            public const string ProcessingForgotPassword = "Processing forgot password for email: {0}";
            public const string ForgotPasswordCompleted = "Forgot password completed for email: {0}";
            public const string ForgotPasswordEmailNotFound = "Forgot password email not found: {0}";
            public const string ForgotPasswordValidationFailed = "Forgot password validation failed";
            public const string PasswordResetEmailSent = "Password reset email sent successfully";
            public const string ProcessingResetPassword = "Processing reset password for email: {0}";
            public const string ResetPasswordCompleted = "Reset password completed for email: {0}";
            public const string ResetPasswordTokenInvalid = "Reset password token invalid for email: {0}";
            public const string ResetPasswordValidationFailed = "Reset password validation failed";
            public const string PasswordResetSuccessfully = "Password reset successfully";

            // User Messages
            public const string ProcessingUsersRetrieval = "Processing users retrieval";
            public const string UsersRetrievalCompleted = "Users retrieval completed, found {0} users";
            public const string ProcessingUserRetrieval = "Processing user retrieval for ID: {0}";
            public const string UserRetrievalCompleted = "User retrieval completed for ID: {0}";
            public const string UserNotFoundWarning = "User not found for ID: {0}";
            public const string ProcessingUserCreation = "Processing user creation for email: {0}";
            public const string UserCreationCompleted = "User creation completed with ID: {0}";
            public const string ProcessingPasswordChange = "Processing password change for user ID: {0}";
            public const string PasswordChangeCompleted = "Password change completed for user ID: {0}";
            public const string PasswordChangeIncorrectCurrent = "Password change failed - incorrect current password for user ID: {0}";
            public const string PasswordChangedSuccessfully = "Password changed successfully";

            // PDF Messages
            public const string ProcessingPdfGeneration = "Processing {0} PDF generation for ID: {1}";
            public const string PdfGenerationCompleted = "{0} PDF generation completed for ID: {1}";
            public const string PdfDataNotFound = "{0} not found for PDF generation, ID: {1}";
            
            // Stored Procedure Messages
            public const string ProcessingLoanTypePerformanceSP = "Processing loan type performance via stored procedure";
            public const string LoanTypePerformanceSPCompleted = "Loan type performance stored procedure completed";
        }

        public static class ErrorMessages
        {
            // Common Errors
            public const string InternalServerError = "Internal server error";
            public const string NotFound = "Resource not found";
            public const string Unauthorized = "Unauthorized access";
            public const string Forbidden = "Access forbidden";
            public const string BadRequest = "Invalid request";

            // Entity Not Found Errors
            public const string CustomerNotFound = "Customer not found";
            public const string UserNotFound = "User not found";
            public const string LoanApplicationNotFound = "Loan application not found with ID: {0}";
            public const string LoanApplicationsNotFound = "Loan applications not found";
            public const string PaymentNotFound = "Payment not found";
            public const string NotificationNotFound = "Notification not found";
            public const string DocumentNotFound = "Document not found with ID: {0}";
            public const string KYCDocumentNotFound = "KYC document not found with ID: {0}";
            public const string EMIPlanNotFound = "EMI Plan not found";

            // Validation Errors
            public const string InvalidUserId = "Invalid user ID";
            public const string InvalidCustomerId = "Invalid customer ID";
            public const string CustomerIdNotFound = "Customer ID not found in token";
            public const string EligibilityValidationFailed = "For new users, PAN, Age, Annual Income, Occupation, and Home Ownership Status are required";
            public const string IneligibleForLoan = "Customer is not eligible for this loan type. Please improve your profile and try again.";

            // Access Control Errors
            public const string AccessDenied = "You can only access your own profile";
            public const string CreateProfileDenied = "You can only create your own customer profile";
            public const string UpdateProfileDenied = "You can only update your own profile";
            public const string DocumentNotFoundOrUnauthorized = "Document not found or unauthorized for document ID: {0} and user ID: {1}";

            // Document Operation Errors
            public const string DocumentUploadFailed = "Failed to upload document";
            public const string DocumentDownloadFailed = "Failed to download document with ID: {0}";
            public const string DocumentVerificationFailed = "Failed to verify document with ID: {0}";
            public const string DocumentDeletionFailed = "Failed to delete document with ID: {0}";
            public const string DocumentViewFailed = "Failed to view document with ID: {0}";
            public const string DocumentsRetrievalFailed = "Failed to retrieve documents for loan application ID: {0}";

            // Eligibility Errors
            public const string EligibilityCheckFailed = "Failed to check eligibility";
            public const string EligibilityCalculationFailed = "Failed to calculate eligibility for customer ID: {0}";
            public const string CustomerProfileUpdateFailed = "Failed to update customer profile for user ID: {0}";

            // EMI Errors
            public const string EMICalculationFailed = "Failed to calculate EMI";
            public const string EMIDashboardFailed = "Failed to retrieve EMI dashboard";
            public const string EMIRestructureFailed = "Failed to restructure EMI for ID: {0}";
            public const string NoActiveEMIFound = "No active EMI found for customer";
            public const string EMIPlanCreationFailed = "Failed to create EMI plan";
            public const string EMIPlanRetrievalFailed = "Failed to retrieve EMI plan for ID: {0}";

            // KYC Errors
            public const string KYCSubmissionFailed = "Failed to submit KYC document";
            public const string KYCStatusRetrievalFailed = "Failed to retrieve KYC status for customer ID: {0}";
            public const string KYCVerificationFailed = "Failed to verify KYC document with ID: {0}";
            public const string KYCRejectionFailed = "Failed to reject KYC document with ID: {0}";
            public const string KYCScoreCalculationFailed = "Failed to calculate KYC score for customer ID: {0}";

            // Loan Application Errors
            public const string LoanCreationFailed = "Failed to create loan for customer ID: {0}";
            public const string LoanRetrievalFailed = "Failed to retrieve loan with ID: {0}";
            public const string LoanStatusUpdateFailed = "Failed to update loan status for ID: {0}";

            // Manager Dashboard Errors
            public const string OverallMetricsFailed = "Failed to retrieve overall metrics";
            public const string ApplicationStatusSummaryFailed = "Failed to retrieve application status summary";
            public const string ApplicationTrendsFailed = "Failed to retrieve application trends";
            public const string LoanTypePerformanceFailed = "Failed to retrieve loan type performance";
            public const string NewApplicationsSummaryFailed = "Failed to retrieve new applications summary";

            // Manager Workflow Errors
            public const string PendingApplicationsFailed = "Failed to retrieve pending applications";
            public const string ApplicationDetailsFailed = "Failed to retrieve application details for ID: {0}";
            public const string WorkflowStartFailed = "Failed to start workflow for application ID: {0}";
            public const string WorkflowStepUpdateFailed = "Failed to update workflow step for application ID: {0}";
            public const string WorkflowStatusFailed = "Failed to retrieve workflow status for application ID: {0}";
            public const string ApplicationApprovalFailed = "Failed to approve application for ID: {0}";
            public const string ApplicationRejectionFailed = "Failed to reject application for ID: {0}";
            public const string ApplicationRevisionFailed = "Failed to revise application for ID: {0}";
            public const string ApplicationDisbursementFailed = "Failed to disburse application for ID: {0}";

            // Success Messages (moved from error messages)
            public const string DocumentsVerifiedSuccessfully = "Documents verified successfully";
            public const string ApplicationApprovedSuccessfully = "Application approved successfully";
            public const string ApplicationRejected = "Application rejected";
            public const string ApplicationRevisedSuccessfully = "Application revised successfully";
            public const string KYCVerificationSuccess = "KYC document verified successfully";
            public const string KYCRejectionSuccess = "KYC document rejected";
            public const string LoanDisbursedSuccessfully = "Loan disbursed successfully";

            // Missing Error Constants
            public const string DocumentLinkNotFound = "Document link not found";
            public const string RetrieveDocumentsFailed = "Failed to retrieve documents";
            public const string RetrievePendingDocumentsFailed = "Failed to retrieve pending documents";
            public const string ApplicantsRetrievalFailed = "Failed to retrieve applicants for loan application ID: {0}";
            public const string EMIRestructureApplicationFailed = "Failed to apply EMI restructure for ID: {0}";
            public const string PendingKYCRetrievalFailed = "Failed to retrieve pending KYC documents";
            public const string PersonalLoanCreationFailed = "Failed to create personal loan for customer ID: {0}";
            public const string HomeLoanCreationFailed = "Failed to create home loan for customer ID: {0}";
            public const string VehicleLoanCreationFailed = "Failed to create vehicle loan for customer ID: {0}";
            public const string KYCSubmissionSuccess = "KYC document submitted successfully";
            public const string VerifiedPrefix = "[VERIFIED]";
            public const string RejectedPrefix = "[REJECTED]";

            // Razorpay Error Messages
            public const string RazorpayOrderCreationFailed = "Failed to create Razorpay order";
            public const string RazorpayPaymentProcessingFailed = "Failed to process Razorpay payment";
            public const string RazorpaySignatureVerificationFailed = "Failed to verify Razorpay signature";
            public const string RazorpayDisbursementFailed = "Failed to create Razorpay disbursement";
            public const string RazorpayDisbursementStatusFailed = "Failed to retrieve Razorpay disbursement status";

            // Additional Error Messages
            public const string EMIPlansRetrievalFailed = "Failed to retrieve EMI plans";
            public const string NotificationsRetrievalFailed = "Failed to retrieve notifications";
            public const string MarkAsReadFailed = "Failed to mark as read";
            public const string MarkAllAsReadFailed = "Failed to mark all as read";
            public const string UnreadCountFailed = "Failed to get unread count";
            public const string UnreadNotificationsFailed = "Failed to get unread notifications";
            public const string PreferencesRetrievalFailed = "Failed to retrieve preferences";
            public const string PreferencesUpdateFailed = "Failed to update preferences";
            public const string TestNotificationFailed = "Failed to send test notification";
            public const string BulkNotificationFailed = "Failed to send bulk notification";
            public const string OrderCreationFailed = "Failed to create order";
            public const string PaymentCreationFailed = "Failed to create payment";
            public const string PaymentProcessingFailed = "Failed to process payment";
            public const string SignatureVerificationFailed = "Failed to verify signature";
            public const string PaymentsRetrievalFailed = "Failed to retrieve payments";
            public const string PaymentStatusUpdateFailed = "Failed to update payment status";
            public const string InvalidPaymentSignature = "Invalid payment signature";
            public const string LoanDisbursementFailed = "Failed to disburse loan";
            public const string DisbursementStatusFailed = "Failed to get disbursement status";
            public const string UserIdsRequired = "User IDs are required";
            public const string AllNotificationsMarkedAsRead = "All notifications marked as read";
            public const string BulkNotificationSent = "Bulk notification sent";
            public const string TestNotificationSent = "Test notification sent";
            public const string PaymentProcessedSuccessfully = "Payment processed successfully";
            public const string PreferencesUpdatedSuccessfully = "Preferences updated successfully";

            // Reports Error Messages
            public const string LoanPerformanceReportFailed = "Failed to generate loan performance report";
            public const string RiskAssessmentReportFailed = "Failed to generate risk assessment report";
            public const string ComplianceReportFailed = "Failed to generate compliance report";
            public const string CustomerAnalyticsFailed = "Failed to generate customer analytics";
            public const string AnalyticsDashboardFailed = "Failed to generate analytics dashboard";
            public const string ReportExportFailed = "Failed to export report";
            public const string InvalidReportType = "Invalid report type";

            // Token Error Messages
            public const string UsernamePasswordRequired = "Username and password are required";
            public const string InvalidCredentials = "Invalid credentials";
            public const string LoginProcessingFailed = "Failed to process login";
            public const string RegistrationFailed = "Registration failed";
            public const string EmailRequired = "Email is required";
            public const string EmailNotFound = "Email not found";
            public const string ForgotPasswordFailed = "Failed to process forgot password";
            public const string ResetPasswordFieldsRequired = "Email, reset token, and new password are required";
            public const string InvalidOrExpiredResetToken = "Invalid or expired reset token";
            public const string ResetPasswordFailed = "Failed to reset password";

            // User Error Messages
            public const string UsersRetrievalFailed = "Failed to retrieve users";
            public const string UserRetrievalFailed = "Failed to retrieve user";
            public const string UserCreationFailed = "Failed to create user";
            public const string UserCreationValidationFailed = "User creation validation failed";
            public const string PasswordChangeFailed = "Failed to change password";
            public const string PasswordChangeValidationFailed = "Password change validation failed";
            public const string CurrentPasswordIncorrect = "Current password is incorrect";

            // PDF Error Messages
            public const string PdfGenerationFailed = "Failed to generate PDF";
            public const string LoanAccountNotFound = "Loan account not found";
            
            // Stored Procedure Error Messages
            public const string LoanTypePerformanceSPFailed = "Failed to retrieve loan type performance via stored procedure";
        }

        public static class Routes
        {
            public const string DocumentController = "api/[controller]";
            public const string EligibilityController = "api/[controller]";
            public const string EmiCalculatorController = "api/[controller]";
            public const string JointLoanController = "api/[controller]";
            public const string Upload = "upload";
            public const string DownloadDocument = "download/{documentId}";
            public const string ViewDocument = "view/{documentId}";
            public const string DocumentsByApplication = "application/{loanApplicationBaseId}";

            public const string DeleteDocument = "delete/{documentId}";
            public const string Pending = "pending";
            public const string Check = "check";
            public const string Calculate = "calculate";
            public const string Dashboard = "dashboard";
            public const string RestructureApply = "restructure/apply";
            public const string Applicants = "{loanApplicationId}/applicants";
            public const string Documents = "{loanApplicationId}/documents";
            public const string KYCController = "api/[controller]";
            public const string SubmitDocument = "submit-document";
            public const string Status = "status/{customerId}";
            public const string MyStatus = "my-status";
            public const string IsCompleted = "is-completed/{customerId}";
            public const string PendingDocuments = "pending-documents";
            public const string Document = "document/{documentId}";
            public const string Verify = "verify";
            public const string KYCReject = "reject";
            public const string Reject = "reject"; // For backward compatibility
            public const string Score = "score/{customerId}";
            public const string LoanApplicationController = "api/[controller]";
            public const string Personal = "personal";
            public const string PersonalById = "personal/{id}";
            public const string PersonalByStatus = "personal/status/{status}";
            public const string PersonalSubmit = "personal/{customerId}/submit";
            public const string PersonalUpdateStatus = "personal/{id}/status";
            public const string Home = "home/{customerId}";
            public const string Vehicle = "vehicle/{customerId}";
            public const string ManagerDashboardController = "api/[controller]";
            public const string OverallMetrics = "overall-metrics";
            public const string ApplicationStatus = "application-status";
            public const string ApplicationTrends = "application-trends";
            public const string LoanTypePerformance = "loan-type-performance";
            public const string NewApplications = "new-applications";
            public const string ManagerWorkflowController = "api/[controller]";
            public const string PendingApplications = "pending-applications";
            public const string Application = "application/{applicationId}";
            public const string StartWorkflow = "start-workflow/{applicationId}/{managerId}";
            public const string UpdateStep = "update-step/{applicationId}";
            public const string WorkflowStatus = "workflow-status/{applicationId}";
            public const string VerifyDocuments = "verify-documents/{applicationId}/{managerId}";
            public const string Approve = "approve/{applicationId}/{managerId}";
            public const string WorkflowReject = "reject/{applicationId}/{managerId}";
            public const string Revise = "revise/{applicationId}/{managerId}";
            public const string Disburse = "disburse/{applicationId}/{managerId}";

            public const string NotificationController = "api/[controller]";
            public const string PaymentController = "api/[controller]";
            public const string MyNotifications = "my-notifications";
            public const string MarkRead = "mark-read/{notificationId}";
            public const string MarkAllRead = "mark-all-read";
            public const string UnreadCount = "unread-count";
            public const string Unread = "unread";
            public const string Preferences = "preferences";
            public const string PreferencesUpdate = "preferences/{userId}";
            public const string Test = "test";
            public const string Bulk = "bulk";
            public const string EMIPlans = "emi-plans";
            public const string LoanAccount = "loan-account/{customerId}";
            public const string PaymentStatus = "payment-status/{paymentId}";

            public static class RazorpayController
            {
                public const string Base = "api/[controller]";
                public const string CreateOrder = "create-order";
                public const string ProcessPayment = "process-payment";
                public const string VerifySignature = "verify-signature";
                public const string DisburseLoan = "disburse-loan";
                public const string GetDisbursementStatus = "disbursement-status/{disbursementId}";
            }

            public static class ReportsController
            {
                public const string Base = "api/[controller]";
                public const string LoanPerformance = "loan-performance";
                public const string RiskAssessment = "risk-assessment";
                public const string Compliance = "compliance";
                public const string CustomerAnalytics = "customer-analytics";
                public const string AnalyticsDashboard = "analytics-dashboard";
                public const string Export = "export/{reportType}";
            }

            public static class TokenController
            {
                public const string Base = "api/[controller]";
                public const string Login = "login";
                public const string Register = "register";
                public const string ForgotPassword = "forgot-password";
                public const string ResetPassword = "reset-password";
            }

            public static class UserController
            {
                public const string Base = "api/[controller]";
                public const string GetById = "{id}";
                public const string ChangePassword = "change-password";
            }
        }

        public static class ReportTypes
        {
            public const string LoanPerformance = "loan-performance";
            public const string RiskAssessment = "risk-assessment";
            public const string Compliance = "compliance";
        }

        public static class ReportFormats
        {
            public const string Json = "json";
            public const string Csv = "csv";
            public const string Pdf = "pdf";
        }

        public static class Headers
        {
            public const string ContentDisposition = "Content-Disposition";
            public const string Inline = "inline";
        }

        public static class FileExtensions
        {
            public const string Pdf = ".pdf";
            public const string Jpg = ".jpg";
            public const string Jpeg = ".jpeg";
            public const string Png = ".png";
            public const string Gif = ".gif";
            public const string Txt = ".txt";
            public const string Doc = ".doc";
            public const string Docx = ".docx";
        }

        public static class ContentTypes
        {
            public const string Pdf = "application/pdf";
            public const string Jpeg = "image/jpeg";
            public const string Png = "image/png";
            public const string Gif = "image/gif";
            public const string Text = "text/plain";
            public const string Doc = "application/msword";
            public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public const string OctetStream = "application/octet-stream";
        }

        public static class KYCDocumentTypes
        {
            public const string Prefix = "KYC_";
            public const string Aadhaar = "KYC_Aadhaar";
            public const string PAN = "KYC_PAN";
        }

        public static class KYCStatus
        {
            public const string Pending = "Pending";
            public const string Submitted = "Submitted";
            public const string Verified = "Verified";
            public const string Rejected = "Rejected";
        }

        public static class NotificationTitles
        {
            public const string KYCDocumentVerified = "KYC Document Verified";
            public const string KYCDocumentRejected = "KYC Document Rejected";
        }

        public static class NotificationMessages
        {
            public const string KYCDocumentVerifiedMessage = "Your {0} document has been verified successfully.";
            public const string KYCDocumentRejectedMessage = "Your {0} document has been rejected. Reason: {1}";
            public const string DefaultRejectionReason = "Please resubmit with correct information.";
        }
    }
}