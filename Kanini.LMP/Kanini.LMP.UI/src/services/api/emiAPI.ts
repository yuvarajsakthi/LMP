import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface CalculateEmiRequest {
  principalAmount: number;
  interestRate: number;
  termMonths: number;
}

export interface EMIPlan {
  emiId: number;
  loanApplicationId: number;
  principalAmount: number;
  interestRate: number;
  termMonths: number;
  monthlyEMI: number;
  totalAmount: number;
  totalInterest: number;
  startDate: string;
  endDate: string;
}

export interface EMIDashboard {
  customerId: number;
  totalActiveLoans: number;
  totalMonthlyEMI: number;
  nextDueDate: string;
  nextDueAmount: number;
  totalOutstanding: number;
  upcomingEMIs: Array<{
    emiId: number;
    dueDate: string;
    amount: number;
    loanType: string;
  }>;
}

export const emiAPI = {
  async calculateEMI(request: CalculateEmiRequest): Promise<EMIPlan> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<EMIPlan>>(
        API_ENDPOINTS.CALCULATE_EMI,
        request
      );
      return response;
    });
  },

  async createEMIPlan(plan: Partial<EMIPlan>): Promise<EMIPlan> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<EMIPlan>>(
        API_ENDPOINTS.CREATE_EMI_PLAN,
        plan
      );
      return response;
    });
  },

  async getEMIPlan(emiId: number): Promise<EMIPlan> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<EMIPlan>>(
        `${API_ENDPOINTS.GET_EMI_PLAN}/${emiId}`
      );
      return response;
    });
  },

  async getEMIDashboard(): Promise<EMIDashboard> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<EMIDashboard>>(
        API_ENDPOINTS.GET_EMI_DASHBOARD
      );
      return response;
    });
  },

  async getEMISchedule(emiId: number): Promise<any[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>(
        `${API_ENDPOINTS.GET_EMI_SCHEDULE}/${emiId}`
      );
      return response;
    });
  }
};