import axiosInstance from './axiosInstance';
import { jwtDecode } from 'jwt-decode';
import { API_ENDPOINTS } from '../../config';
import { secureStorage } from '../../utils/secureStorage';
import { ApiService } from './apiService';
import type { LoginCredentials, RegisterCredentials, DecodedToken, ApiResponse } from '../../types';

// Backend response types (matching C# controller responses)
interface LoginResponseData {
  token?: string;
  username?: string;
  role?: string;
  requiresVerification?: boolean;
  message?: string;
  userId?: number;
}

interface RegisterResponseData {
  message: string;
  userId: number;
  email: string;
  fullName: string;
}

const processTokenResponse = (responseData: LoginResponseData): { token: string; user: DecodedToken } => {
  const { token, role, username } = responseData;
  
  if (!token || typeof token !== 'string') {
    throw new Error('Invalid token received from server');
  }
  
  try {
    const decodedToken = jwtDecode<any>(token);
    console.log('Decoded token:', decodedToken);
    
    // Use role from response data (more reliable than JWT parsing)
    const userRole = role || decodedToken.role || 
                     decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    
    if (!userRole) {
      throw new Error('Invalid token structure - missing role');
    }
    
    const user: DecodedToken = {
      role: userRole,
      FullName: username || decodedToken.name || decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      username: username,
      email: decodedToken.email,
      ...decodedToken
    };
    
    secureStorage.setToken(token);
    return { token, user };
  } catch (error) {
    console.error('Token decode error:', error);
    throw new Error(`Failed to process authentication token: ${error}`);
  }
};

export const authAPI = {
  async login(credentials: LoginCredentials): Promise<{ token?: string; user?: DecodedToken; requiresVerification?: boolean; message?: string; userId?: number }> {
    if (!credentials?.username || !credentials?.password) {
      throw new Error('Authentication credentials are required');
    }
    
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<LoginResponseData>>(
        API_ENDPOINTS.USER_LOGIN, 
        credentials
      );
      return response;
    }).then(loginData => {
      if (loginData.requiresVerification) {
        return {
          requiresVerification: true,
          message: loginData.message,
          userId: loginData.userId
        };
      }
      
      if (!loginData.token) {
        throw new Error('No token received from server');
      }
      
      const tokenResponse = processTokenResponse(loginData as LoginResponseData & { token: string; username: string; role: string });
      return {
        requiresVerification: false,
        ...tokenResponse
      };
    });
  },

  async register(credentials: RegisterCredentials): Promise<RegisterResponseData> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<RegisterResponseData>>(
        API_ENDPOINTS.USER_REGISTER, 
        credentials
      );
      return response;
    });
  },



  async resetPassword(data: { 
    userId: number; 
    otp: string; 
    newPassword: string 
  }): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{ message: string }>>(
        API_ENDPOINTS.RESET_PASSWORD, 
        data
      );
      return response;
    });
  },

  async sendOTP(data: {
    email: string;
    phoneNumber?: string;
    purpose: 'LOGIN' | 'REGISTRATION' | 'PASSWORD_RESET';
  }): Promise<{ message: string; userId: number }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{ message: string; userId: number }>>(
        API_ENDPOINTS.SEND_OTP,
        data
      );
      return response;
    });
  },

  async verifyOTP(data: {
    userId: number;
    otp: string;
  }): Promise<{ message: string; token?: string; user?: DecodedToken }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<LoginResponseData & { message: string }>>(
        API_ENDPOINTS.VERIFY_OTP,
        data
      );
      return response;
    }).then(verifyData => {
      if (verifyData.token) {
        const tokenResponse = processTokenResponse(verifyData as LoginResponseData & { token: string; username: string; role: string });
        return {
          message: verifyData.message || 'Account verified successfully',
          ...tokenResponse
        };
      }
      return {
        message: verifyData.message || 'Account verified successfully'
      };
    });
  },

  async verifyLoginOTP(data: {
    userId: number;
    otp: string;
  }): Promise<{ token: string; user: DecodedToken }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<LoginResponseData>>(
        API_ENDPOINTS.VERIFY_LOGIN_OTP,
        data
      );
      return response;
    }).then(loginData => {
      if (!loginData.token) {
        throw new Error('No token received from server');
      }
      return processTokenResponse(loginData);
    });
  },

  logout(): void {
    try {
      secureStorage.removeToken();
      sessionStorage.removeItem('eligibilityModalShown');
    } catch (error) {
      console.error('Logout error:', error);
    }
  }
};