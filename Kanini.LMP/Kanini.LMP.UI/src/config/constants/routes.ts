export const COMMON_ROUTES = {
  LOGIN: '/login',
  REGISTER: '/register',
  FORGOT_PASSWORD: '/forgotpassword',
  UNAUTHORIZED: '/unauthorized',
} as const;

export const CUSTOMER_ROUTES = {
  CUSTOMER_DASHBOARD: '/customer-dashboard',
  VIEWSTATUS: '/view-status',
  EMI_CALCULATOR: '/emi-calculator',
  LOAN_TYPES: '/loan-types',
  LOAN_APPLICATION: '/loan-application',
  FAQ: '/faq',
  SETTINGS: '/settings',
} as const;

export const MANAGER_ROUTES = {
  MANAGER_DASHBOARD: '/manager-dashboard',
  APPLIED_LOAN: '/applied-loan',
  MANAGER_APPLICATIONS: '/manager/applications',
  MANAGER_FAQ: '/manager/faq',  
} as const;