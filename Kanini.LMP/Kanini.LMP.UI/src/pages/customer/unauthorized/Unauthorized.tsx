import { Result, Button } from 'antd';
import { useNavigate } from 'react-router-dom';
import { COMMON_ROUTES } from '../../../config';

const Unauthorized = () => {
  const navigate = useNavigate();

  return (
    <Result
      status="403"
      title="403"
      subTitle="Sorry, you are not authorized to access this page."
      extra={
        <Button type="primary" onClick={() => navigate(COMMON_ROUTES.LOGIN)}>
          Back to Login
        </Button>
      }
    />
  );
};

export default Unauthorized;