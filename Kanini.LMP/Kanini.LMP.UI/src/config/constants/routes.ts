export const COMMON_ROUTES = {
  LOGIN: '/login',
  REGISTER: '/register',
  FORGOT_PASSWORD: '/forgotpassword',
  UNAUTHORIZED: '/unauthorized',
  APPLIED_LOAN: '/applied-loan',
  LOAN_APPLICATION_FORM: '/loan-application-form',
  MANAGER_APPLICATIONS: '/manager/applications',
  MANAGER_APPROVALS: '/manager/approvals',
} as const;

export const CUSTOMER_ROUTES = {
CUSTOMER_DASHBOARD: '/customer-dashboard',
  VIEWSTATUS: '/view-status',
  EMI_CALCULATOR: '/emi-calculator',
  LOAN_TYPES: '/loan-types',
  INTEGRATION: '/integration',
  FAQ: '/faq',
  SETTINGS: '/settings',
  
} as const;

export const MANAGER_ROUTES = {
MANAGER_DASHBOARD: '/manager-dashboard',
  
} as const;