import useAuthUser from 'react-auth-kit/hooks/useAuthUser';
import { Navigate } from 'react-router-dom';
import { USER_ROLES, ROUTES } from '../config';
import type { GuardProps, User } from '../types';

const CustomerGuard = ({ children }: GuardProps) => {
  try {
    const auth = useAuthUser();
    const user = auth as User | null;

    if (!user || user.role?.toLowerCase() !== USER_ROLES.CUSTOMER.toLowerCase()) {
      return <Navigate to={ROUTES.UNAUTHORIZED} replace />;
    }

    return <>{children}</>;
  } catch (error) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }
};

export default CustomerGuard;