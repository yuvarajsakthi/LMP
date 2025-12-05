import { Navigate } from "react-router-dom";
import { useAuth } from "../../context";
import { COMMON_ROUTES } from "../../config";
import type { ReactNode } from "react";

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRole?: string;
}

const ProtectedRoute = ({ children, requiredRole }: ProtectedRouteProps) => {
  const { isAuthenticated, token } = useAuth();
  
  if (!isAuthenticated || !token) {
    return <Navigate to={COMMON_ROUTES.LOGIN} replace />;
  }
  
  if (requiredRole) {
    const userRole = token.role || token['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (!userRole || userRole.toLowerCase() !== requiredRole.toLowerCase()) {
      return <Navigate to={COMMON_ROUTES.UNAUTHORIZED} replace />;
    }
  }
  
  return <>{children}</>;
};

export default ProtectedRoute;