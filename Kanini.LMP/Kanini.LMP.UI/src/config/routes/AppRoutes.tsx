import { Routes, Route, useNavigate } from 'react-router-dom';
import { useEffect } from 'react';
import { Login } from '../../pages';
import { ROUTES } from '..';
import { navigationService } from '../../services';

const AppRoutes = () => {
  const navigate = useNavigate();
  
  useEffect(() => {
    navigationService.setNavigate(navigate);
  }, [navigate]);

  return (
    <Routes>
      <Route path={ROUTES.LOGIN} element={<Login />} />
      <Route path="/" element={<Login />} />
      
    </Routes>
  );
};

export default AppRoutes;