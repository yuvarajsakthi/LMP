import React from 'react';
import { Form, Input, InputNumber, Select, Button } from 'antd';
import { useAppDispatch } from '../../../../hooks';

interface HomeLoanDetailsProps {
  onNext: () => void;
}

const HomeLoanDetails: React.FC<HomeLoanDetailsProps> = ({ onNext }) => {
  const [form] = Form.useForm();
  const dispatch = useAppDispatch();

  const handleSubmit = async (values: any) => {
    onNext();
  };

  return (
    <Form form={form} layout="vertical" onFinish={handleSubmit}>
      <Form.Item
        name="requestedAmount"
        label="Requested Amount"
        rules={[{ required: true, message: 'Please enter requested amount' }]}
      >
        <InputNumber min={1} style={{ width: '100%' }} prefix="₹" />
      </Form.Item>

      <Form.Item
        name="tenureMonths"
        label="Tenure (Months)"
        rules={[{ required: true, message: 'Please enter tenure' }]}
      >
        <InputNumber min={1} max={360} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        name="propertyType"
        label="Property Type"
        rules={[{ required: true, message: 'Please select property type' }]}
      >
        <Select>
          <Select.Option value={0}>Residential</Select.Option>
          <Select.Option value={1}>Commercial</Select.Option>
          <Select.Option value={2}>Industrial</Select.Option>
          <Select.Option value={3}>Special Purpose</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="propertyAddress"
        label="Property Address"
        rules={[{ required: true, message: 'Please enter property address' }]}
      >
        <Input maxLength={250} />
      </Form.Item>

      <Form.Item
        name="city"
        label="City"
        rules={[{ required: true, message: 'Please enter city' }]}
      >
        <Input maxLength={100} />
      </Form.Item>

      <Form.Item
        name="zipCode"
        label="Zip Code"
        rules={[{ required: true, message: 'Please enter zip code' }]}
      >
        <InputNumber min={100000} max={999999} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        name="ownershipType"
        label="Ownership Type"
        rules={[{ required: true, message: 'Please select ownership type' }]}
      >
        <Select>
          <Select.Option value={0}>Owned</Select.Option>
          <Select.Option value={1}>Builder</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="propertyCost"
        label="Property Cost"
        rules={[{ required: true, message: 'Please enter property cost' }]}
      >
        <InputNumber min={0} style={{ width: '100%' }} prefix="₹" />
      </Form.Item>

      <Form.Item
        name="downPayment"
        label="Down Payment"
        rules={[{ required: true, message: 'Please enter down payment' }]}
      >
        <InputNumber min={0} style={{ width: '100%' }} prefix="₹" />
      </Form.Item>

      <Form.Item
        name="loanPurpose"
        label="Loan Purpose"
        rules={[{ required: true, message: 'Please select loan purpose' }]}
      >
        <Select>
          <Select.Option value={0}>Purchase</Select.Option>
          <Select.Option value={1}>Construction</Select.Option>
          <Select.Option value={2}>Renovation</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item>
        <Button type="primary" htmlType="submit">
          Next
        </Button>
      </Form.Item>
    </Form>
  );
};

export default HomeLoanDetails;
