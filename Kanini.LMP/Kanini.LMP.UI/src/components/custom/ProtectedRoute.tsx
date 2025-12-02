import { Navigate } from "react-router-dom";
import { useAuth } from "../../context";
import { ROUTES } from "../../config";
import type { ReactNode } from "react";

interface ProtectedRouteProps {
  children: ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { isAuthenticated, token } = useAuth();
  
  // Check both context state and actual token validity
  const hasValidToken = isAuthenticated && token;
  
  if (!hasValidToken) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }
  
  return <>{children}</>;
};

export default ProtectedRoute;