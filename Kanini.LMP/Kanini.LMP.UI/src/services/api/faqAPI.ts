import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface FaqDTO {
  id?: number;
  customerId?: number;
  customerName?: string;
  question: string;
  answer?: string;
  status?: string;
  createdAt?: string;
  updatedAt?: string;
}

export const faqAPI = {
  async createFaq(faq: FaqDTO): Promise<FaqDTO> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('question', faq.question);
      if (faq.answer) formData.append('answer', faq.answer);
      if (faq.customerId) formData.append('customerId', String(faq.customerId));
      
      const response = await axiosInstance.post<ApiResponse<FaqDTO>>(
        API_ENDPOINTS.CREATE_FAQ,
        formData
      );
      return response;
    });
  },

  async getAllFaqs(): Promise<FaqDTO[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<FaqDTO[]>>(
        API_ENDPOINTS.GET_ALL_FAQS
      );
      return response;
    });
  },

  async getFaqById(id: number): Promise<FaqDTO> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<FaqDTO>>(
        `${API_ENDPOINTS.GET_FAQ_BY_ID}/${id}`
      );
      return response;
    });
  },

  async getFaqsByCustomerId(customerId: number): Promise<FaqDTO[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<FaqDTO[]>>(
        `${API_ENDPOINTS.GET_FAQS_BY_CUSTOMER}/${customerId}`
      );
      return response;
    });
  },

  async updateFaq(id: number, faq: FaqDTO): Promise<FaqDTO> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('question', faq.question);
      if (faq.answer) formData.append('answer', faq.answer);
      if (faq.status) formData.append('status', faq.status);
      
      const response = await axiosInstance.put<ApiResponse<FaqDTO>>(
        `${API_ENDPOINTS.UPDATE_FAQ}/${id}`,
        formData
      );
      return response;
    });
  },

  async deleteFaq(id: number): Promise<any> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.delete<ApiResponse<any>>(
        `${API_ENDPOINTS.DELETE_FAQ}/${id}`
      );
      return response;
    });
  }
};
