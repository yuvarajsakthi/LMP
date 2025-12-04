import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface Customer {
  customerId: number;
  userId: number;
  phoneNumber: string;
  dateOfBirth: string;
  occupation: string;
  annualIncome: number;
  creditScore: number;
  gender?: number;
  homeOwnershipStatus?: number;
  age?: number;
  profileImageBase64?: string;
  applicationIds?: number[];
  updatedAt?: string;
}

export interface CustomerSettings {
  fullName: string;
  email: string;
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
}

export interface UpdateCustomerSettings {
  fullName: string;
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
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

  async getCustomerByUserId(userId: number): Promise<Customer> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer>>(
        `${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/user/${userId}`
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
  },

  async getCustomerSettings(userId: number): Promise<CustomerSettings> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<CustomerSettings>>(
        `${API_ENDPOINTS.GET_CUSTOMER_SETTINGS}/${userId}`
      );
      return response;
    });
  },

  async updateCustomerSettings(userId: number, settings: UpdateCustomerSettings): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<any>>(
        `${API_ENDPOINTS.UPDATE_CUSTOMER_SETTINGS}/${userId}`,
        settings
      );
      return response;
    });
  }
};