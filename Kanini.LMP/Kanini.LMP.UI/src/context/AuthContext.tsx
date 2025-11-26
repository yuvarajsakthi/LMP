import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { DecodedToken } from '../types';
import { authMiddleware } from '../middleware';

interface AuthContextType {
  token: DecodedToken | null;
  setToken: (token: DecodedToken | null) => void;
  isAuthenticated: boolean;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [token, setToken] = useState<DecodedToken | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  useEffect(() => {
    const storedToken = authMiddleware.getDecodedToken();
    if (storedToken && authMiddleware.isAuthenticated()) {
      setToken(storedToken);
      setIsAuthenticated(true);
    }
  }, []);

  const handleSetToken = (newToken: DecodedToken | null) => {
    setToken(newToken);
    setIsAuthenticated(!!newToken);
  };

  const logout = () => {
    authMiddleware.removeToken();
    setToken(null);
    setIsAuthenticated(false);
  };

  return (
    <AuthContext.Provider value={{ token, setToken: handleSetToken, isAuthenticated, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};