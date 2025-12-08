import axiosInstance from './axiosInstance';
import { jwtDecode } from 'jwt-decode';
import { secureStorage } from '../../utils/secureStorage';
import { ApiService } from './apiService';
import { API_ENDPOINTS } from '../../config';
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

const processTokenResponse = (responseData: LoginResponseData): { token: string; user: DecodedToken } => {
  let { token, role, username } = responseData;
  
  // Handle token as object with value property
  if (token && typeof token === 'object' && 'value' in token) {
    token = (token as any).value;
  }
  
  if (!token || typeof token !== 'string' || token.trim() === '') {
    console.error('Token validation failed:', { token, responseData });
    throw new Error('Invalid token received from server');
  }
  
  try {
    const decodedToken = jwtDecode<any>(token);
    
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
      customerId: decodedToken.CustomerId || decodedToken.customerId,
      CustomerId: decodedToken.CustomerId || decodedToken.customerId,
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
  async login(credentials: LoginCredentials): Promise<{ token: string; user: DecodedToken } | { requiresVerification: boolean; email: string; message: string }> {
    if (!credentials?.Username || !credentials?.PasswordHash) {
      throw new Error('Authentication credentials are required');
    }
    
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Username', credentials.Username);
      formData.append('PasswordHash', credentials.PasswordHash);
      
      const response = await axiosInstance.post<ApiResponse<LoginResponseData>>(
        API_ENDPOINTS.USER_LOGIN, 
        formData
      );
      return response;
    }).then(loginData => {
      if (loginData.requiresVerification) {
        return {
          requiresVerification: true,
          email: (loginData as any).email || credentials.Username,
          message: loginData.message || 'Account not verified'
        };
      }
      if (!loginData.token) {
        throw new Error('No token received from server');
      }
      return processTokenResponse(loginData as LoginResponseData & { token: string; username: string; role: string });
    });
  },

  async sendOTP(data: {
    email: string;
    purpose: 'LOGIN' | 'REGISTER' | 'FORGETPASSWORD';
  }): Promise<{ message: string; userId: number }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Email', data.email);
      formData.append('Purpose', data.purpose);
      
      const response = await axiosInstance.post<ApiResponse<{ message: string; userId: number }>>(
        API_ENDPOINTS.SEND_OTP,
        formData
      );
      return response;
    });
  },

  async loginWithOTP(data: {
    email: string;
    otp: string;
  }): Promise<{ token: string; user: DecodedToken }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Email', data.email);
      formData.append('OTP', data.otp);
      
      const response = await axiosInstance.post<ApiResponse<LoginResponseData>>(
        API_ENDPOINTS.VERIFY_LOGIN_OTP,
        formData
      );
      return response;
    }).then(loginData => {
      if (!loginData.token) {
        throw new Error('No token received from server');
      }
      return processTokenResponse(loginData as LoginResponseData & { token: string; username: string; role: string });
    });
  },

  async register(data: RegisterCredentials): Promise<{ message: string; userId: number }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('FullName', data.fullName);
      formData.append('Email', data.email);
      formData.append('Password', data.password);
      formData.append('DateOfBirth', data.dateOfBirth);
      formData.append('Gender', data.gender.toString());
      formData.append('PhoneNumber', data.phoneNumber);
      formData.append('PANNumber', data.panNumber);
      formData.append('AadhaarNumber', data.aadhaarNumber);
      formData.append('AnnualIncome', data.annualIncome.toString());
      if (data.homeOwnershipStatus !== undefined) {
        formData.append('HomeOwnershipStatus', data.homeOwnershipStatus.toString());
      }
      
      const response = await axiosInstance.post<ApiResponse<{ message: string; userId: number }>>(
        API_ENDPOINTS.USER_REGISTER,
        formData
      );
      return response;
    });
  },

  async verifyOTP(data: {
    email: string;
    otp: string;
    purpose: string;
  }): Promise<{ message: string; token?: string; username?: string; role?: string }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Email', data.email);
      formData.append('OTP', data.otp);
      formData.append('Purpose', data.purpose);
      
      const response = await axiosInstance.post<ApiResponse<{ message: string; token?: string; username?: string; role?: string }>>(
        API_ENDPOINTS.VERIFY_OTP,
        formData
      );
      return response;
    });
  },

  async forgotPassword(data: { email: string }): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Email', data.email);
      
      const response = await axiosInstance.post<ApiResponse<{ message: string }>>(
        API_ENDPOINTS.FORGOT_PASSWORD,
        formData
      );
      return response;
    });
  },

  async resetPassword(data: { 
    email: string; 
    otp: string; 
    newPassword: string 
  }): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const formData = new FormData();
      formData.append('Email', data.email);
      formData.append('OTP', data.otp);
      formData.append('NewPassword', data.newPassword);
      
      const response = await axiosInstance.post<ApiResponse<{ message: string }>>(
        API_ENDPOINTS.RESET_PASSWORD, 
        formData
      );
      return response;
    });
  },

  logout(): void {
    try {
      secureStorage.removeToken();
      sessionStorage.removeItem('eligibilityModalShown');
    } catch (error) {
      // Silent error handling
    }
  }
};