import axiosInstance from './axiosInstance';
import { API_ENDPOINTS } from '../../config';
import { ApiService } from './apiService';
import type { ApiResponse } from '../../types';

export interface Notification {
  notificationId: number;
  userId: number;
  title: string;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: string;
  readAt?: string;
}

export const notificationAPI = {
  async getAllNotifications(): Promise<Notification[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Notification[]>>(API_ENDPOINTS.GET_ALL_NOTIFICATIONS);
      return response;
    });
  },

  async deleteNotification(notificationId: number): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.delete<ApiResponse<{ message: string }>>(
        `${API_ENDPOINTS.DELETE_NOTIFICATION}/${notificationId}`
      );
      return response;
    });
  }
};