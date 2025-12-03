import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface OverallMetrics {
  totalApplications: number;
  approvedApplications: number;
  pendingApplications: number;
  rejectedApplications: number;
  totalLoanAmount: number;
  averageProcessingTime: number;
}

export interface ApplicationStatusSummary {
  status: string;
  count: number;
  percentage: number;
}

export interface ApplicationTrend {
  date: string;
  applications: number;
  approvals: number;
}

export interface LoanTypePerformance {
  loanType: string;
  applications: number;
  approvals: number;
  approvalRate: number;
  averageAmount: number;
}

export const managerAPI = {
  async getOverallMetrics(): Promise<OverallMetrics> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<OverallMetrics>>(
        API_ENDPOINTS.GET_OVERALL_METRICS
      );
      return response;
    });
  },

  async getApplicationStatusSummary(): Promise<ApplicationStatusSummary[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<ApplicationStatusSummary[]>>(
        API_ENDPOINTS.GET_APPLICATION_STATUS_SUMMARY
      );
      return response;
    });
  },

  async getApplicationTrends(): Promise<ApplicationTrend[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<ApplicationTrend[]>>(
        API_ENDPOINTS.GET_APPLICATION_TRENDS
      );
      return response;
    });
  },

  async getLoanTypePerformance(): Promise<LoanTypePerformance[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<LoanTypePerformance[]>>(
        API_ENDPOINTS.GET_LOAN_TYPE_PERFORMANCE
      );
      return response;
    });
  },

  async getNewApplicationsSummary(): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any>>(
        API_ENDPOINTS.GET_NEW_APPLICATIONS_SUMMARY
      );
      return response;
    });
  }
};