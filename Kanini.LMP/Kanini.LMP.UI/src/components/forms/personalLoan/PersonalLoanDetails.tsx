import React from 'react';
import { Form, InputNumber, Button, Select, Row, Col, Card, message } from 'antd';
import { EmploymentType, LoanPurposePersonal } from '../../../types/personalLoan';
import { useLoanApplication } from '../../../context';
import styles from './PersonalLoanDetails.module.css';

interface PersonalLoanDetailsProps {
  onNext: () => void;
  onPrevious?: () => void;
}

const PersonalLoanDetails: React.FC<PersonalLoanDetailsProps> = ({ onNext, onPrevious }) => {
  const [form] = Form.useForm();
  const { state, dispatch } = useLoanApplication();

  React.useEffect(() => {
    if (state.formData.loanDetails) {
      form.setFieldsValue(state.formData.loanDetails);
    }
  }, [state.formData.loanDetails, form]);

  const handleSubmit = (values: any) => {
    if (!values.requestedLoanAmount || values.requestedLoanAmount <= 0) {
      message.error('Please enter a valid loan amount');
      return;
    }
    if (!values.tenureMonths || values.tenureMonths <= 0) {
      message.error('Please enter a valid tenure');
      return;
    }
    if (!values.monthlyIncome || values.monthlyIncome <= 0) {
      message.error('Please enter a valid monthly income');
      return;
    }

    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: { section: 'loanDetails', data: values }
    });
    dispatch({ type: 'SET_LOAN_TYPE', payload: 'personal' });
    onNext();
  };

  return (
    <Card className={styles.card}>
      <div className={styles.header}>
        <h2 className={styles.title}>Personal Loan Details</h2>
        <p className={styles.subtitle}>Enter your loan and employment details</p>
      </div>
      
      <Form form={form} layout="vertical" onFinish={handleSubmit} className={styles.form}>
        <Row gutter={16}>
          <Col xs={24} sm={12}>
            <Form.Item 
              name="requestedLoanAmount" 
              label="Requested Loan Amount (₹)" 
              rules={[
                { required: true, message: 'Please enter loan amount' },
                { type: 'number', min: 10000, message: 'Minimum amount is ₹10,000' }
              ]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                min={10000} 
                max={10000000}
                placeholder="Enter amount" 
                formatter={value => `₹ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                parser={value => value?.replace(/₹\s?|(,*)/g, '') as any}
              />
            </Form.Item>
          </Col>
          <Col xs={24} sm={12}>
            <Form.Item 
              name="tenureMonths" 
              label="Tenure (Months)" 
              rules={[
                { required: true, message: 'Please enter tenure' },
                { type: 'number', min: 6, max: 360, message: 'Tenure must be between 6-360 months' }
              ]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                min={6} 
                max={360} 
                placeholder="Enter tenure in months" 
              />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item 
          name="employmentType" 
          label="Employment Type" 
          rules={[{ required: true, message: 'Please select employment type' }]}
        >
          <Select placeholder="Select employment type" size="large">
            <Select.Option value={EmploymentType.Salaried}>Salaried</Select.Option>
            <Select.Option value={EmploymentType.SelfEmployed}>Self Employed</Select.Option>
            <Select.Option value={EmploymentType.Business}>Business</Select.Option>
            <Select.Option value={EmploymentType.Professional}>Professional</Select.Option>
          </Select>
        </Form.Item>

        <Row gutter={16}>
          <Col xs={24} sm={12}>
            <Form.Item 
              name="monthlyIncome" 
              label="Monthly Income (₹)" 
              rules={[
                { required: true, message: 'Please enter monthly income' },
                { type: 'number', min: 10000, message: 'Minimum income is ₹10,000' }
              ]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                min={10000} 
                placeholder="Enter monthly income" 
                formatter={value => `₹ ${value}`.replace(/\B(?=(\d{3})+(?!\d))/g, ',')}
                parser={value => value?.replace(/₹\s?|(,*)/g, '') as any}
              />
            </Form.Item>
          </Col>
          <Col xs={24} sm={12}>
            <Form.Item 
              name="workExperienceYears" 
              label="Work Experience (Years)" 
              rules={[
                { required: true, message: 'Please enter work experience' },
                { type: 'number', min: 0, max: 50, message: 'Experience must be between 0-50 years' }
              ]}
            >
              <InputNumber 
                style={{ width: '100%' }} 
                min={0} 
                max={50}
                placeholder="Enter years of experience" 
              />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item 
          name="loanPurpose" 
          label="Loan Purpose" 
          rules={[{ required: true, message: 'Please select loan purpose' }]}
        >
          <Select placeholder="Select loan purpose" size="large">
            <Select.Option value={LoanPurposePersonal.Medical}>Medical</Select.Option>
            <Select.Option value={LoanPurposePersonal.Education}>Education</Select.Option>
            <Select.Option value={LoanPurposePersonal.Travel}>Travel</Select.Option>
            <Select.Option value={LoanPurposePersonal.Wedding}>Wedding</Select.Option>
            <Select.Option value={LoanPurposePersonal.DebtConsolidation}>Debt Consolidation</Select.Option>
            <Select.Option value={LoanPurposePersonal.HomeRenovation}>Home Renovation</Select.Option>
            <Select.Option value={LoanPurposePersonal.Other}>Other</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item className={styles.buttonGroup}>
          <div className={styles.buttons}>
            {onPrevious && (
              <Button onClick={onPrevious} size="large" className={styles.backButton}>
                BACK
              </Button>
            )}
            <Button type="primary" htmlType="submit" size="large" className={styles.nextButton}>
              NEXT
            </Button>
          </div>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default PersonalLoanDetails;
