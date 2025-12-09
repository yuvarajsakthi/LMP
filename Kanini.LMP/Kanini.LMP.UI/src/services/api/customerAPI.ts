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
  gender?: number | string;
  homeOwnershipStatus?: number | string;
  age?: number;
  profileImageBase64?: string;
  profileImage?: string;
  profileImageUrl?: string;
  applicationIds?: number[];
  updatedAt?: string;
  aadhaarNumber?: string;
  panNumber?: string;
}

export interface CustomerSettings {
  fullName: string;
  email: string;
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
  homeOwnershipStatus?: string;
  aadhaarNumber?: string;
  panNumber?: string;
}

export interface UpdateCustomerSettings {
  phoneNumber: string;
  occupation: string;
  annualIncome: number;
  homeOwnershipStatus?: string;
  aadhaarNumber?: string;
  panNumber?: string;
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
    console.log('🌐 API Call - getCustomerByUserId for userId:', userId);
    console.log('🔗 Endpoint:', `${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/user/${userId}`);
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Customer>>(`${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/user/${userId}`);
      console.log('✅ API Response:', response);
      return response;
    });
  },

  async getCustomerSettings(userId: number): Promise<CustomerSettings> {
    const settingsResponse = await axiosInstance.get<ApiResponse<any>>(`${API_ENDPOINTS.GET_CUSTOMER_SETTINGS}/${userId}`);
    const customerResponse = await axiosInstance.get<ApiResponse<Customer>>(`${API_ENDPOINTS.GET_CUSTOMER_BY_ID}/user/${userId}`);
    
    const settings = settingsResponse.data.data || settingsResponse.data;
    const customerData = customerResponse.data.data as Customer;
    
    return {
      fullName: settings.fullName,
      email: settings.email,
      phoneNumber: customerData.phoneNumber,
      occupation: customerData.occupation,
      annualIncome: customerData.annualIncome,
      homeOwnershipStatus: typeof customerData.homeOwnershipStatus === 'string' ? customerData.homeOwnershipStatus : (customerData.homeOwnershipStatus !== undefined ? ['Rented', 'Owned', 'Mortage'][customerData.homeOwnershipStatus] : undefined),
      aadhaarNumber: customerData.aadhaarNumber,
      panNumber: customerData.panNumber
    };
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
      formData.append('PhoneNumber', settings.phoneNumber);
      formData.append('Occupation', settings.occupation);
      formData.append('AnnualIncome', settings.annualIncome.toString());
      
      if (settings.homeOwnershipStatus) {
        formData.append('HomeOwnershipStatus', settings.homeOwnershipStatus);
      }
      
      if (settings.aadhaarNumber) {
        formData.append('AadhaarNumber', settings.aadhaarNumber);
      }
      
      if (settings.panNumber) {
        formData.append('PANNumber', settings.panNumber);
      }
      
      if (profileImage) {
        formData.append('profileImage', profileImage);
      }
      
      const response = await axiosInstance.put<ApiResponse<{ message: string }>>(`${API_ENDPOINTS.UPDATE_CUSTOMER_SETTINGS}/${userId}`, formData);
      return response;
    });
  },

};
