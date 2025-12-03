import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Login, Register, ForgotPassword, FAQ, EmiCalculatorPage, ViewStatus, Settings, LoanTypes, Unauthorized } from '../../pages';
import CustomerDashboard from '../../pages/customer/customerDashboard/CustomerDashboard';
import LoanApplicationForm from '../../pages/customer/loanApplicationForm/LoanApplicationForm';
import ProtectedRoute from '../../components/custom/ProtectedRoute';
import { COMMON_ROUTES, CUSTOMER_ROUTES } from '..';
import { navigationService } from '../../services';
import { useAuth } from '../../context';

const AppRoutes = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  useEffect(() => {
    navigationService.setNavigate(navigate);
  }, [navigate]);

  const getDefaultRoute = () => {
    return isAuthenticated ? CUSTOMER_ROUTES.CUSTOMER_DASHBOARD : COMMON_ROUTES.LOGIN;
  };

  return (
    <Routes>
      <Route index element={<Navigate to={getDefaultRoute()} replace />} />
      
      {/* Public Routes */}
      <Route path={COMMON_ROUTES.LOGIN} element={<Login />} />
      <Route path={COMMON_ROUTES.REGISTER} element={<Register />} />
      <Route path={COMMON_ROUTES.FORGOT_PASSWORD} element={<ForgotPassword />} />
      <Route path={COMMON_ROUTES.UNAUTHORIZED} element={<Unauthorized />} />
      
      {/* Protected Customer Routes */}
      <Route path={CUSTOMER_ROUTES.CUSTOMER_DASHBOARD} element={
        <ProtectedRoute>
          <CustomerDashboard />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.LOAN_TYPES} element={
        <ProtectedRoute>
          <LoanTypes />
        </ProtectedRoute>
      } />

      <Route path={CUSTOMER_ROUTES.VIEWSTATUS} element={
        <ProtectedRoute>
          <ViewStatus />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.EMI_CALCULATOR} element={
        <ProtectedRoute>
          <EmiCalculatorPage />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.FAQ} element={
        <ProtectedRoute>
          <FAQ />
        </ProtectedRoute>
      } />
      <Route path={CUSTOMER_ROUTES.SETTINGS} element={
        <ProtectedRoute>
          <Settings />
        </ProtectedRoute>
      } />
      <Route path={COMMON_ROUTES.LOAN_APPLICATION_FORM} element={
        <ProtectedRoute>
          <LoanApplicationForm />
        </ProtectedRoute>
      } />
      
      {/* Catch all route */}
      <Route path="*" element={<Navigate to={getDefaultRoute()} replace />} />
    </Routes>
  );
};

export default AppRoutes;