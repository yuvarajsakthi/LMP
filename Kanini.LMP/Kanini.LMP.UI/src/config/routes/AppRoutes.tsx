import { Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Login, Register, ForgotPassword } from '../../pages';
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
    </Routes>
  );
};

export default AppRoutes;