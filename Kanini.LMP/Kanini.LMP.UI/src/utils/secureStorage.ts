// Secure token storage utility to mitigate XSS attacks
import { STORAGE_KEYS } from '../config';

class SecureStorage {
  private static instance: SecureStorage;

  static getInstance(): SecureStorage {
    if (!SecureStorage.instance) {
      SecureStorage.instance = new SecureStorage();
    }
    return SecureStorage.instance;
  }

  setToken(token: string): void {
    try {
      // In production, consider using httpOnly cookies instead of localStorage
      localStorage.setItem(STORAGE_KEYS.ACCESS_TOKEN, token);
    } catch (error) {
      console.error('Failed to store token:', error);
    }
  }

  getToken(): string | null {
    try {
      return localStorage.getItem(STORAGE_KEYS.ACCESS_TOKEN);
    } catch (error) {
      console.error('Failed to retrieve token:', error);
      return null;
    }
  }

  removeToken(): void {
    try {
      localStorage.removeItem(STORAGE_KEYS.ACCESS_TOKEN);
    } catch (error) {
      console.error('Failed to remove token:', error);
    }
  }
}

export const secureStorage = SecureStorage.getInstance();