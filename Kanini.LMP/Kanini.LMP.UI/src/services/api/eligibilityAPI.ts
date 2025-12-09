import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface EligibilityResponse {
  customerId?: number;
  creditScore?: {
    score: number;
    range: string;
  };
  eligibilityScore?: number;
  status?: string;
  eligibleProductCount?: number;
  products?: Array<{
    productId: number;
    productName: string;
    available: boolean;
    minScore: number;
    maxAmount: string;
    interestRate: string;
  }>;
  message?: string;
  improvementTips?: string[];
  lastUpdated?: string;
  nextSteps?: string;
}

export const eligibilityAPI = {
  async getEligibilityScore(customerId: number): Promise<any> {
    try {
      const response = await axiosInstance.get(
        `${API_ENDPOINTS.GET_ELIGIBILITY_SCORE}/${customerId}`
      );
      return response.data;
    } catch (error: any) {
      throw error;
    }
  },

  async calculateEligibility(customerId: number): Promise<EligibilityResponse> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<EligibilityResponse>>(
        `${API_ENDPOINTS.CALCULATE_ELIGIBILITY}/${customerId}`
      );
      return response;
    });
  }
};
