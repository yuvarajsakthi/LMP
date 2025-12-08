import React from 'react';
import { Form, Input, InputNumber, Select, Button } from 'antd';
import { useAppDispatch } from '../../../../hooks';

interface VehicleLoanDetailsProps {
  onNext: () => void;
}

const VehicleLoanDetails: React.FC<VehicleLoanDetailsProps> = ({ onNext }) => {
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
        name="vehicleType"
        label="Vehicle Type"
        rules={[{ required: true, message: 'Please select vehicle type' }]}
      >
        <Select>
          <Select.Option value={0}>Car</Select.Option>
          <Select.Option value={1}>Bike</Select.Option>
          <Select.Option value={2}>Truck</Select.Option>
          <Select.Option value={3}>Bus</Select.Option>
          <Select.Option value={4}>Scooter</Select.Option>
          <Select.Option value={5}>Van</Select.Option>
          <Select.Option value={6}>Tractor</Select.Option>
          <Select.Option value={12}>SUV</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="manufacturer"
        label="Manufacturer"
        rules={[{ required: true, message: 'Please enter manufacturer' }]}
      >
        <Input maxLength={100} />
      </Form.Item>

      <Form.Item
        name="model"
        label="Model"
        rules={[{ required: true, message: 'Please enter model' }]}
      >
        <Input maxLength={100} />
      </Form.Item>

      <Form.Item
        name="manufacturingYear"
        label="Manufacturing Year"
        rules={[{ required: true, message: 'Please enter year' }]}
      >
        <InputNumber min={1900} max={2100} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        name="onRoadPrice"
        label="On-Road Price"
        rules={[{ required: true, message: 'Please enter on-road price' }]}
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
        name="loanPurposeVehicle"
        label="Loan Purpose"
        rules={[{ required: true, message: 'Please select loan purpose' }]}
      >
        <Select>
          <Select.Option value={0}>New</Select.Option>
          <Select.Option value={1}>Used</Select.Option>
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

export default VehicleLoanDetails;
