import axiosInstance from './axiosInstance';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export const customerDashboardAPI = {
  async getLoanProducts() {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>('/customerdashboard/loanproducts');
      return response;
    });
  },

  async getRecentAppliedLoans() {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>('/customerdashboard/recent-applied-loans');
      return response;
    });
  },

  async getEligibilityScore() {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any>>('/customerdashboard/eligibilityscore');
      return response;
    });
  },

  async updateEligibilityScore(data: FormData) {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<any>>('/customerdashboard/eligibilityscore', data);
      return response;
    });
  },

  async getApplicationStatus() {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>('/customerdashboard/applicationstatus');
      return response;
    });
  }
};
