import axiosInstance from './axiosInstance';
import { jwtDecode } from 'jwt-decode';
import { API_ENDPOINTS } from '../../config';
import { secureStorage } from '../../utils/secureStorage';
import { ApiService } from './apiService';
import type { LoginCredentials, RegisterCredentials, DecodedToken, ApiResponse } from '../../types';

// Backend response types (matching C# controller responses)
interface LoginResponseData {
  token: string;
  username: string;
  role: string;
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
      FullName: username,
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
  async login(credentials: LoginCredentials): Promise<{ token: string; user: DecodedToken }> {
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
      if (!loginData.token) {
        throw new Error('No token received from server');
      }
      return processTokenResponse(loginData);
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

  async forgotPassword(email: string): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{}>>(
        API_ENDPOINTS.FORGOT_PASSWORD, 
        { email }
      );
      return response;
    }).then(() => ({ message: 'Password reset email sent successfully' }));
  },

  async resetPassword(data: { 
    email: string; 
    resetToken: string; 
    newPassword: string 
  }): Promise<{ message: string }> {
    return ApiService.execute(async () => {
      const response = await axiosInstance.post<ApiResponse<{}>>(
        API_ENDPOINTS.RESET_PASSWORD, 
        data
      );
      return response;
    }).then(() => ({ message: 'Password reset successfully' }));
  },

  logout(): void {
    try {
      secureStorage.removeToken();
    } catch (error) {
      console.error('Logout error:', error);
    }
  }
};