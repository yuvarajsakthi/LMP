import React, { useState, useEffect } from 'react';
import { Modal, Form, Input, Select, Button, Radio, InputNumber, Divider, Card, Progress, Tag } from 'antd';
import { CheckCircleOutlined, CloseCircleOutlined, BankOutlined } from '@ant-design/icons';
import { motion } from 'framer-motion';
import { useAppDispatch, useAppSelector } from '../../hooks';
import { checkEligibility } from '../../store/slices/eligibilitySlice';
import { fetchCustomerByUserId } from '../../store/slices/customerSlice';
import { useAuth } from '../../context/AuthContext';
import styles from './EligibilityModal.module.css';

interface EligibilityModalProps {
  visible: boolean;
  onClose: () => void;
}

const EligibilityModal: React.FC<EligibilityModalProps> = ({ visible, onClose }) => {
  const [form] = Form.useForm();
  const [isExisting, setIsExisting] = useState(false);
  const dispatch = useAppDispatch();
  const { checkResult, loading } = useAppSelector((state) => state.eligibility);
  const { currentCustomer } = useAppSelector((state) => state.customer);
  const { token } = useAuth();

  useEffect(() => {
    const userId = token?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    if (visible && userId) {
      if (!currentCustomer) {
        dispatch(fetchCustomerByUserId(Number(userId)));
      } else if (isExisting) {
        form.setFieldsValue({
          pan: currentCustomer.panNumber,
          age: currentCustomer.age,
          annualIncome: currentCustomer.annualIncome,
          occupation: currentCustomer.occupation,
          homeOwnershipStatus: currentCustomer.homeOwnershipStatus
        });
      }
    }
  }, [visible, token, currentCustomer, dispatch, isExisting, form]);

  useEffect(() => {
    if (isExisting && currentCustomer) {
      form.setFieldsValue({
        pan: currentCustomer.panNumber,
        age: currentCustomer.age,
        annualIncome: currentCustomer.annualIncome,
        occupation: currentCustomer.occupation,
        homeOwnershipStatus: currentCustomer.homeOwnershipStatus
      });
    }
  }, [isExisting, currentCustomer, form]);



  const handleSubmit = async (values: any) => {
    const request = {
      isExistingBorrower: isExisting,
      ...values
    };

    await dispatch(checkEligibility(request));
  };

  const resetForm = () => {
    form.resetFields();
    setIsExisting(false);
  };

  useEffect(() => {
    if (!visible) {
      setIsExisting(false);
    }
  }, [visible]);

  const handleClose = () => {
    resetForm();
    onClose();
  };

  if (checkResult) {
    return (
      <Modal
        title="Eligibility Results"
        open={visible}
        onCancel={handleClose}
        footer={[
          <Button key="close" onClick={handleClose}>Close</Button>,
          <Button key="check-again" onClick={resetForm}>Check Again</Button>
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
                  percent={Math.round((checkResult.eligibilityScore / 850) * 100)}
                  format={() => checkResult.eligibilityScore}
                  strokeColor={checkResult.eligibilityScore >= 650 ? '#52c41a' : checkResult.eligibilityScore >= 550 ? '#faad14' : '#ff4d4f'}
                />
              </div>
              <p className={styles.creditScore}>
                Credit Score: {checkResult.creditScore.score} ({checkResult.creditScore.range})
              </p>
            </div>
            <p className={styles.message}>{checkResult.message}</p>
          </Card>

          <Card title="Available Loan Products" className={styles.productsCard}>
            {checkResult.products.map((product: any) => (
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

          {checkResult.improvementTips.length > 0 && (
            <Card title="Improvement Tips" className={styles.tipsCard}>
              <ul>
                {checkResult.improvementTips.map((tip: string, index: number) => (
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
          <Radio.Group value={isExisting} onChange={(e) => {
            console.log('📻 Radio changed to:', e.target.value);
            setIsExisting(e.target.value);
          }}>
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
            <Form.Item name="pan" label="PAN Number">
              <Input disabled placeholder="ABCDE1234F" />
            </Form.Item>

            <Form.Item name="age" label="Age">
              <InputNumber disabled min={18} max={80} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="annualIncome" label="Annual Income (₹)">
              <InputNumber disabled min={100000} max={50000000} style={{ width: '100%' }} />
            </Form.Item>

            <Form.Item name="occupation" label="Occupation">
              <Input disabled />
            </Form.Item>

            <Form.Item name="homeOwnershipStatus" label="Home Ownership">
              <Select disabled placeholder="Select home ownership">
                <Select.Option value={0}>Rented</Select.Option>
                <Select.Option value={1}>Owned</Select.Option>
                <Select.Option value={2}>Mortgage</Select.Option>
              </Select>
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