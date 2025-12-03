import { AxiosError, type AxiosResponse } from "axios";
import { enqueueSnackbar } from "notistack";
import { COMMON_ROUTES } from '../../../config';
import { navigationService } from '../..';
import { secureStorage } from '../../../utils/secureStorage';
import type { ApiResponse } from '../../../types';

export const responseInterceptor = (response: AxiosResponse<ApiResponse>) => {
  // Handle structured API responses from backend
  if (response.data && typeof response.data === 'object') {
    // If backend returns success: false, treat as error (business logic error)
    if (response.data.success === false) {
      const error = new Error(response.data.message || 'Operation failed');
      (error as any).response = response;
      throw error;
    }
  }
  return response;
};

export const responseErrorHandler = async (error: AxiosError) => {
  // Handle 401 Unauthorized
  if (error.response?.status === 401) {
    handleAuthFailure();
    return Promise.reject(error);
  }

  // Handle structured error responses
  handleApiError(error);
  return Promise.reject(error);
};

function handleAuthFailure(): void {
  try {
    secureStorage.removeToken();
    enqueueSnackbar('Session expired. Please login again.', { variant: 'warning' });
    navigationService.navigateTo(COMMON_ROUTES.LOGIN, true);
  } catch (error) {
    console.error("Failed to handle auth failure:", error);
  }
}

function handleApiError(error: AxiosError): void {
  let errorMessage = "An unexpected error occurred";
  let errors: string[] = [];
  
  try {
    const responseData = error.response?.data as ApiResponse;
    
    if (responseData) {
      errorMessage = responseData.message || errorMessage;
      errors = responseData.errors || [];
    } else {
      errorMessage = error.message || errorMessage;
    }
  } catch (parseError) {
    console.error("Failed to parse error response:", parseError);
    errorMessage = error.message || errorMessage;
  }
  
  // Show main error message
  enqueueSnackbar(errorMessage, { variant: 'error' });
  
  // Show additional error details
  errors.forEach(err => {
    if (err !== errorMessage) {
      enqueueSnackbar(err, { variant: 'error' });
    }
  });
  
  console.error("API Error:", {
    message: errorMessage,
    errors,
    status: error.response?.status,
    url: error.config?.url
  });
}