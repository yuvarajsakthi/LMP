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
      const response = await axiosInstance.get<ApiResponse<Customer[]>>(API_ENDPOINTS.GET_CUSTOMERS);
      return response;
    });
  },

  async getCustomerById(id: number): Promise<Customer> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer>>(`${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/${id}`);
      return response;
    });
  },

  async getCustomerByUserId(userId: number): Promise<Customer> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer>>(`${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/user/${userId}`);
      return response;
    });
  },

  async getCustomerSettings(userId: number): Promise<CustomerSettings> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<CustomerSettings>>(`${API_ENDPOINTS.GET_CUSTOMER_SETTINGS}/${userId}`);
      return response;
    });
  },

  async updateCustomer(id: number, data: Partial<Customer>, profileImage?: File): Promise<Customer> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('CustomerId', id.toString());
      if (data.phoneNumber) formData.append('PhoneNumber', data.phoneNumber);
      if (data.occupation) formData.append('Occupation', data.occupation);
      if (data.annualIncome) formData.append('AnnualIncome', data.annualIncome.toString());
      if (profileImage) formData.append('profileImage', profileImage);
      
      const response = await axiosInstance.put<ApiResponse<Customer>>(`${API_ENDPOINTS.UPDATE_CUSTOMER}/${id}`, formData);
      return response;
    });
  },

  async updateCustomerSettings(userId: number, settings: UpdateCustomerSettings, profileImage?: File): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('FullName', settings.fullName);
      formData.append('PhoneNumber', settings.phoneNumber);
      formData.append('Occupation', settings.occupation);
      formData.append('AnnualIncome', settings.annualIncome.toString());
      if (profileImage) formData.append('profileImage', profileImage);
      
      const response = await axiosInstance.put<ApiResponse<{ message: string }>>(`${API_ENDPOINTS.UPDATE_CUSTOMER_SETTINGS}/${userId}`, formData);
      return response;
    });
  }
};
