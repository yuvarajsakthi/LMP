import { jwtDecode } from 'jwt-decode';
import type { DecodedToken } from '../types';

export const authMiddleware = {
  getToken: (): string | null => {
    return localStorage.getItem('token');
  },

  setToken: (token: string): void => {
    localStorage.setItem('token', token);
  },

  removeToken: (): void => {
    localStorage.removeItem('token');
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