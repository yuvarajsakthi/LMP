import axios, { AxiosError, type AxiosResponse, type InternalAxiosRequestConfig } from "axios";
import { enqueueSnackbar } from "notistack";
import { API_ENDPOINTS, ROUTES } from '../../config';
import { navigationService } from '../../services';

interface ExtendedAxiosRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

const BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api";

export const responseInterceptor = (response: AxiosResponse) => response;

export const responseErrorHandler = async (error: AxiosError) => {
  const originalRequest = error.config as ExtendedAxiosRequestConfig;

  if (error.response?.status === 401 && !originalRequest?._retry) {
    originalRequest._retry = true;

    try {
      const refreshToken = localStorage.getItem("refreshToken");
      if (refreshToken) {
        const { data } = await axios.post(`${BASE_URL}${API_ENDPOINTS.REFRESH_TOKEN}`, { refreshToken });
        localStorage.setItem("accessToken", data.accessToken);
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        return axios(originalRequest);
      }
    } catch (refreshError) {
      console.error("Token refresh failed:", refreshError);
      localStorage.clear();
      navigationService.navigateTo(ROUTES.LOGIN, true);
    }
    
    localStorage.clear();
    navigationService.navigateTo(ROUTES.LOGIN, true);
  }

  const errorMessage = (error.response?.data as any)?.message || error.message || "An unexpected error occurred";
  
  const status = error.response?.status;
  
  if (status && status >= 500) {
    enqueueSnackbar("Server error. Please try again later.", { variant: "error" });
  } else if (status && status >= 400) {
    enqueueSnackbar(errorMessage, { variant: "error" });
  } else {
    enqueueSnackbar("Network error. Please check your connection.", { variant: "error" });
  }

  console.error("API Error:", errorMessage);
  return Promise.reject(error);
};