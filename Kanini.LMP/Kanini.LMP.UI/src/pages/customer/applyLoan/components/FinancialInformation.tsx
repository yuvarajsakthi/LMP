import React from 'react';
import { Form, InputNumber, Button, Typography, Card, Row, Col } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;

const FinancialInformation: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  const handleNext = (values: any) => {
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'financialInformation', data: values } });
    dispatch({ type: 'NEXT_STEP' });
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Financial Information</Title>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleNext}
        initialValues={state.formData.financialInformation}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="existingEMI"
              label="Existing EMI (₹)"
              rules={[{ required: true, message: 'Please enter existing EMI' }]}
            >
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="otherIncome"
              label="Other Income (₹)"
            >
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="bankBalance"
              label="Bank Balance (₹)"
              rules={[{ required: true, message: 'Please enter bank balance' }]}
            >
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="creditScore"
              label="Credit Score (if known)"
            >
              <InputNumber min={300} max={900} style={{ width: '100%' }} />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item>
          <Button onClick={handlePrev} style={{ marginRight: 8 }}>
            Previous
          </Button>
          <Button type="primary" htmlType="submit">
            Next
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default FinancialInformation;