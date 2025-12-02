import { jwtDecode } from 'jwt-decode';
import type { DecodedToken } from '../types';
import { secureStorage } from '../utils/secureStorage';

export const authMiddleware = {
  getToken: (): string | null => {
    return secureStorage.getToken();
  },

  setToken: (token: string): void => {
    if (!token || typeof token !== 'string') {
      throw new Error('Invalid token format');
    }
    secureStorage.setToken(token);
  },

  removeToken: (): void => {
    secureStorage.removeToken();
  },

  isAuthenticated: (): boolean => {
    const token = authMiddleware.getToken();
    if (!token) return false;
    
    try {
      const decoded: DecodedToken = jwtDecode(token);
      if (decoded.exp && decoded.exp <= Date.now() / 1000) {
        authMiddleware.removeToken();
        return false;
      }
      return true;
    } catch {
      authMiddleware.removeToken();
      return false;
    }
  },

  getDecodedToken: (): DecodedToken | null => {
    const token = authMiddleware.getToken();
    if (!token) return null;
    
    try {
      return jwtDecode(token);
    } catch {
      return null;
    }
  },

  getUserRole: (): string | null => {
    const decoded = authMiddleware.getDecodedToken();
    return decoded?.role || null;
  }
};