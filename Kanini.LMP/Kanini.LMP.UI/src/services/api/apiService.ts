import type { AxiosResponse, AxiosError } from 'axios';
import { enqueueSnackbar } from 'notistack';
import type { ApiResponse, ErrorResponse } from '../../types';
import { ApiException } from '../../types';

export class ApiService {

  static handleSuccess<T>(response: AxiosResponse<ApiResponse<T>>): T {
    const { data } = response;
    
    // Backend returns success: false for business logic errors
    if (data.success === false) {
      throw new ApiException(
        data.message || 'Operation failed',
        response.status,
        data.errors
      );
    }

    // Show success message if provided and not generic
    if (data.message && data.message.toLowerCase() !== 'success') {
      enqueueSnackbar(data.message, { variant: 'success' });
    }

    // If data.data exists, return it; otherwise return data itself
    return (data.data !== undefined ? data.data : data) as T;
  }

  static handleError(error: AxiosError): never {
    let message = 'An unexpected error occurred';
    let errors: string[] = [];
    let statusCode = error.response?.status || 500;
    let details: string | undefined;

    if (error.response?.data) {
      const responseData = error.response.data as any;
      
      // Check if it's a structured ApiResponse (from controllers)
      if (typeof responseData.success === 'boolean') {
        const apiResponse = responseData as ApiResponse;
        message = apiResponse.message || message;
        errors = apiResponse.errors || [];
      }
      // Check if it's a GlobalExceptionMiddleware response
      else if (responseData.statusCode && responseData.message) {
        const errorResponse = responseData as ErrorResponse;
        message = errorResponse.message;
        details = errorResponse.details;
        statusCode = errorResponse.statusCode;
      }
      // Fallback for other error formats
      else {
        message = responseData.message || responseData.title || message;
        if (responseData.errors) {
          errors = Array.isArray(responseData.errors) ? responseData.errors : [responseData.errors];
        }
      }
    } else if (error.request) {
      // Network error
      message = 'Network error. Please check your connection.';
      statusCode = 0;
    } else {
      // Request setup error
      message = error.message || message;
    }

    // Show error messages to user
    this.showErrorMessages(message, errors, details);

    throw new ApiException(message, statusCode, errors, details);
  }

  private static showErrorMessages(message: string, errors?: string[], details?: string): void {
    // Don't show authentication errors on dashboard (user may not be logged in)
    const authErrors = ['Failed to retrieve loan products', 'Failed to retrieve eligibility score'];
    if (authErrors.some(err => message.includes(err))) {
      return;
    }

    // Show main error message
    enqueueSnackbar(message, { variant: 'error' });

    // Show additional error details if available
    if (errors && errors.length > 0) {
      errors.forEach(err => {
        if (err !== message) {
          enqueueSnackbar(err, { variant: 'error' });
        }
      });
    }

    // Show details if available and different from message
    if (details && details !== message) {
      enqueueSnackbar(details, { variant: 'error' });
    }
  }

  /**
   * Wraps API calls with structured error handling
   */
  static async execute<T>(
    apiCall: () => Promise<AxiosResponse<ApiResponse<T>>>
  ): Promise<T> {
    try {
      const response = await apiCall();
      return this.handleSuccess<T>(response);
    } catch (error) {
      return this.handleError(error as AxiosError);
    }
  }

  static createSuccessResponse<T>(data: T, message?: string): ApiResponse<T> {
    return {
      success: true,
      data,
      message
    };
  }

  static createErrorResponse(
    message: string,
    errors?: string[]
  ): ApiResponse<null> {
    return {
      success: false,
      message,
      errors
    };
  }
}