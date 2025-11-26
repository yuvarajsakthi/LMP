import { jwtDecode } from 'jwt-decode';
import type { DecodedToken } from '../types';
import { secureStorage } from '../utils/secureStorage';

export const authMiddleware = {
  getToken: (): string | null => {
    return secureStorage.getToken();
  },

  setToken: (token: string): void => {
    if (!token || typeof token !== 'string' || !/^[A-Za-z0-9._-]+$/.test(token)) {
      throw new Error('Invalid token format');
    }
    const sanitizedToken = token.replace(/[^A-Za-z0-9._-]/g, '');
    secureStorage.setToken(sanitizedToken);
  },

  removeToken: (): void => {
    secureStorage.removeToken();
  },

  isAuthenticated: (): boolean => {
    const token = authMiddleware.getToken();
    if (!token) return false;
    
    try {
      const decoded: DecodedToken = jwtDecode(token);
      return decoded.exp ? decoded.exp > Date.now() / 1000 : true;
    } catch {
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