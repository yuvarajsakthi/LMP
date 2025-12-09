namespace Kanini.LMP.Api.Constants
{
    public static class ApiConstants
    {
        public static class Routes
        {
            public const string ApiController = "api/[controller]";
            
            
            // Customer routes
            public static class CustomerController
            {
                public const string GetAll = "";
                public const string GetById = "{id}";
                public const string GetByUserId = "user/{userId}";
                public const string GetSettings = "settings/{userId}";
                public const string Update = "{id}";
                public const string UpdateSettings = "settings/{userId}";
            }
            
            // Customer Dashboard routes
            public static class CustomerDashboardController
            {
                public const string Base = "customerdashboard";
                public const string LoanProducts = "loanproducts";
                public const string RecentAppliedLoans = "recent-applied-loans";
                public const string ApplicationStatus = "applicationstatus";
            }
            
            // Eligibility routes
            public static class EligibilityController
            {
                public const string GetScore = "{customerId}";
                public const string Calculate = "calculate/{customerId}";
                public const string Update = "update/{customerId}";
                public const string Check = "check";
            }
            
            // Loan Application Flow routes
            public static class LoanApplicationFlowController
            {
                public const string CreatePersonal = "personal/{customerId}";
                public const string CreateHome = "home/{customerId}";
                public const string CreateVehicle = "vehicle/{customerId}";
                public const string UpdateStatus = "{loanId}/status";
                public const string Withdraw = "{loanId}";
            }
            
            // Notification routes
            public static class NotificationController
            {
                public const string GetAll = "";
                public const string Delete = "{notificationId}";
            }
            
            // FAQ routes
            public static class FaqController
            {
                public const string Create = "";
                public const string GetAll = "";
                public const string GetById = "{id}";
                public const string GetByCustomer = "customer/{customerId}";
                public const string Update = "{id}";
                public const string Delete = "{id}";
            }
            
            // Token routes  
            public static class TokenController
            {
                public const string Login = "login";
                public const string Register = "register";
                public const string SendOTP = "sendotp";
                public const string LoginWithOTP = "login/otp";
                public const string VerifyOTP = "verify/otp";
                public const string ForgotPassword = "forgot-password";
                public const string ResetPassword = "reset-password";
            }
            
            // Manager Dashboard routes
            public static class ManagerDashboardController
            {
                public const string Stats = "stats";
                public const string Loans = "loans";
                public const string LoanById = "loans/{id}";
                public const string LoansByStatus = "loans/status/{status}";
                public const string UpdateStatus = "loans/status";
                public const string DisburseLoan = "loans/{id}/disburse";
            }
        }

        public static class LogMessages
        {
            // Customer Controller
            public const string GettingAllCustomers = "Getting all customers";
            public const string RetrievedCustomers = "Retrieved {Count} customers";
            public const string ErrorRetrievingAllCustomers = "Error retrieving all customers";
            public const string GettingCustomerById = "Getting customer with ID: {CustomerId}";
            public const string CustomerNotFoundById = "Customer with ID {CustomerId} not found";
            public const string ErrorRetrievingCustomerById = "Error retrieving customer with ID: {CustomerId}";
            public const string GettingCustomerByUserId = "Getting customer by user ID: {UserId}";
            public const string UnauthorizedProfileAccess = "User {CurrentUserId} attempted to access profile of user {UserId}";
            public const string CustomerNotFoundByUserId = "Customer with user ID {UserId} not found";
            public const string ErrorRetrievingCustomerByUserId = "Error retrieving customer by user ID: {UserId}";
            public const string CreatingCustomerProfile = "Creating customer profile for user ID: {UserId}";
            public const string UnauthorizedProfileCreation = "User {CurrentUserId} attempted to create profile for user {UserId}";
            public const string CustomerProfileCreated = "Customer profile created successfully with ID: {CustomerId}";
            public const string ErrorCreatingCustomerProfile = "Error creating customer profile for user ID: {UserId}";
            public const string IdMismatch = "ID mismatch: URL ID {UrlId} vs DTO ID {DtoId}";
            public const string UpdatingCustomerProfile = "Updating customer profile with ID: {CustomerId}";
            public const string UnauthorizedProfileUpdate = "User {CurrentUserId} attempted to update profile of user {UserId}";
            public const string CustomerProfileUpdated = "Customer profile updated successfully with ID: {CustomerId}";
            public const string ErrorUpdatingCustomerProfile = "Error updating customer profile with ID: {CustomerId}";
            public const string DeletingCustomer = "Deleting customer with ID: {CustomerId}";
            public const string CustomerDeleted = "Customer deleted successfully with ID: {CustomerId}";
            public const string ErrorDeletingCustomer = "Error deleting customer with ID: {CustomerId}";
            public const string InvalidUserIdClaim = "Invalid user ID claim: {UserIdClaim}";
            public const string NoRoleClaimFound = "No role claim found for user";

            // Document Controller
            public const string DocumentUploadRequested = "Document upload requested for: {DocumentName}";
            public const string DocumentUploadCompleted = "Document upload completed with ID: {DocumentId}";
            public const string DocumentDownloadRequested = "Document download requested for ID: {DocumentId}";
            public const string DocumentViewRequested = "Document view requested for ID: {DocumentId}";
            public const string DocumentViewCompleted = "Document view completed for ID: {DocumentId}";
            public const string DocumentsByApplicationRequested = "Documents by application requested for: {LoanApplicationBaseId}";
            public const string DocumentsByApplicationCompleted = "Documents by application completed, found {Count} documents for: {LoanApplicationBaseId}";
            public const string DocumentVerificationRequested = "Document verification requested for ID: {DocumentId}";
            public const string DocumentVerificationCompleted = "Document verification completed for ID: {DocumentId}";
            public const string DocumentDeletionRequested = "Document deletion requested for ID: {DocumentId}";
            public const string DocumentDeletionCompleted = "Document deletion completed for ID: {DocumentId}";
            public const string DocumentNotFoundOrUnauthorized = "Document not found or unauthorized for document ID: {DocumentId} and user ID: {UserId}";
            public const string PendingDocumentsRequested = "Pending documents requested";
            public const string PendingDocumentsCompleted = "Pending documents completed, found {Count} documents";

            // Eligibility Controller
            public const string EligibilityCheckRequested = "Eligibility check requested for existing borrower: {IsExistingBorrower}";
            public const string EligibilityCheckCompleted = "Eligibility check completed for user ID: {UserId}, score: {EligibilityScore}";

            // EMI Calculator Controller
            public const string EMICalculationRequested = "EMI calculation requested for amount: {PrincipalAmount}, term: {TermMonths}";
            public const string EMICalculationCompleted = "EMI calculation completed with monthly EMI: {MonthlyEMI}";
            public const string EMIDashboardRequested = "EMI dashboard requested";
            public const string EMIDashboardCompleted = "EMI dashboard completed for customer ID: {CustomerId}";
            public const string EMIRestructureRequested = "EMI restructure requested for EMI ID: {EMIId}";
            public const string EMIRestructureCompleted = "EMI restructure completed for EMI ID: {EMIId}";

            // Joint Loan Controller
            public const string ApplicantsRetrievalRequested = "Applicants retrieval requested for loan application ID: {LoanApplicationId}";
            public const string ApplicantsRetrievalCompleted = "Retrieved {Count} applicants for loan application ID: {LoanApplicationId}";
            public const string DocumentsRetrievalRequested = "Documents retrieval requested for loan application ID: {LoanApplicationId}";
            public const string DocumentsRetrievalCompleted = "Retrieved {Count} documents for loan application ID: {LoanApplicationId}";

            // KYC Controller
            public const string KYCSubmissionRequested = "KYC submission requested for customer ID: {CustomerId}";
            public const string KYCSubmissionCompleted = "KYC submission completed for customer ID: {CustomerId}";
            public const string KYCStatusRetrievalRequested = "KYC status retrieval requested for customer ID: {CustomerId}";
            public const string KYCStatusRetrievalCompleted = "KYC status retrieval completed for customer ID: {CustomerId}";
            public const string PendingKYCRetrievalRequested = "Pending KYC retrieval requested";
            public const string PendingKYCRetrievalCompleted = "Retrieved {Count} pending KYC documents";
            public const string KYCVerificationRequested = "KYC verification requested for document ID: {DocumentId}";
            public const string KYCVerificationCompleted = "KYC verification completed for document ID: {DocumentId}";
            public const string KYCRejectionRequested = "KYC rejection requested for document ID: {DocumentId}";
            public const string KYCRejectionCompleted = "KYC rejection completed for document ID: {DocumentId}";
            public const string KYCScoreCalculationRequested = "KYC score calculation requested for customer ID: {CustomerId}";
            public const string KYCScoreCalculationCompleted = "KYC score calculated for customer ID: {CustomerId} with score: {Score}";

            // Loan Application Controller
            public const string PersonalLoanRetrievalRequested = "Personal loan retrieval requested for all loans";
            public const string PersonalLoanRetrievalCompleted = "Personal loan retrieval completed, found {Count} loans";
            public const string PersonalLoanByIdRequested = "Personal loan by ID requested for ID: {Id}";
            public const string PersonalLoanByIdCompleted = "Personal loan by ID completed for ID: {Id}";
            public const string LoansByStatusRequested = "Loans by status requested for status: {Status}";
            public const string LoansByStatusCompleted = "Loans by status completed, found {Count} loans";
            public const string PersonalLoanCreationRequested = "Personal loan creation requested for customer ID: {CustomerId}";
            public const string PersonalLoanCreationCompleted = "Personal loan creation completed with ID: {LoanApplicationBaseId}";
            public const string LoanStatusUpdateRequested = "Loan status update requested for ID: {Id} to status: {Status}";
            public const string LoanStatusUpdateCompleted = "Loan status update completed for ID: {Id}";
            public const string HomeLoanCreationRequested = "Home loan creation requested for customer ID: {CustomerId}";
            public const string HomeLoanCreationCompleted = "Home loan creation completed with ID: {LoanApplicationBaseId}";
            public const string VehicleLoanCreationRequested = "Vehicle loan creation requested for customer ID: {CustomerId}";
            public const string VehicleLoanCreationCompleted = "Vehicle loan creation completed with ID: {LoanApplicationBaseId}";

            // Manager Dashboard Controller
            public const string OverallMetricsRequested = "Overall metrics requested";
            public const string OverallMetricsCompleted = "Overall metrics completed";
            public const string ApplicationStatusSummaryRequested = "Application status summary requested";
            public const string ApplicationStatusSummaryCompleted = "Application status summary completed";
            public const string ApplicationTrendsRequested = "Application trends requested";
            public const string ApplicationTrendsCompleted = "Application trends completed";
            public const string LoanTypePerformanceRequested = "Loan type performance requested";
            public const string LoanTypePerformanceCompleted = "Loan type performance completed";
            public const string NewApplicationsSummaryRequested = "New applications summary requested";
            public const string NewApplicationsSummaryCompleted = "New applications summary completed";

            // Manager Workflow Controller
            public const string PendingApplicationsRequested = "Pending applications requested";
            public const string PendingApplicationsCompleted = "Pending applications completed";
            public const string ApplicationDetailsRequested = "Application details requested for ID: {ApplicationId}";
            public const string ApplicationDetailsCompleted = "Application details completed for ID: {ApplicationId}";
            public const string WorkflowStartRequested = "Workflow start requested for application ID: {ApplicationId}, manager ID: {ManagerId}";
            public const string WorkflowStartCompleted = "Workflow start completed for application ID: {ApplicationId}";
            public const string WorkflowStepUpdateRequested = "Workflow step update requested for application ID: {ApplicationId}";
            public const string WorkflowStepUpdateCompleted = "Workflow step update completed for application ID: {ApplicationId}";
            public const string WorkflowStatusRequested = "Workflow status requested for application ID: {ApplicationId}";
            public const string WorkflowStatusCompleted = "Workflow status completed for application ID: {ApplicationId}";
            public const string DocumentVerificationWorkflowRequested = "Document verification workflow requested for application ID: {ApplicationId}";
            public const string DocumentVerificationWorkflowCompleted = "Document verification workflow completed for application ID: {ApplicationId}";
            public const string ApplicationApprovalRequested = "Application approval requested for ID: {ApplicationId}";
            public const string ApplicationApprovalCompleted = "Application approval completed for ID: {ApplicationId}";
            public const string ApplicationRejectionRequested = "Application rejection requested for ID: {ApplicationId}";
            public const string ApplicationRejectionCompleted = "Application rejection completed for ID: {ApplicationId}";
            public const string ApplicationRevisionRequested = "Application revision requested for ID: {ApplicationId}";
            public const string ApplicationRevisionCompleted = "Application revision completed for ID: {ApplicationId}";
            public const string ApplicationDisbursementRequested = "Application disbursement requested for ID: {ApplicationId}";
            public const string ApplicationDisbursementCompleted = "Application disbursement completed for ID: {ApplicationId}";

            // Notification Controller
            public const string NotificationsRetrievalRequested = "Notifications retrieval requested for user ID: {UserId}";
            public const string NotificationsRetrievalCompleted = "Notifications retrieval completed, found {Count} notifications for user ID: {UserId}";
            public const string UnreadNotificationsRequested = "Unread notifications requested for user ID: {UserId}";
            public const string UnreadNotificationsCompleted = "Unread notifications completed, found {Count} notifications for user ID: {UserId}";
            public const string UnreadCountRequested = "Unread count requested for user ID: {UserId}";
            public const string UnreadCountCompleted = "Unread count completed for user ID: {UserId}, count: {Count}";
            public const string MarkAsReadRequested = "Mark as read requested for notification ID: {NotificationId}";
            public const string MarkAsReadCompleted = "Mark as read completed for notification ID: {NotificationId}";
            public const string MarkAllAsReadRequested = "Mark all as read requested for user ID: {UserId}";
            public const string MarkAllAsReadCompleted = "Mark all as read completed for user ID: {UserId}";
            public const string PreferencesRetrievalRequested = "Preferences retrieval requested for user ID: {UserId}";
            public const string PreferencesRetrievalCompleted = "Preferences retrieval completed for user ID: {UserId}";
            public const string PreferencesUpdateRequested = "Preferences update requested for user ID: {UserId}, type: {NotificationType}";
            public const string PreferencesUpdateCompleted = "Preferences update completed for user ID: {UserId}, type: {NotificationType}";
            public const string TestNotificationRequested = "Test notification requested for user ID: {UserId}";
            public const string TestNotificationCompleted = "Test notification completed for user ID: {UserId}";
            public const string BulkNotificationRequested = "Bulk notification requested for {Count} users";
            public const string BulkNotificationCompleted = "Bulk notification completed for {Count} users";

            // Razorpay Controller
            public const string RazorpayOrderCreationRequested = "Razorpay order creation requested for amount: {Amount}";
            public const string RazorpayOrderCreationCompleted = "Razorpay order creation completed with ID: {Id}";
            public const string RazorpayPaymentRequested = "Razorpay payment requested for payment ID: {RazorpayPaymentId}";
            public const string RazorpayPaymentCompleted = "Razorpay payment completed for payment ID: {RazorpayPaymentId}";
            public const string RazorpaySignatureVerificationRequested = "Razorpay signature verification requested for payment ID: {RazorpayPaymentId}";
            public const string RazorpaySignatureVerificationCompleted = "Razorpay signature verification completed for payment ID: {RazorpayPaymentId}, result: {IsValid}";
            public const string RazorpayDisbursementRequested = "Razorpay disbursement requested for amount: {Amount}";
            public const string RazorpayDisbursementCompleted = "Razorpay disbursement completed with ID: {Id}";
            public const string RazorpayDisbursementStatusRequested = "Razorpay disbursement status requested for ID: {DisbursementId}";
            public const string RazorpayDisbursementStatusCompleted = "Razorpay disbursement status completed for ID: {DisbursementId}";

            // Token Controller
            public const string LoginRequested = "Login requested for user: {Username}";
            public const string LoginCompleted = "Login completed for user: {Username}";
            public const string LoginFailed = "Login failed for user: {Username}";
            public const string RegistrationRequested = "Registration requested for email: {Email}";
            public const string RegistrationCompleted = "Registration completed for email: {Email}";
            public const string ForgotPasswordRequested = "Forgot password requested for email: {Email}";
            public const string ForgotPasswordCompleted = "Forgot password completed for email: {Email}";
            public const string ForgotPasswordEmailNotFound = "Forgot password email not found: {Email}";
            public const string ResetPasswordRequested = "Reset password requested for email: {Email}";
            public const string ResetPasswordCompleted = "Reset password completed for email: {Email}";
            public const string ResetPasswordTokenInvalid = "Reset password token invalid for email: {Email}";

            // User Controller
            public const string UsersRetrievalRequested = "Users retrieval requested";
            public const string UsersRetrievalCompleted = "Users retrieval completed, found {Count} users";
            public const string UserRetrievalRequested = "User retrieval requested for ID: {Id}";
            public const string UserRetrievalCompleted = "User retrieval completed for ID: {Id}";
            public const string UserNotFoundById = "User not found for ID: {Id}";
            public const string UserCreationRequested = "User creation requested for email: {Email}";
            public const string UserCreationCompleted = "User creation completed with ID: {UserId}";
            public const string PasswordChangeRequested = "Password change requested for user ID: {UserId}";
            public const string PasswordChangeCompleted = "Password change completed for user ID: {UserId}";
            public const string PasswordChangeIncorrectCurrent = "Password change failed - incorrect current password for user ID: {UserId}";
        }

        public static class ErrorMessages
        {
            public const string UserIdsRequired = "User IDs are required";
            public const string LoanApplicationNotFound = "Loan application not found";
            public const string FailedToUpdateApplicationStatus = "Failed to update application status";
            public const string FailedToDisburseLoan = "Failed to disburse loan. Loan must be approved first.";
        }

        public static class SuccessMessages
        {
            public const string AllNotificationsMarkedAsRead = "All notifications marked as read";
            public const string BulkNotificationSent = "Bulk notification sent to {Count} users";
            public const string TestNotificationSent = "Test notification sent";
            public const string PaymentProcessedSuccessfully = "Payment processed successfully";
            public const string PreferencesUpdatedSuccessfully = "Preferences updated successfully";
            public const string ApplicationStatusUpdatedSuccessfully = "Application status updated successfully";
            public const string LoanDisbursedSuccessfully = "Loan disbursed successfully";
        }

        public static class Roles
        {
            public const string Manager = "Manager";
        }
    }
}