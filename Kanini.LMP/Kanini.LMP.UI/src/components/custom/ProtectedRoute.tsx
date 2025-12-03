import { Navigate } from "react-router-dom";
import { Spin } from "antd";
import { useAuth } from "../../context";
import { authMiddleware } from "../../middleware";
import { COMMON_ROUTES } from "../../config";
import type { ReactNode } from "react";

interface ProtectedRouteProps {
  children: ReactNode;
  requiredRole?: string;
}

const ProtectedRoute = ({ children, requiredRole }: ProtectedRouteProps) => {
  const { isAuthenticated, token } = useAuth();
  
  // Show loading while auth is being determined
  if (token === undefined) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Spin size="large" />
      </div>
    );
  }
  
  // Check authentication
  const contextAuth = isAuthenticated && token;
  const middlewareAuth = authMiddleware.isAuthenticated();
  const hasValidToken = contextAuth && middlewareAuth;
  
  if (!hasValidToken) {
    authMiddleware.removeToken();
    return <Navigate to={COMMON_ROUTES.LOGIN} replace />;
  }
  
  // Check role if required
  if (requiredRole && token?.role?.toLowerCase() !== requiredRole.toLowerCase()) {
    return <Navigate to={COMMON_ROUTES.UNAUTHORIZED} replace />;
  }
  
  return <>{children}</>;
};

export default ProtectedRoute;