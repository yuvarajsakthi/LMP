import { Navigate } from 'react-router-dom';
import { useAuth } from '../context';
import { USER_ROLES, ROUTES } from '../config';
import type { GuardProps } from '../types';

const CustomerGuard = ({ children }: GuardProps) => {
  const { isAuthenticated, token } = useAuth();

  if (!isAuthenticated || !token || token.role?.toLowerCase() !== USER_ROLES.CUSTOMER.toLowerCase()) {
    return <Navigate to={ROUTES.UNAUTHORIZED} replace />;
  }

  return <>{children}</>;
};

export default CustomerGuard;