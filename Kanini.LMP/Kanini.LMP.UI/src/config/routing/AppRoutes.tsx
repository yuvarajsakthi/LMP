import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Login, Register, ForgotPassword, FAQ, EmiCalculatorPage, ViewStatus, Settings, ApplyLoan } from '../../pages';
import CustomerDashboard from '../../pages/customer/customerDashboard/CustomerDashboard';
import ProtectedRoute from '../../components/custom/ProtectedRoute';
import { ROUTES } from '..';
import { navigationService } from '../../services';

const AppRoutes = () => {
  const navigate = useNavigate();

  useEffect(() => {
    navigationService.setNavigate(navigate);
  }, [navigate]);

  return (
    <Routes>
      <Route index element={<Navigate to={ROUTES.LOGIN} replace />} />
      <Route path={ROUTES.LOGIN} element={<Login />} />
      <Route path={ROUTES.REGISTER} element={<Register />} />
      <Route path={ROUTES.FORGOT_PASSWORD} element={<ForgotPassword />} />
      <Route path={ROUTES.CUSTOMER_DASHBOARD} element={
        <ProtectedRoute>
          <CustomerDashboard />
        </ProtectedRoute>
      } />
      <Route path={ROUTES.LOAN_TYPES} element={
        <ProtectedRoute>
          <ApplyLoan />
        </ProtectedRoute>
      } />
      <Route path={ROUTES.INTEGRATION} element={
        <ProtectedRoute>
          <ViewStatus />
        </ProtectedRoute>
      } />
      <Route path={ROUTES.EMI_CALCULATOR} element={
        <ProtectedRoute>
          <EmiCalculatorPage />
        </ProtectedRoute>
      } />
      <Route path={ROUTES.FAQ} element={
        <ProtectedRoute>
          <FAQ />
        </ProtectedRoute>
      } />
      <Route path={ROUTES.SETTINGS} element={
        <ProtectedRoute>
          <Settings />
        </ProtectedRoute>
      } />
    </Routes>
  );
};

export default AppRoutes;