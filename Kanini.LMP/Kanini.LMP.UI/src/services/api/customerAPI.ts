import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface Customer {
  customerId: number;
  userId: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: string;
  panNumber: string;
  aadharNumber: string;
  annualIncome: number;
  employmentType: string;
  creditScore: number;
  createdAt: string;
  updatedAt: string;
}

export const customerAPI = {
  async getCustomers(): Promise<Customer[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer[]>>(
        API_ENDPOINTS.GET_CUSTOMERS
      );
      return response;
    });
  },

  async getCustomerById(id: number): Promise<Customer> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer>>(
        `${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/${id}`
      );
      return response;
    });
  },

  async updateCustomer(id: number, customer: Partial<Customer>): Promise<Customer> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<Customer>>(
        `${API_ENDPOINTS.UPDATE_CUSTOMER}/${id}`,
        customer
      );
      return response;
    });
  }
};