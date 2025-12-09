import React from 'react';
import { Form, Input, InputNumber, Button, Select, Row, Col, Card } from 'antd';
import { PropertyType, OwnershipType, LoanPurposeHome } from '../../../types/homeLoan';
import { useLoanApplication } from '../../../context';
import styles from './HomeLoanDetails.module.css';

interface HomeLoanDetailsProps {
  onNext: () => void;
  onPrevious?: () => void;
}

const HomeLoanDetails: React.FC<HomeLoanDetailsProps> = ({ onNext, onPrevious }) => {
  const [form] = Form.useForm();
  const { state, dispatch } = useLoanApplication();

  React.useEffect(() => {
    if (state.formData.loanDetails) {
      form.setFieldsValue(state.formData.loanDetails);
    }
  }, []);

  const handleSubmit = (values: any) => {
    const loanData = {
      requestedLoanAmount: values.requestedLoanAmount,
      tenureMonths: values.tenureMonths,
      propertyType: values.propertyType,
      propertyAddress: values.propertyAddress,
      city: values.city,
      zipCode: values.zipCode,
      ownershipType: values.ownershipType,
      propertyCost: values.propertyCost,
      downPayment: values.downPayment,
      loanPurpose: values.loanPurpose
    };
    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: { section: 'loanDetails', data: loanData }
    });
    dispatch({ type: 'SET_LOAN_TYPE', payload: 'home' });
    onNext();
  };

  return (
    <Card className={styles.card}>
      <div className={styles.header}>
        <h2 className={styles.title}>Home Loan Details</h2>
        <p className={styles.subtitle}>Enter your home loan and property details</p>
      </div>
      <Form form={form} layout="vertical" onFinish={handleSubmit} className={styles.form}>
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

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="propertyType" label="Property Type" rules={[{ required: true }]}>
              <Select placeholder="Select property type">
                <Select.Option value={PropertyType.Residential}>Residential</Select.Option>
                <Select.Option value={PropertyType.Commercial}>Commercial</Select.Option>
                <Select.Option value={PropertyType.Industrial}>Industrial</Select.Option>
                <Select.Option value={PropertyType.SpecialPurpose}>Special Purpose</Select.Option>
              </Select>
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="ownershipType" label="Ownership Type" rules={[{ required: true }]}>
              <Select placeholder="Select ownership type">
                <Select.Option value={OwnershipType.Owned}>Owned</Select.Option>
                <Select.Option value={OwnershipType.Builder}>Builder</Select.Option>
              </Select>
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="propertyAddress" label="Property Address" rules={[{ required: true }]}>
          <Input.TextArea rows={2} placeholder="Enter property address" />
        </Form.Item>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="city" label="City" rules={[{ required: true }]}>
              <Input placeholder="Enter city" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="zipCode" label="Zip Code" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} placeholder="Enter zip code" />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={12}>
            <Form.Item name="propertyCost" label="Property Cost (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter property cost" />
            </Form.Item>
          </Col>
          <Col span={12}>
            <Form.Item name="downPayment" label="Down Payment (₹)" rules={[{ required: true }]}>
              <InputNumber style={{ width: '100%' }} min={0} placeholder="Enter down payment" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item name="loanPurpose" label="Loan Purpose" rules={[{ required: true }]}>
          <Select placeholder="Select loan purpose">
            <Select.Option value={LoanPurposeHome.Purchase}>Purchase</Select.Option>
            <Select.Option value={LoanPurposeHome.Construction}>Construction</Select.Option>
            <Select.Option value={LoanPurposeHome.Renovation}>Renovation</Select.Option>
          </Select>
        </Form.Item>

        <Form.Item className={styles.buttonGroup}>
          <div className={styles.buttons}>
            {onPrevious && <Button onClick={onPrevious} size="large" className={styles.backButton}>BACK</Button>}
            <Button type="primary" htmlType="submit" size="large" className={styles.nextButton}>NEXT</Button>
          </div>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default HomeLoanDetails;
