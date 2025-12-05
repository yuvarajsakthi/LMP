import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { useDispatch } from 'react-redux';
import type { DecodedToken } from '../types';
import { authMiddleware } from '../middleware';
import { setToken as setReduxToken } from '../store/slices/authSlice';
import type { AppDispatch } from '../store';

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
  const [isInitialized, setIsInitialized] = useState(false);
  const dispatch = useDispatch<AppDispatch>();

  useEffect(() => {
    const initializeAuth = () => {
      try {
        const rawToken = authMiddleware.getToken();
        
        if (rawToken) {
          const isValid = authMiddleware.isAuthenticated();
          
          if (isValid) {
            const storedToken = authMiddleware.getDecodedToken();
            if (storedToken) {
              const enhancedToken = {
                ...storedToken,
                FullName: storedToken.FullName || storedToken.name || storedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                role: storedToken.role || storedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
              };
              setToken(enhancedToken);
              setIsAuthenticated(true);
              dispatch(setReduxToken({ token: rawToken, user: enhancedToken }));
            } else {
              setToken(null);
              setIsAuthenticated(false);
            }
          } else {
            setToken(null);
            setIsAuthenticated(false);
          }
        } else {
          setToken(null);
          setIsAuthenticated(false);
        }
      } catch (error) {
        console.error('Auth initialization error:', error);
        setToken(null);
        setIsAuthenticated(false);
      } finally {
        setIsInitialized(true);
      }
    };
    
    initializeAuth();
  }, [dispatch]);

  const handleSetToken = (newToken: DecodedToken | null) => {
    try {
      if (newToken && (!newToken.role || typeof newToken.role !== 'string')) {
        throw new Error('Invalid token structure');
      }
      setToken(newToken);
      setIsAuthenticated(!!newToken);
      
      // Store token in localStorage when setting
      if (newToken) {
        const rawToken = authMiddleware.getToken();
        if (rawToken) {
          dispatch(setReduxToken({ token: rawToken, user: newToken }));
        }
      }
    } catch (error) {
      console.error('Failed to set token:', error);
      setToken(null);
      setIsAuthenticated(false);
    }
  };

  const logout = () => {
    try {
      authMiddleware.removeToken();
      sessionStorage.clear();
      setToken(null);
      setIsAuthenticated(false);
    } catch (error) {
      console.error('Logout failed:', error);
      sessionStorage.clear();
      setToken(null);
      setIsAuthenticated(false);
    }
  };

  // Don't render children until auth is initialized to prevent flash
  if (!isInitialized) {
    return null; // or a loading spinner
  }

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