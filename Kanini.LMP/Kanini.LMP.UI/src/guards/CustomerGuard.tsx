import useAuthUser from 'react-auth-kit/hooks/useAuthUser';
import { Navigate } from 'react-router-dom';
import { USER_ROLES, ROUTES } from '../config';
import type { GuardProps, User } from '../types';

const CustomerGuard = ({ children }: GuardProps) => {
  const auth = useAuthUser();
  const user = auth as User | null;

  if (!user || user.role?.toLowerCase() !== USER_ROLES.CUSTOMER) {
    return <Navigate to={ROUTES.UNAUTHORIZED} replace />;
  }

  return <>{children}</>;
};

export default CustomerGuard;