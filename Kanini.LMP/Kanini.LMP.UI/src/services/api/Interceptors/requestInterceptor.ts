import { AxiosError, type InternalAxiosRequestConfig } from "axios";
import { secureStorage } from '../../../utils/secureStorage';

export const requestInterceptor = (config: InternalAxiosRequestConfig) => {
  try {
    const token = secureStorage.getToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    
    // Remove Content-Type for FormData to let browser set it with boundary
    if (config.data instanceof FormData && config.headers) {
      delete config.headers['Content-Type'];
    }
  } catch (error) {
    console.error('Failed to set authorization header:', error);
  }
  return config;
};

export const requestErrorHandler = (error: AxiosError) => {
  return Promise.reject(error);
};