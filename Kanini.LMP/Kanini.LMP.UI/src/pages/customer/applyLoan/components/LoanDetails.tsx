import React from 'react';
import { Form, Input, InputNumber, Button, Typography, Card } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;

const LoanDetails: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  const handleNext = (values: any) => {
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'loanDetails', data: values } });
    dispatch({ type: 'NEXT_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Loan Details</Title>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleNext}
        initialValues={state.formData.loanDetails}
      >
        <Form.Item
          name="requestedAmount"
          label="Requested Amount (₹)"
          rules={[{ required: true, message: 'Please enter loan amount' }]}
        >
          <InputNumber
            min={10000}
            max={5000000}
            style={{ width: '100%' }}
            formatter={(value) => `₹ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
            parser={(value) => value!.replace(/₹\s?|(,*)/g, '')}
          />
        </Form.Item>

        <Form.Item
          name="tenureMonths"
          label="Loan Tenure (Months)"
          rules={[{ required: true, message: 'Please enter tenure' }]}
        >
          <InputNumber min={6} max={84} style={{ width: '100%' }} />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" size="large">
            Next
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default LoanDetails;