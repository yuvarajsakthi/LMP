import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface LoanProduct {
  loanProductId: number;
  loanType: string;
  interestRate: number;
  minAmount: number;
  maxAmount: number;
  minTenure: number;
  maxTenure: number;
}

export interface RecentAppliedLoan {
  loanId: number;
  loanName: string;
  amountToBePaid: number;
  emi: number;
  yearsRemaining: number;
  status: string;
  appliedDate: string;
}

export interface ApplicationStatus {
  applicationId: number;
  loanType: string;
  amount: number;
  status: string;
  appliedDate: string;
  lastUpdated: string;
}

export const customerDashboardAPI = {
  async getLoanProducts(): Promise<LoanProduct[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<LoanProduct[]>>(
        API_ENDPOINTS.CUSTOMER_DASHBOARD_LOAN_PRODUCTS
      );
      return response;
    });
  },

  async getRecentAppliedLoans(): Promise<RecentAppliedLoan[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<RecentAppliedLoan[]>>(
        API_ENDPOINTS.CUSTOMER_DASHBOARD_RECENT_LOANS
      );
      return response;
    });
  },

  async getApplicationStatus(): Promise<ApplicationStatus[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<ApplicationStatus[]>>(
        API_ENDPOINTS.CUSTOMER_DASHBOARD_APPLICATION_STATUS
      );
      return response;
    });
  },

  async getEligibilityScore(customerId: number): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any>>(
        `${API_ENDPOINTS.GET_ELIGIBILITY_SCORE}?customerId=${customerId}`
      );
      return response;
    });
  }
};
