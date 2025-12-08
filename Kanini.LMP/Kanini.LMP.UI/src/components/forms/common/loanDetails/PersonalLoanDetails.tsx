import React from 'react';
import { Form, InputNumber, Select, Button } from 'antd';
import { useAppDispatch } from '../../../../hooks';

interface PersonalLoanDetailsProps {
  onNext: () => void;
}

const PersonalLoanDetails: React.FC<PersonalLoanDetailsProps> = ({ onNext }) => {
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
        name="employmentType"
        label="Employment Type"
        rules={[{ required: true, message: 'Please select employment type' }]}
      >
        <Select>
          <Select.Option value={0}>Salaried</Select.Option>
          <Select.Option value={1}>Self Employed</Select.Option>
          <Select.Option value={2}>Business</Select.Option>
          <Select.Option value={3}>Professional</Select.Option>
        </Select>
      </Form.Item>

      <Form.Item
        name="monthlyIncome"
        label="Monthly Income"
        rules={[{ required: true, message: 'Please enter monthly income' }]}
      >
        <InputNumber min={0} style={{ width: '100%' }} prefix="₹" />
      </Form.Item>

      <Form.Item
        name="workExperienceYears"
        label="Work Experience (Years)"
        rules={[{ required: true, message: 'Please enter work experience' }]}
      >
        <InputNumber min={0} style={{ width: '100%' }} />
      </Form.Item>

      <Form.Item
        name="loanPurpose"
        label="Loan Purpose"
        rules={[{ required: true, message: 'Please select loan purpose' }]}
      >
        <Select>
          <Select.Option value={0}>Medical</Select.Option>
          <Select.Option value={1}>Education</Select.Option>
          <Select.Option value={2}>Travel</Select.Option>
          <Select.Option value={3}>Wedding</Select.Option>
          <Select.Option value={4}>Debt Consolidation</Select.Option>
          <Select.Option value={5}>Home Renovation</Select.Option>
          <Select.Option value={6}>Other</Select.Option>
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

export default PersonalLoanDetails;
