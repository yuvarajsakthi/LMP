import React from 'react';
import { Form, Input, Button, Typography, Card, Row, Col } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;

const FamilyEmergencyDetails: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  const handleNext = (values: any) => {
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'familyEmergencyDetails', data: values } });
    dispatch({ type: 'NEXT_STEP' });
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Family & Emergency Details</Title>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleNext}
        initialValues={state.formData.familyEmergencyDetails}
      >
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item
              name="emergencyContactName"
              label="Emergency Contact Name"
              rules={[{ required: true, message: 'Please enter emergency contact name' }]}
            >
              <Input />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item
              name="emergencyContactPhone"
              label="Emergency Contact Phone"
              rules={[{ required: true, message: 'Please enter emergency contact phone' }]}
            >
              <Input />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item
          name="relationship"
          label="Relationship"
          rules={[{ required: true, message: 'Please enter relationship' }]}
        >
          <Input />
        </Form.Item>

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

export default FamilyEmergencyDetails;