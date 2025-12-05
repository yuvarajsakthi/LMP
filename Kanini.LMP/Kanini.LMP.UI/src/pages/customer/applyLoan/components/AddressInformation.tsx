import React from 'react';
import { Form, Input, Select, Button, Typography, Card, Row, Col } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;
const { Option } = Select;

const AddressInformation: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm();

  const handleNext = (values: any) => {
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'addressInformation', data: values } });
    dispatch({ type: 'NEXT_STEP' });
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Address Information</Title>
      <Form
        form={form}
        layout="vertical"
        onFinish={handleNext}
        initialValues={state.formData.addressInformation}
      >
        <Form.Item
          name="addressLine1"
          label="Address Line 1"
          rules={[{ required: true, message: 'Please enter address' }]}
        >
          <Input />
        </Form.Item>

        <Form.Item name="addressLine2" label="Address Line 2">
          <Input />
        </Form.Item>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item
              name="city"
              label="City"
              rules={[{ required: true, message: 'Please enter city' }]}
            >
              <Input />
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item
              name="state"
              label="State"
              rules={[{ required: true, message: 'Please select state' }]}
            >
              <Select>
                <Option value="Karnataka">Karnataka</Option>
                <Option value="Tamil Nadu">Tamil Nadu</Option>
                <Option value="Andra Pradesh">Andra Pradesh</Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={8}>
            <Form.Item
              name="pincode"
              label="Pincode"
              rules={[{ required: true, message: 'Please enter pincode' }]}
            >
              <Input />
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

export default AddressInformation;