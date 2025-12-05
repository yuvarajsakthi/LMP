import React from 'react';
import { Card, Typography, Divider, Progress } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import { useLoanApplication } from '../context/LoanApplicationContext';
import styles from '../ApplyLoan.module.css';

const { Text } = Typography;

const Summary: React.FC = () => {
    const { state } = useLoanApplication();
    const { formData, currentStep } = state;
    const totalSteps = 6;
    const progress = Math.round(((currentStep + 1) / totalSteps) * 100);

    return (
        <Card className={styles.summaryCard} title="Application Summary" variant="borderless">
            <div style={{ marginBottom: '16px' }}>
                <Text type="secondary" style={{ fontSize: '14px', display: 'block', marginBottom: '8px' }}>
                    Your loan application progress will be shown here.
                </Text>
                <Progress percent={progress} status="active" strokeColor="#1890ff" />
                <Text type="secondary" style={{ fontSize: '13px', display: 'block', textAlign: 'center', marginTop: '8px' }}>
                    Step {currentStep + 1} of {totalSteps}
                </Text>
            </div>
            <Divider style={{ margin: '16px 0' }} />
            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Loan Amount</Text>
                    <div className={styles.summaryValue}>₹ {(formData.loanDetails?.requestedAmount || 0).toLocaleString()}</div>
                </div>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Tenure</Text>
                    <div className={styles.summaryValue}>{formData.loanDetails?.tenureMonths || 0} months</div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>

            <Divider style={{ margin: '12px 0' }} />

            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Document uploaded</Text>
                    <div className={styles.summaryValue}>
                        {formData.personalDetails?.idProofImage ? 'Successfully' : 'Pending'}
                    </div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>

            <Divider style={{ margin: '12px 0' }} />

            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Full Name</Text>
                    <div className={styles.summaryValue}>{formData.personalDetails?.fullName || '-'}</div>
                </div>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>D.O.B</Text>
                    <div className={styles.summaryValue}>{formData.personalDetails?.dateOfBirth || '-'}</div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>
        </Card>
    );
};

export default Summary;
