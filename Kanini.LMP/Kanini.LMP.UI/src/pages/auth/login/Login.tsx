import { Row, Col } from 'antd';
import { LoginComponent, LoginRegister } from '../../../components';


const Login = () => {

    return (

        <Row gutter={24}>
            <Col xs={24} md={15}>
                <LoginRegister />
            </Col>
            <Col xs={24} md={9}>
                <LoginComponent />
            </Col>
        </Row>
    );
}
export default Login;