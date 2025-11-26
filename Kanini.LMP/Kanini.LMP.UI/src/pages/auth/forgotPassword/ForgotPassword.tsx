import { Row, Col } from 'antd';
import { ForgotPasswordComponent, LoginRegister } from '../../../components';

const ForgotPassword = () => {
    return (

        <Row gutter={24} id="container">
            <Col xs={24} md={15}>
                <LoginRegister />
            </Col>
            <Col xs={24} md={9}>
                <ForgotPasswordComponent />
            </Col>
        </Row>
    );
}
export default ForgotPassword;