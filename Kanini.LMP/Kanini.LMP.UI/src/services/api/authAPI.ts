import axiosInstance from './axiosInstance';
import { jwtDecode } from 'jwt-decode';
import { API_ENDPOINTS } from '../../config';
import { secureStorage } from '../../utils/secureStorage';
import type { LoginCredentials, RegisterCredentials, LoginResponse, RegisterResponse, DecodedToken } from '../../types';

const processTokenResponse = (token: string): { token: string; user: DecodedToken } => {
  if (!token || typeof token !== 'string') {
    throw new Error('Invalid token received from server');
  }
  
  try {
    const decodedToken = jwtDecode<any>(token);
    console.log('Decoded token:', decodedToken);
    
    // Extract role from Microsoft JWT claim format
    const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decodedToken.role;
    
    if (!role) {
      throw new Error('Invalid token structure - missing role');
    }
    
    const user: DecodedToken = {
      role,
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
    try {
      const response = await axiosInstance.post<LoginResponse>(API_ENDPOINTS.USER_LOGIN, credentials);
      console.log('Login response:', response.data);
      if (!response.data || !response.data.token) {
        throw new Error('Invalid response from server');
      }
      const result = processTokenResponse(response.data.token);
      console.log('Login successful:', result);
      return result;
    } catch (error: any) {
      console.error('Login error:', error);
      throw new Error(error.response?.data?.message || error.message || 'Login failed');
    }
  },

  async register(credentials: RegisterCredentials): Promise<{ token: string; user: DecodedToken }> {
    try {
      const response = await axiosInstance.post<RegisterResponse>(API_ENDPOINTS.USER_REGISTER, credentials);
      if (!response.data || !response.data.token) {
        throw new Error('Invalid response from server');
      }
      return processTokenResponse(response.data.token);
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Registration failed');
    }
  },

  async forgotPassword(email: string): Promise<{ success: boolean; message: string }> {
    try {
      await axiosInstance.post(API_ENDPOINTS.FORGOT_PASSWORD, { email });
      return { success: true, message: 'OTP sent successfully' };
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to send OTP');
    }
  },

  async resetPassword(data: { email: string; resetToken: string; newPassword: string }): Promise<{ success: boolean; message: string }> {
    try {
      await axiosInstance.post(API_ENDPOINTS.RESET_PASSWORD, data);
      return { success: true, message: 'Password reset successfully' };
    } catch (error: any) {
      throw new Error(error.response?.data?.message || 'Failed to reset password');
    }
  },

  logout(): void {
    secureStorage.removeToken();
  }
};