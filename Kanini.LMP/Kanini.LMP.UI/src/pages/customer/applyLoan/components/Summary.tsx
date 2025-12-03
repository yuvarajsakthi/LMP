import React from 'react';
import { Card, Typography, Divider } from 'antd';
import { EditOutlined } from '@ant-design/icons';
import { useLoanApplication } from '../context/LoanApplicationContext';
import styles from '../ApplyLoan.module.css';

const { Title, Text } = Typography;

const Summary: React.FC = () => {
    const { state } = useLoanApplication();
    const { formData } = state;

    return (
        <Card className={styles.summaryCard} title="Summary" bordered={false}>
            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Loan Amount</Text>
                    <div className={styles.summaryValue}>₹ {formData.loanDetails.requestedAmount.toLocaleString()}</div>
                </div>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Tenure</Text>
                    <div className={styles.summaryValue}>{formData.loanDetails.tenureMonths} months</div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>

            <Divider style={{ margin: '12px 0' }} />

            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Document uploaded</Text>
                    <div className={styles.summaryValue}>
                        {formData.personalDetails.idProofImage ? 'Successfully' : 'Pending'}
                    </div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>

            <Divider style={{ margin: '12px 0' }} />

            <div className={styles.summaryItem}>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>Full Name</Text>
                    <div className={styles.summaryValue}>{formData.personalDetails.fullName || '-'}</div>
                </div>
                <div>
                    <Text type="secondary" className={styles.summaryLabel}>D.O.B</Text>
                    <div className={styles.summaryValue}>{formData.personalDetails.dateOfBirth || '-'}</div>
                </div>
                <EditOutlined className={styles.editIcon} />
            </div>
        </Card>
    );
};

export default Summary;
