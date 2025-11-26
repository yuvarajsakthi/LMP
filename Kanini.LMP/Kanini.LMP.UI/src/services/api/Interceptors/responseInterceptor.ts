import axios, { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from "axios";
import { enqueueSnackbar } from "notistack";
import { API_ENDPOINTS, ROUTES } from '../../../config';
import { navigationService } from '../..';
import { secureStorage } from '../../../utils/secureStorage';

interface ExtendedAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5156";

export const responseInterceptor = (response: AxiosResponse) => response;

export const responseErrorHandler = async (error: AxiosError) => {
  const originalRequest = error.config as ExtendedAxiosRequestConfig;

  if (error.response?.status === 401 && !originalRequest?._retry) {
    originalRequest._retry = true;

    try {
      const refreshToken = secureStorage.getRefreshToken();
      if (refreshToken) {
        const { data } = await axios.post(`${BASE_URL}${API_ENDPOINTS.REFRESH_TOKEN}`, { refreshToken });
        const sanitizedToken = data.accessToken?.replace(/[^A-Za-z0-9._-]/g, '') || '';
        secureStorage.setToken(sanitizedToken);
        originalRequest.headers.Authorization = `Bearer ${sanitizedToken}`;
        return axios(originalRequest);
      }
    } catch (refreshError) {
      console.error("Token refresh failed:", refreshError);
      try {
        secureStorage.removeToken();
      } catch (storageError) {
        console.error("Failed to remove token:", storageError);
      }
      try {
        navigationService.navigateTo(ROUTES.LOGIN, true);
      } catch (navError) {
        console.error("Navigation failed:", navError);
      }
    }
    
    try {
      secureStorage.removeToken();
    } catch (storageError) {
      console.error("Failed to remove token:", storageError);
    }
    try {
      navigationService.navigateTo(ROUTES.LOGIN, true);
    } catch (navError) {
      console.error("Navigation failed:", navError);
    }
  }

  let errorMessage = "An unexpected error occurred";
  try {
    errorMessage = (error.response?.data as any)?.message || error.message || "An unexpected error occurred";
  } catch (parseError) {
    console.error("Failed to parse error message:", parseError);
  }
  
  const status = error.response?.status;
  

  console.error("API Error:", errorMessage);
  return Promise.reject(error);
};