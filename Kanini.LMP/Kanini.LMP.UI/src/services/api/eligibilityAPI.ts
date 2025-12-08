import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface EligibilityProfileRequest {
  isExistingBorrower: boolean;
  pan?: string;
  age?: number;
  annualIncome?: number;
  occupation?: string;
  homeOwnershipStatus?: number;
}

export interface EligibilityScore {
  customerId: number;
  creditScore: number;
  eligibilityScore: number;
  eligibilityStatus: string;
  calculatedOn: string;
}

export const eligibilityAPI = {
  async getEligibilityScore(customerId: number): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any>>(
        `${API_ENDPOINTS.GET_ELIGIBILITY_SCORE}/${customerId}`
      );
      return response;
    });
  },

  async calculateEligibility(customerId: number): Promise<EligibilityScore> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<EligibilityScore>>(
        `${API_ENDPOINTS.CALCULATE_ELIGIBILITY}?customerId=${customerId}`
      );
      return response;
    });
  },

  async updateEligibilityProfile(customerId: number, request: EligibilityProfileRequest): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<any>>(
        `${API_ENDPOINTS.UPDATE_ELIGIBILITY_PROFILE}?customerId=${customerId}`,
        request
      );
      return response;
    });
  },

  async checkEligibility(request: EligibilityProfileRequest): Promise<any> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('isExistingBorrower', String(request.isExistingBorrower));
      if (request.pan) formData.append('pan', request.pan);
      if (request.age) formData.append('age', String(request.age));
      if (request.annualIncome) formData.append('annualIncome', String(request.annualIncome));
      if (request.occupation) formData.append('occupation', request.occupation);
      if (request.homeOwnershipStatus) formData.append('homeOwnershipStatus', String(request.homeOwnershipStatus));

      const response = await axiosInstance.post<ApiResponse<any>>(
        API_ENDPOINTS.CHECK_ELIGIBILITY,
        formData
      );
      return response;
    });
  }
};
