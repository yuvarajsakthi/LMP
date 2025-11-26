import { Row, Col } from 'antd';
import { LoginRegister, RegisterComponent } from '../../../components';

const Register = () => {
    return (

        <Row gutter={24} id="container">
            <Col xs={24} md={15}>
                <LoginRegister />
            </Col>
            <Col xs={24} md={9}>
                <RegisterComponent />
            </Col>
        </Row>
    );
}

export default Register;