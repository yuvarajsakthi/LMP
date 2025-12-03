import { Navigate } from 'react-router-dom';
import { useAuth } from '../context';
import { ROUTES } from '../config';
import type { GuardProps } from '../types';

const AuthGuard = ({ children }: GuardProps) => {
  const { isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }

  return <>{children}</>;
};

export default AuthGuard;