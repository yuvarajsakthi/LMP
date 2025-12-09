import React, { useEffect } from 'react';
import { Modal, Button, Card, Progress, Tag } from 'antd';
import { CheckCircleOutlined, CloseCircleOutlined, BankOutlined } from '@ant-design/icons';
import { motion } from 'framer-motion';
import { useAppDispatch, useAppSelector } from '../../hooks';
import { calculateEligibility } from '../../store/slices/eligibilitySlice';
import { fetchCustomerByUserId } from '../../store/slices/customerSlice';
import { useAuth } from '../../context/AuthContext';
import styles from './EligibilityModal.module.css';

interface EligibilityModalProps {
  visible: boolean;
  onClose: () => void;
}

const EligibilityModal: React.FC<EligibilityModalProps> = ({ visible, onClose }) => {
  const dispatch = useAppDispatch();
  const { score, loading } = useAppSelector((state) => state.eligibility);
  const { currentCustomer } = useAppSelector((state) => state.customer);
  const { token } = useAuth();

  useEffect(() => {
    const userId = token?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    if (visible && userId && !currentCustomer) {
      dispatch(fetchCustomerByUserId(Number(userId)));
    }
  }, [visible, token, currentCustomer, dispatch]);

  const handleSubmit = async () => {
    if (!currentCustomer?.customerId) return;
    await dispatch(calculateEligibility(currentCustomer.customerId));
  };

  const handleCheckAgain = async () => {
    if (!currentCustomer?.customerId) return;
    await dispatch(calculateEligibility(currentCustomer.customerId));
  };

  const handleClose = () => {
    onClose();
  };

  if (score) {
    return (
      <Modal
        title="Eligibility Results"
        open={visible}
        onCancel={handleClose}
        footer={[
          <Button key="close" onClick={handleClose}>Close</Button>,
          <Button key="check-again" onClick={handleCheckAgain} loading={loading}>Check Again</Button>
        ]}
        width={800}
      >
        <div className={styles.resultContainer}>
          <Card className={styles.scoreCard}>
            <div className={styles.scoreHeader}>
              <h3>Your Credit Score</h3>
              <div className={styles.scoreDisplay}>
                <Progress
                  type="circle"
                  percent={Math.round(((score.creditScore?.score || 0) / 900) * 100)}
                  format={() => score.creditScore?.score || 0}
                  strokeColor={(score.creditScore?.score || 0) >= 650 ? '#52c41a' : (score.creditScore?.score || 0) >= 550 ? '#faad14' : '#ff4d4f'}
                />
              </div>
              {score.creditScore && (
                <p className={styles.creditScore}>
                  Range: {score.creditScore.range} | Eligibility: {score.status}
                </p>
              )}
            </div>
            {score.message && <p className={styles.message}>{score.message}</p>}
          </Card>

          {score.products && score.products.length > 0 && (
            <Card title="Available Loan Products" className={styles.productsCard}>
              {score.products.map((product) => (
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
          )}

          {score.improvementTips && score.improvementTips.length > 0 && (
            <Card title="Improvement Tips" className={styles.tipsCard}>
              <ul>
                {score.improvementTips.map((tip: string, index: number) => (
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
        <div style={{ padding: '24px', background: '#f0f2f5', borderRadius: '8px', textAlign: 'center', marginBottom: '24px' }}>
          <p style={{ margin: 0, color: '#666', fontSize: '16px' }}>Your profile will be used to calculate eligibility</p>
        </div>
        <Button type="primary" onClick={handleSubmit} loading={loading} size="large" block className={styles.checkButton}>
          Check Eligibility
        </Button>
      </motion.div>
    </Modal>
  );
};

export default EligibilityModal;