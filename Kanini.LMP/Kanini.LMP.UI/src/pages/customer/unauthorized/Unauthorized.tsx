import { Result, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../../context';
import { CUSTOMER_ROUTES, MANAGER_ROUTES } from '../../../config';

const Unauthorized = () => {
  const navigate = useNavigate();
  const { token } = useAuth();

  const handleGoBack = () => {
    const isManager = token?.role?.toLowerCase() === 'manager';
    navigate(isManager ? MANAGER_ROUTES.MANAGER_DASHBOARD : CUSTOMER_ROUTES.CUSTOMER_DASHBOARD, { replace: true });
  };

  return (
    <Result
      status="403"
      title="403"
      subTitle="Sorry, you are not authorized to access this page."
      extra={
        <Button type="primary" onClick={handleGoBack}>
          Back to Dashboard
        </Button>
      }
    />
  );
};

export default Unauthorized;