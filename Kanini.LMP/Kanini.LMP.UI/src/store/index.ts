import { configureStore } from '@reduxjs/toolkit';
import authReducer from './slices/authSlice';
import dashboardReducer from './slices/dashboardSlice';
import customerReducer from './slices/customerSlice';
import eligibilityReducer from './slices/eligibilitySlice';
import faqReducer from './slices/faqSlice';
import loanApplicationReducer from './slices/loanApplicationSlice';
import notificationReducer from './slices/notificationSlice';
import managerReducer from './slices/managerSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    dashboard: dashboardReducer,
    customer: customerReducer,
    eligibility: eligibilityReducer,
    faq: faqReducer,
    loanApplication: loanApplicationReducer,
    notification: notificationReducer,
    manager: managerReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export * from './slices/authSlice';
export * from './slices/dashboardSlice';
export * from './slices/customerSlice';
export * from './slices/eligibilitySlice';
export * from './slices/faqSlice';
export * from './slices/loanApplicationSlice';
export { updateLoanStatus as updateManagerLoanStatus } from './slices/managerSlice';
export * from './slices/notificationSlice';
export * from './slices/managerSlice';
export { fetchCustomerById } from './slices/customerSlice';