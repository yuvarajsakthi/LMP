import React from 'react';
import { Form, InputNumber, Button, Select, Row, Col, Card } from 'antd';
import { EmploymentType, LoanPurposePersonal } from '../../../types/personalLoan';
import { useLoanApplication } from '../../../context';

interface PersonalLoanDetailsProps {
  onNext: () => void;
}

const PersonalLoanDetails: React.FC<PersonalLoanDetailsProps> = ({ onNext }) => {
  const [form] = Form.useForm();
  const { dispatch } = useLoanApplication();

  const handleSubmit = (values: any) => {
    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: { section: 'loanDetails', data: values }
    });
    dispatch({ type: 'SET_LOAN_TYPE', payload: 'personal' });
    onNext();
  };

  return (
    <Card>
      <h2>Personal Loan Details</h2>
      <p style={{ marginBottom: 24, color: '#666' }}>Enter your loan and employment details</p>
      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="requestedLoanAmount" label="Requested Loan Amount (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter amount" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="tenureMonths" label="Tenure (Months)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={1} max={360} placeholder="Enter tenure" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="employmentType" label="Employment Type" rules={[{ required: true }]}>
          <Select placeholder="Select employment type">
            <Select.Option value={EmploymentType.Salaried}>Salaried</Select.Option>
            <Select.Option value={EmploymentType.SelfEmployed}>Self Employed</Select.Option>
            <Select.Option value={EmploymentType.Business}>Business</Select.Option>
            <Select.Option value={EmploymentType.Professional}>Professional</Select.Option>
          </Select>
        </Form.Item>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="monthlyIncome" label="Monthly Income (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter monthly income" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="workExperienceYears" label="Work Experience (Years)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter experience" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="loanPurpose" label="Loan Purpose" rules={[{ required: true }]}>
          <Select placeholder="Select loan purpose">
            <Select.Option value={LoanPurposePersonal.Medical}>Medical</Select.Option>
            <Select.Option value={LoanPurposePersonal.Education}>Education</Select.Option>
            <Select.Option value={LoanPurposePersonal.Travel}>Travel</Select.Option>
            <Select.Option value={LoanPurposePersonal.Wedding}>Wedding</Select.Option>
            <Select.Option value={LoanPurposePersonal.DebtConsolidation}>Debt Consolidation</Select.Option>
            <Select.Option value={LoanPurposePersonal.HomeRenovation}>Home Renovation</Select.Option>
            <Select.Option value={LoanPurposePersonal.Other}>Other</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item>
          <Button type="primary" htmlType="submit" block>Next</Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default PersonalLoanDetails;
