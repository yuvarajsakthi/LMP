import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface PaymentTransaction {
  paymentTransactionId: number;
  loanAccountId: number;
  amount: number;
  paymentDate: string;
  paymentMethod: string;
  transactionId: string;
  status: string;
  description?: string;
}

export interface RazorpayOrder {
  id: string;
  amount: number;
  currency: string;
  status: string;
}

export const paymentAPI = {
  async getPaymentsByLoan(loanAccountId: number): Promise<PaymentTransaction[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<PaymentTransaction[]>>(
        `${API_ENDPOINTS.GET_PAYMENTS_BY_LOAN}/${loanAccountId}`
      );
      return response;
    });
  },

  async getEMIPlansByLoan(loanAccountId: number): Promise<any[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any[]>>(
        `${API_ENDPOINTS.GET_EMI_PLANS_BY_LOAN}/${loanAccountId}`
      );
      return response;
    });
  },

  async createPayment(payment: Partial<PaymentTransaction>): Promise<PaymentTransaction> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<PaymentTransaction>>(
        API_ENDPOINTS.CREATE_PAYMENT,
        payment
      );
      return response;
    });
  },

  async updatePaymentStatus(paymentId: number, status: string): Promise<PaymentTransaction> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<PaymentTransaction>>(
        `${API_ENDPOINTS.UPDATE_PAYMENT_STATUS}/${paymentId}`,
        status
      );
      return response;
    });
  },

  async getPaymentAnalytics(fromDate?: string, toDate?: string): Promise<any> {
    return ApiService.execute(async () => {
      const params = new URLSearchParams();
      if (fromDate) params.append('fromDate', fromDate);
      if (toDate) params.append('toDate', toDate);
      
      const response = await axiosInstance.get<ApiResponse<any>>(
        `${API_ENDPOINTS.GET_PAYMENT_ANALYTICS}?${params}`
      );
      return response;
    });
  },

  async getPaymentHistory(loanAccountId: number): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<any>>(
        `${API_ENDPOINTS.GET_PAYMENT_HISTORY}/${loanAccountId}`
      );
      return response;
    });
  },

  // Razorpay endpoints
  async createRazorpayOrder(amount: number, currency: string = 'INR'): Promise<RazorpayOrder> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<RazorpayOrder>>(
        API_ENDPOINTS.CREATE_RAZORPAY_ORDER,
        { amount, currency }
      );
      return response;
    });
  },

  async processRazorpayPayment(paymentData: any): Promise<PaymentTransaction> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<PaymentTransaction>>(
        API_ENDPOINTS.PROCESS_RAZORPAY_PAYMENT,
        paymentData
      );
      return response;
    });
  },

  async verifyRazorpaySignature(paymentData: any): Promise<{ isValid: boolean }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{ isValid: boolean }>>(
        API_ENDPOINTS.VERIFY_RAZORPAY_SIGNATURE,
        paymentData
      );
      return response;
    });
  }
};