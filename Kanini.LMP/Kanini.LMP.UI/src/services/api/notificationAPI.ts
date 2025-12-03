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
  async getMyNotifications(): Promise<Notification[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Notification[]>>(
        API_ENDPOINTS.GET_MY_NOTIFICATIONS
      );
      return response;
    });
  },

  async getUnreadNotifications(): Promise<Notification[]> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<Notification[]>>(
        API_ENDPOINTS.GET_UNREAD_NOTIFICATIONS
      );
      return response;
    });
  },

  async getUnreadCount(): Promise<{ count: number }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.get<ApiResponse<{ count: number }>>(
        API_ENDPOINTS.GET_UNREAD_COUNT
      );
      return response;
    });
  },

  async markAsRead(notificationId: number): Promise<Notification> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<Notification>>(
        `${API_ENDPOINTS.MARK_NOTIFICATION_READ}/${notificationId}`
      );
      return response;
    });
  },

  async markAllAsRead(): Promise<{ success: boolean; message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.put<ApiResponse<{ success: boolean; message: string }>>(
        API_ENDPOINTS.MARK_ALL_NOTIFICATIONS_READ
      );
      return response;
    });
  }
};