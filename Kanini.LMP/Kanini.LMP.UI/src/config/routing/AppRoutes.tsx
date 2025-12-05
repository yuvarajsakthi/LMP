import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Login, Register, ForgotPassword, FAQ, EmiCalculatorPage, ViewStatus, Settings, LoanTypes, Unauthorized, LoanApplicationForm } from '../../pages';
import CustomerDashboard from '../../pages/customer/customerDashboard/CustomerDashboard';
import ProtectedRoute from '../../components/custom/ProtectedRoute';
import { COMMON_ROUTES, CUSTOMER_ROUTES, MANAGER_ROUTES } from '..';
import { navigationService } from '../../services';
import { useAuth } from '../../context';
import ManagerDashboard from '../../pages/manager/managerDashboard/ManagerDashboard';
import AppliedLoans from '../../pages/manager/appliedLoans/AppliedLoans';
import LoanDetails from '../../pages/manager/loanDetails/LoanDetails';

const AppRoutes = () => {
  const navigate = useNavigate();
  const { isAuthenticated, token } = useAuth();

  useEffect(() => {
    navigationService.setNavigate(navigate);
  }, [navigate]);

  const getDefaultRoute = () => {
    if (!isAuthenticated) return COMMON_ROUTES.LOGIN;
    
    const userRole = token?.role?.toLowerCase();
    if (userRole === 'manager') {
      return MANAGER_ROUTES.MANAGER_DASHBOARD;
    }
    return CUSTOMER_ROUTES.CUSTOMER_DASHBOARD;
  };

  return (
    <Routes>
      <Route index element={<Navigate to={getDefaultRoute()} replace />} />
      
      {/* Public Routes */}
      <Route path={COMMON_ROUTES.LOGIN} element={
        isAuthenticated ? <Navigate to={getDefaultRoute()} replace /> : <Login />
      } />
      <Route path={COMMON_ROUTES.REGISTER} element={
        isAuthenticated ? <Navigate to={getDefaultRoute()} replace /> : <Register />
      } />
      <Route path={COMMON_ROUTES.FORGOT_PASSWORD} element={<ForgotPassword />} />
      <Route path={COMMON_ROUTES.UNAUTHORIZED} element={<Unauthorized />} />
      
      {/* Protected Customer Routes */}
      <Route path={CUSTOMER_ROUTES.CUSTOMER_DASHBOARD} element={
        <ProtectedRoute requiredRole="customer">
          <CustomerDashboard />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.LOAN_TYPES} element={
        <ProtectedRoute requiredRole="customer">
          <LoanTypes />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.LOAN_APPLICATION} element={
        <ProtectedRoute requiredRole="customer">
          <LoanApplicationForm />
        </ProtectedRoute>
      } />

      <Route path={CUSTOMER_ROUTES.VIEWSTATUS} element={
        <ProtectedRoute requiredRole="customer">
          <ViewStatus />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.EMI_CALCULATOR} element={
        <ProtectedRoute requiredRole="customer">
          <EmiCalculatorPage />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.FAQ} element={
        <ProtectedRoute requiredRole="customer">
          <FAQ />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.SETTINGS} element={
        <ProtectedRoute requiredRole="customer">
          <Settings />
        </ProtectedRoute>
      } />

      {/* Protected Manager Routes */}
      <Route path={MANAGER_ROUTES.MANAGER_DASHBOARD} element={
        <ProtectedRoute requiredRole="manager">
          <ManagerDashboard />
        </ProtectedRoute>
      } />
      <Route path={MANAGER_ROUTES.APPLIED_LOAN} element={
        <ProtectedRoute requiredRole="manager">
          <AppliedLoans />
        </ProtectedRoute>
      } />
      <Route path="/manager/applied-loans/:applicationNumber" element={
        <ProtectedRoute requiredRole="manager">
          <LoanDetails />
        </ProtectedRoute>
      } />
      
      {/* Catch all route */}
      <Route path="*" element={<Navigate to={getDefaultRoute()} replace />} />
    </Routes>
  );
};

export default AppRoutes;