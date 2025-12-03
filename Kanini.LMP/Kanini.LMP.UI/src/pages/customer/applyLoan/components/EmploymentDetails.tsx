import React from 'react';
import { Form, Input, Select, InputNumber, Button, Typography, Card, Row, Col } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;
const { Option } = Select;

const EmploymentDetails: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  const handleNext = (values: any) => {
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'employmentDetails', data: values } });
    dispatch({ type: 'NEXT_STEP' });
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Employment Details</Title>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleNext}
        initialValues={state.formData.employmentDetails}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="employmentType"
              label="Employment Type"
              rules={[{ required: true, message: 'Please select employment type' }]}
            >
              <Select>
                <Option value="Salaried">Salaried</Option>
                <Option value="Self-Employed">Self-Employed</Option>
                <Option value="Business">Business</Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="companyName"
              label="Company Name"
              rules={[{ required: true, message: 'Please enter company name' }]}
            >
              <Input />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="monthlyIncome"
              label="Monthly Income (₹)"
              rules={[{ required: true, message: 'Please enter monthly income' }]}
            >
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="workExperience"
              label="Work Experience (Years)"
              rules={[{ required: true, message: 'Please enter work experience' }]}
            >
              <InputNumber min={0} style={{ width: '100%' }} />
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

export default EmploymentDetails;