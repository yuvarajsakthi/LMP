import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface EligibilityRequest {
  isExistingBorrower: boolean;
  pan?: string;
  age?: number;
  annualIncome?: number;
  occupation?: string;
  homeOwnershipStatus?: number;
  experienceYears?: number;
  employerName?: string;
  monthlyEMI?: number;
  existingLoanAmount?: number;
  previousLoanCount?: number;
  onTimePayments?: number;
  latePayments?: number;
  missedPayments?: number;
  hasDefaultHistory?: boolean;
  daysOverdueMax?: number;
}

export interface EligibilityResponse {
  customerId: number;
  creditScore: { score: number; range: string };
  eligibilityScore: number;
  status: string;
  eligibleProductCount: number;
  products: Array<{
    productId: number;
    productName: string;
    available: boolean;
    minScore: number;
    maxAmount: string;
    interestRate: string;
  }>;
  message: string;
  improvementTips: string[];
  lastUpdated: string;
  nextSteps: string;
}

export interface PersonalLoanApplication {
  loanApplicationBaseId?: number;
  customerId: number;
  loanAmount: number;
  loanPurpose: string;
  employmentType: string;
  monthlyIncome: number;
  existingEMI: number;
  status?: string;
}

export const loanAPI = {
  async checkEligibility(request: EligibilityRequest): Promise<EligibilityResponse> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<EligibilityResponse>>(
        API_ENDPOINTS.CHECK_ELIGIBILITY,
        request
      );
      return response;
    });
  },

  async getPersonalLoans(): Promise<PersonalLoanApplication[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<PersonalLoanApplication[]>>(
        API_ENDPOINTS.GET_PERSONAL_LOANS
      );
      return response;
    });
  },

  async getPersonalLoanById(id: number): Promise<PersonalLoanApplication> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<PersonalLoanApplication>>(
        `${API_ENDPOINTS.GET_PERSONAL_LOAN_BY_ID}/${id}`
      );
      return response;
    });
  },

  async submitPersonalLoan(customerId: number, application: any): Promise<{ applicationId: number; status: string; message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{ applicationId: number; status: string; message: string }>>(
        `${API_ENDPOINTS.SUBMIT_PERSONAL_LOAN}?customerId=${customerId}`,
        application
      );
      return response;
    });
  },

  async updateLoanStatus(id: number, status: string): Promise<PersonalLoanApplication> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<PersonalLoanApplication>>(
        `${API_ENDPOINTS.UPDATE_LOAN_STATUS}/${id}`,
        status
      );
      return response;
    });
  },

  async getCustomerApplications(): Promise<PersonalLoanApplication[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<PersonalLoanApplication[]>>(
        API_ENDPOINTS.GET_CUSTOMER_APPLICATIONS
      );
      return response;
    });
  },

  async getLoanCategories(): Promise<any[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>(
        API_ENDPOINTS.GET_LOAN_PRODUCTS
      );
      return response;
    });
  }
};