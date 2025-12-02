import React, { useState } from 'react';
import { Modal, Form, Input, Select, Button, Radio, InputNumber, Divider, Card, Progress, Tag } from 'antd';
import { CheckCircleOutlined, CloseCircleOutlined, BankOutlined } from '@ant-design/icons';
import { motion } from 'framer-motion';
import { useApi } from '../../hooks';
import axiosInstance from '../../services/api/axiosInstance';
import styles from './EligibilityModal.module.css';

interface EligibilityModalProps {
  visible: boolean;
  onClose: () => void;
}

const EligibilityModal: React.FC<EligibilityModalProps> = ({ visible, onClose }) => {
  console.log('EligibilityModal rendered, visible:', visible);
  const [form] = Form.useForm();
  const [isExisting, setIsExisting] = useState(false);
  const [result, setResult] = useState<any>(null);
  const { loading, execute } = useApi();

  const handleSubmit = async (values: any) => {
    const request = {
      isExistingBorrower: isExisting,
      ...values
    };

    const response = await execute(() => 
      axiosInstance.post('/api/Eligibility/check', request)
    );

    if (response) {
      setResult(response);
    }
  };

  const resetForm = () => {
    form.resetFields();
    setResult(null);
    setIsExisting(false);
  };

  const handleClose = () => {
    resetForm();
    onClose();
  };

  if (result) {
    return (
      <Modal
        title="Eligibility Results"
        open={visible}
        onCancel={handleClose}
        footer={[
          <Button key="close" onClick={handleClose}>Close</Button>,
          <Button key="check-again" onClick={() => setResult(null)}>Check Again</Button>
        ]}
        width={800}
      >
        <div className={styles.resultContainer}>
          <Card className={styles.scoreCard}>
            <div className={styles.scoreHeader}>
              <h3>Your Eligibility Score</h3>
              <div className={styles.scoreDisplay}>
                <Progress
                  type="circle"
                  percent={Math.round((result.eligibilityScore / 850) * 100)}
                  format={() => result.eligibilityScore}
                  strokeColor={result.eligibilityScore >= 650 ? '#52c41a' : result.eligibilityScore >= 550 ? '#faad14' : '#ff4d4f'}
                />
              </div>
              <p className={styles.creditScore}>
                Credit Score: {result.creditScore.score} ({result.creditScore.range})
              </p>
            </div>
            <p className={styles.message}>{result.message}</p>
          </Card>

          <Card title="Available Loan Products" className={styles.productsCard}>
            {result.products.map((product: any) => (
              <div key={product.productId} className={styles.productItem}>
                <div className={styles.productInfo}>
                  <span className={styles.productName}>{product.productName}</span>
                  <span className={styles.productDetails}>
                    Max: {product.maxAmount} | Rate: {product.interestRate}
                  </span>
                </div>
                <Tag color={product.available ? 'green' : 'red'}>
                  {product.available ? <CheckCircleOutlined /> : <CloseCircleOutlined />}
                  {product.available ? 'Available' : `Min Score: ${product.minScore}`}
                </Tag>
              </div>
            ))}
          </Card>

          {result.improvementTips.length > 0 && (
            <Card title="Improvement Tips" className={styles.tipsCard}>
              <ul>
                {result.improvementTips.map((tip: string, index: number) => (
                  <li key={index}>{tip}</li>
                ))}
              </ul>
            </Card>
          )}
        </div>
      </Modal>
    );
  }

  return (
    <Modal
      title={
        <div className={styles.modalTitle}>
          <BankOutlined className={styles.titleIcon} />
          Check Your Loan Eligibility
        </div>
      }
      open={visible}
      onCancel={handleClose}
      footer={null}
      width={700}
      className={styles.eligibilityModal}
      centered
    >
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.3 }}
      >
        <Form form={form} onFinish={handleSubmit} layout="vertical" className={styles.eligibilityForm}>
        <Form.Item label="Are you an existing borrower?">
          <Radio.Group value={isExisting} onChange={(e) => setIsExisting(e.target.value)}>
            <Radio value={false}>New Customer</Radio>
            <Radio value={true}>Existing Borrower</Radio>
          </Radio.Group>
        </Form.Item>

        <Divider />

        {!isExisting && (
          <>
            <Form.Item
              name="pan"
              label="PAN Number"
              rules={[
                { required: true, message: 'PAN is required' },
                { pattern: /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/, message: 'Invalid PAN format (e.g., ABCDE1234F)' }
              ]}
            >
              <Input placeholder="ABCDE1234F" />
            </Form.Item>

            <Form.Item
              name="age"
              label="Age"
              rules={[
                { required: true, message: 'Age is required' },
                { type: 'number', min: 18, max: 80, message: 'Age must be between 18-80' }
              ]}
            >
              <InputNumber min={18} max={80} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item
              name="annualIncome"
              label="Annual Income (₹)"
              rules={[
                { required: true, message: 'Annual income is required' },
                { type: 'number', min: 100000, max: 50000000, message: 'Income must be between ₹1L - ₹5Cr' }
              ]}
            >
              <InputNumber min={100000} max={50000000} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item
              name="occupation"
              label="Occupation"
              rules={[{ required: true, message: 'Occupation is required' }]}
            >
              <Select placeholder="Select occupation">
                <Select.Option value="Government">Government</Select.Option>
                <Select.Option value="IT">IT/Software</Select.Option>
                <Select.Option value="Banking">Banking/Finance</Select.Option>
                <Select.Option value="Healthcare">Healthcare</Select.Option>
                <Select.Option value="Education">Education</Select.Option>
                <Select.Option value="Business">Business</Select.Option>
                <Select.Option value="Other">Other</Select.Option>
              </Select>
            </Form.Item>

            <Form.Item
              name="homeOwnershipStatus"
              label="Home Ownership"
              rules={[{ required: true, message: 'Home ownership status is required' }]}
            >
              <Select placeholder="Select home ownership">
                <Select.Option value={0}>Rented</Select.Option>
                <Select.Option value={1}>Owned</Select.Option>
                <Select.Option value={2}>Mortgage</Select.Option>
              </Select>
            </Form.Item>
          </>
        )}

        {isExisting && (
          <>
            <Form.Item name="experienceYears" label="Work Experience (Years)">
              <InputNumber min={0} max={50} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="employerName" label="Current Employer">
              <Input placeholder="Company name" />
            </Form.Item>

            <Form.Item name="monthlyEMI" label="Current Monthly EMI (₹)">
              <InputNumber min={0} max={100000} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="existingLoanAmount" label="Existing Loan Amount (₹)">
              <InputNumber min={0} max={10000000} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="previousLoanCount" label="Previous Loan Count">
              <InputNumber min={0} max={20} style={{ width: '100%' }} />
            </Form.Item>

            <Divider>Payment History</Divider>

            <Form.Item name="onTimePayments" label="On-time Payments">
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="latePayments" label="Late Payments">
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="missedPayments" label="Missed Payments">
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="hasDefaultHistory" label="Any Default History?">
              <Radio.Group>
                <Radio value={false}>No</Radio>
                <Radio value={true}>Yes</Radio>
              </Radio.Group>
            </Form.Item>

            <Form.Item name="daysOverdueMax" label="Maximum Days Overdue">
              <InputNumber min={0} style={{ width: '100%' }} />
            </Form.Item>
          </>
        )}

        <Form.Item className={styles.submitButton}>
          <Button type="primary" htmlType="submit" loading={loading} size="large" className={styles.checkButton}>
            Check Eligibility
          </Button>
        </Form.Item>
        </Form>
      </motion.div>
    </Modal>
  );
};

export default EligibilityModal;