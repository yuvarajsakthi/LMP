import React from 'react';
import { Form, Input, InputNumber, Button, Select, Row, Col, Card } from 'antd';
import { VehicleType, LoanPurposeVehicle } from '../../../types/vehicleLoan';
import { useLoanApplication } from '../../../context';

interface VehicleLoanDetailsProps {
  onNext: () => void;
}

const VehicleLoanDetails: React.FC<VehicleLoanDetailsProps> = ({ onNext }) => {
  const [form] = Form.useForm();
  const { dispatch } = useLoanApplication();

  const handleSubmit = (values: any) => {
    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: { section: 'loanDetails', data: values }
    });
    dispatch({ type: 'SET_LOAN_TYPE', payload: 'vehicle' });
    onNext();
  };

  return (
    <Card>
      <h2>Vehicle Loan Details</h2>
      <p style={{ marginBottom: 24, color: '#666' }}>Enter your vehicle loan details</p>
      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="requestedLoanAmount" label="Requested Loan Amount (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter amount" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="tenureMonths" label="Tenure (Months)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={1} max={84} placeholder="Enter tenure" />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="vehicleType" label="Vehicle Type" rules={[{ required: true }]}>
              <Select placeholder="Select vehicle type">
                <Select.Option value={VehicleType.Car}>Car</Select.Option>
                <Select.Option value={VehicleType.Bike}>Bike</Select.Option>
                <Select.Option value={VehicleType.Scooter}>Scooter</Select.Option>
                <Select.Option value={VehicleType.Motorcycle}>Motorcycle</Select.Option>
                <Select.Option value={VehicleType.SUV}>SUV</Select.Option>
                <Select.Option value={VehicleType.Truck}>Truck</Select.Option>
                <Select.Option value={VehicleType.Bus}>Bus</Select.Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="loanPurposeVehicle" label="Loan Purpose" rules={[{ required: true }]}>
              <Select placeholder="Select loan purpose">
                <Select.Option value={LoanPurposeVehicle.New}>New Vehicle</Select.Option>
                <Select.Option value={LoanPurposeVehicle.Used}>Used Vehicle</Select.Option>
              </Select>
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="manufacturer" label="Manufacturer" rules={[{ required: true }]}>
              <Input placeholder="Enter manufacturer" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="model" label="Model" rules={[{ required: true }]}>
              <Input placeholder="Enter model" />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="manufacturingYear" label="Manufacturing Year" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={1900} max={2100} placeholder="Enter year" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="onRoadPrice" label="On-Road Price (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter price" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="downPayment" label="Down Payment (₹)" rules={[{ required: true }]}>
          <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter down payment" />
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" block>Next</Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default VehicleLoanDetails;
