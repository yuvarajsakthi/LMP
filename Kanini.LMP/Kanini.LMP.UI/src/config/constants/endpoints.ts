const validateEndpoint = (endpoint: string): string => {
  if (!endpoint || typeof endpoint !== 'string' || !endpoint.startsWith('/')) {
    throw new Error('Invalid API endpoint format');
  }
  return endpoint;
};

export const API_ENDPOINTS = {
  // Token Controller endpoints
  USER_LOGIN: validateEndpoint('/api/Token/login'),
  USER_REGISTER: validateEndpoint('/api/Token/register'),
  SEND_OTP: validateEndpoint('/api/Token/send-otp'),
  VERIFY_OTP: validateEndpoint('/api/Token/verify-otp'),
  VERIFY_LOGIN_OTP: validateEndpoint('/api/Token/verify-login-otp'),
  RESET_PASSWORD: validateEndpoint('/api/Token/reset-password'),
  
  // Customer Controller endpoints
  GET_CUSTOMERS: validateEndpoint('/api/Customer'),
  GET_CUSTOMER_BY_ID: validateEndpoint('/api/Customer'),
  UPDATE_CUSTOMER: validateEndpoint('/api/Customer'),
  GET_CUSTOMER_SETTINGS: validateEndpoint('/api/Customer/settings'),
  UPDATE_CUSTOMER_SETTINGS: validateEndpoint('/api/Customer/settings'),
  
  // Eligibility Controller endpoints
  CHECK_ELIGIBILITY: validateEndpoint('/api/Eligibility/check'),
  
  // Loan Application Controller endpoints
  GET_PERSONAL_LOANS: validateEndpoint('/api/LoanApplication/personal'),
  GET_PERSONAL_LOAN_BY_ID: validateEndpoint('/api/LoanApplication/personal'),
  GET_LOANS_BY_STATUS: validateEndpoint('/api/LoanApplication/personal/status'),
  SUBMIT_PERSONAL_LOAN: validateEndpoint('/api/LoanApplication/personal/submit'),
  UPDATE_LOAN_STATUS: validateEndpoint('/api/LoanApplication/personal/status'),
  CREATE_HOME_LOAN: validateEndpoint('/api/LoanApplication/home'),
  CREATE_VEHICLE_LOAN: validateEndpoint('/api/LoanApplication/vehicle'),
  GET_CUSTOMER_APPLICATIONS: validateEndpoint('/api/LoanApplication/customer-applications'),
  GET_LOAN_PRODUCTS: validateEndpoint('/api/LoanProduct/active'),

  
  // EMI Calculator Controller endpoints
  CALCULATE_EMI: validateEndpoint('/api/EmiCalculator/calculate'),
  CREATE_EMI_PLAN: validateEndpoint('/api/EmiCalculator/create'),
  GET_EMI_PLAN: validateEndpoint('/api/EmiCalculator'),
  GET_EMI_DASHBOARD: validateEndpoint('/api/EmiCalculator/dashboard'),
  GET_EMI_SCHEDULE: validateEndpoint('/api/EmiCalculator/schedule'),
  
  // Document Controller endpoints
  UPLOAD_DOCUMENT: validateEndpoint('/api/Document/upload'),
  DOWNLOAD_DOCUMENT: validateEndpoint('/api/Document/download'),
  VIEW_DOCUMENT: validateEndpoint('/api/Document/view'),
  GET_DOCUMENTS_BY_APPLICATION: validateEndpoint('/api/Document/application'),
  VERIFY_DOCUMENT: validateEndpoint('/api/Document/verify'),
  DELETE_DOCUMENT: validateEndpoint('/api/Document/delete'),
  GET_PENDING_DOCUMENTS: validateEndpoint('/api/Document/pending'),
  
  // Payment Controller endpoints
  GET_PAYMENTS_BY_LOAN: validateEndpoint('/api/Payment/loan-account'),
  GET_EMI_PLANS_BY_LOAN: validateEndpoint('/api/Payment/emi-plans'),
  CREATE_PAYMENT: validateEndpoint('/api/Payment'),
  UPDATE_PAYMENT_STATUS: validateEndpoint('/api/Payment/status'),
  GET_PAYMENT_ANALYTICS: validateEndpoint('/api/Payment/analytics-sp'),
  GET_PAYMENT_HISTORY: validateEndpoint('/api/Payment/history-sp'),
  
  // Notification Controller endpoints
  GET_MY_NOTIFICATIONS: validateEndpoint('/api/Notification/my-notifications'),
  GET_UNREAD_NOTIFICATIONS: validateEndpoint('/api/Notification/unread'),
  GET_UNREAD_COUNT: validateEndpoint('/api/Notification/unread-count'),
  MARK_NOTIFICATION_READ: validateEndpoint('/api/Notification/mark-read'),
  MARK_ALL_NOTIFICATIONS_READ: validateEndpoint('/api/Notification/mark-all-read'),
  
  // Manager Dashboard Controller endpoints
  GET_OVERALL_METRICS: validateEndpoint('/api/ManagerDashboard/overall-metrics'),
  GET_APPLICATION_STATUS_SUMMARY: validateEndpoint('/api/ManagerDashboard/application-status'),
  GET_APPLICATION_TRENDS: validateEndpoint('/api/ManagerDashboard/application-trends'),
  GET_LOAN_TYPE_PERFORMANCE: validateEndpoint('/api/ManagerDashboard/loan-type-performance'),
  GET_NEW_APPLICATIONS_SUMMARY: validateEndpoint('/api/ManagerDashboard/new-applications'),
  
  // Razorpay Controller endpoints
  CREATE_RAZORPAY_ORDER: validateEndpoint('/api/Razorpay/create-order'),
  PROCESS_RAZORPAY_PAYMENT: validateEndpoint('/api/Razorpay/process-payment'),
  VERIFY_RAZORPAY_SIGNATURE: validateEndpoint('/api/Razorpay/verify-signature'),
  DISBURSE_LOAN: validateEndpoint('/api/Razorpay/disburse-loan'),
  GET_DISBURSEMENT_STATUS: validateEndpoint('/api/Razorpay/disbursement-status'),
} as const;