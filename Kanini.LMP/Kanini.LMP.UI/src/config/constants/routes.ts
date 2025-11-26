export const ROUTES = {
  LOGIN: import.meta.env.VITE_LOGIN_ROUTE || '/login',
  REGISTER: import.meta.env.VITE_REGISTER_ROUTE || '/register',
  FORGOT_PASSWORD: import.meta.env.VITE_FORGOT_PASSWORD_ROUTE || '/forgotpassword',
  UNAUTHORIZED: import.meta.env.VITE_UNAUTHORIZED_ROUTE || '/unauthorized',
  CUSTOMER_DASHBOARD: import.meta.env.VITE_CUSTOMER_DASHBOARD_ROUTE || '/customer-dashboard',
  APPLIED_LOAN: import.meta.env.VITE_APPLIED_LOAN_ROUTE || '/applied-loan',
} as const;