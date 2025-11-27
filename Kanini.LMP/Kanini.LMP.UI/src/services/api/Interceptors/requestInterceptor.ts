import { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { secureStorage } from '../../../utils/secureStorage';

export const requestInterceptor = (config: InternalAxiosRequestConfig) => {
  try {
    const token = secureStorage.getToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  } catch (error) {
    console.error('Failed to set authorization header:', error);
  }
  return config;
};

export const requestErrorHandler = (error: AxiosError) => {
  return Promise.reject(error);
};