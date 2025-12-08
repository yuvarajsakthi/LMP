import React from 'react';
import { Card, Progress, Typography } from 'antd';
import { CheckCircleOutlined, ClockCircleOutlined } from '@ant-design/icons';
import styles from './ApplicationSummary.module.css';

const { Text } = Typography;

interface ApplicationSummaryProps {
  currentStep: number;
  totalSteps: number;
  loanAmount?: number;
  tenure?: number;
  documentsUploaded?: boolean;
}

const ApplicationSummary: React.FC<ApplicationSummaryProps> = ({
  currentStep,
  totalSteps,
  loanAmount,
  tenure,
  documentsUploaded
}) => {
  const progress = Math.round(((currentStep + 1) / totalSteps) * 100);

  return (
    <Card title="Summary" className={styles.summaryCard}>
      <div className={styles.summaryContent}>
        <div className={styles.progressSection}>
          <Text className={styles.progressLabel}>Your loan application progress will be shown here.</Text>
          <Progress 
            percent={progress} 
            status="active"
            strokeColor="#1890ff"
          />
          <Text className={styles.stepText}>
            Step {currentStep + 1} of {totalSteps}
          </Text>
        </div>

        <div className={styles.detailsSection}>
          <div className={styles.detailItem}>
            <Text className={styles.detailLabel}>Loan Amount:</Text>
            <Text className={styles.detailValue}>
              {loanAmount ? `₹ ${loanAmount.toLocaleString()}` : '-'}
            </Text>
          </div>

          <div className={styles.detailItem}>
            <Text className={styles.detailLabel}>Tenure:</Text>
            <Text className={styles.detailValue}>
              {tenure ? `${tenure} months` : '-'}
            </Text>
          </div>

          <div className={styles.detailItem}>
            <Text className={styles.detailLabel}>Document uploaded:</Text>
            <Text className={styles.detailValue}>
              {documentsUploaded ? (
                <span className={styles.success}>
                  <CheckCircleOutlined /> Successfully
                </span>
              ) : (
                <span className={styles.pending}>
                  <ClockCircleOutlined /> Pending
                </span>
              )}
            </Text>
          </div>
        </div>
      </div>
    </Card>
  );
};

export default ApplicationSummary;
