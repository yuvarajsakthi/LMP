const validateEndpoint = (endpoint: string): string => {
  if (!endpoint || typeof endpoint !== 'string' || !endpoint.startsWith('/')) {
    throw new Error('Invalid API endpoint format');
  }
  return endpoint;
};

export const API_ENDPOINTS = {
  // Token Controller endpoints
  USER_LOGIN: validateEndpoint('/auth/login'),
  USER_REGISTER: validateEndpoint('/auth/register'),
  SEND_OTP: validateEndpoint('/auth/sendotp'),
  VERIFY_OTP: validateEndpoint('/auth/verify/otp'),
  VERIFY_LOGIN_OTP: validateEndpoint('/auth/login/otp'),
  FORGOT_PASSWORD: validateEndpoint('/auth/forgot-password'),
  RESET_PASSWORD: validateEndpoint('/auth/reset-password'),
  
  // Customer Controller endpoints
  GET_CUSTOMERS: validateEndpoint('/api/Customer'),
  GET_CUSTOMER_BY_ID: validateEndpoint('/api/Customer'),
  UPDATE_CUSTOMER: validateEndpoint('/api/Customer'),
  GET_CUSTOMER_SETTINGS: validateEndpoint('/api/Customer/settings'),
  UPDATE_CUSTOMER_SETTINGS: validateEndpoint('/api/Customer/settings'),
  
  // Eligibility Controller endpoints
  GET_ELIGIBILITY_SCORE: validateEndpoint('/api/Eligibility'),
  CALCULATE_ELIGIBILITY: validateEndpoint('/api/Eligibility/calculate'),
  
  // FAQ Controller endpoints
  CREATE_FAQ: validateEndpoint('/api/Faq'),
  GET_ALL_FAQS: validateEndpoint('/api/Faq'),
  GET_FAQ_BY_ID: validateEndpoint('/api/Faq'),
  GET_FAQS_BY_CUSTOMER: validateEndpoint('/api/Faq/customer'),
  UPDATE_FAQ: validateEndpoint('/api/Faq'),
  DELETE_FAQ: validateEndpoint('/api/Faq'),
  
  // Loan Application Flow Controller endpoints
  CREATE_PERSONAL_LOAN: validateEndpoint('/api/LoanApplicationFlow/personal'),
  CREATE_HOME_LOAN: validateEndpoint('/api/LoanApplicationFlow/home'),
  CREATE_VEHICLE_LOAN: validateEndpoint('/api/LoanApplicationFlow/vehicle'),
  UPDATE_LOAN_STATUS: validateEndpoint('/api/LoanApplicationFlow'),
  WITHDRAW_LOAN: validateEndpoint('/api/LoanApplicationFlow'),

  // EMI Calculator Controller endpoints
  CALCULATE_EMI: validateEndpoint('/api/EmiCalculator/calculate'),
  CREATE_EMI_PLAN: validateEndpoint('/api/EmiCalculator/create'),
  GET_EMI_PLAN: validateEndpoint('/api/EmiCalculator'),
  GET_EMI_DASHBOARD: validateEndpoint('/api/EmiCalculator/dashboard'),
  GET_EMI_SCHEDULE: validateEndpoint('/api/EmiCalculator/schedule'),
  
  // Notification Controller endpoints
  GET_ALL_NOTIFICATIONS: validateEndpoint('/api/Notification'),
  DELETE_NOTIFICATION: validateEndpoint('/api/Notification'),
  
  // Customer Dashboard Controller endpoints
  CUSTOMER_DASHBOARD_LOAN_PRODUCTS: validateEndpoint('/customerdashboard/loanproducts'),
  CUSTOMER_DASHBOARD_RECENT_LOANS: validateEndpoint('/customerdashboard/recent-applied-loans'),
  CUSTOMER_DASHBOARD_APPLICATION_STATUS: validateEndpoint('/customerdashboard/applicationstatus'),
  GET_LOAN_CATEGORIES: validateEndpoint('/customerdashboard/loanproducts'),
  
} as const;