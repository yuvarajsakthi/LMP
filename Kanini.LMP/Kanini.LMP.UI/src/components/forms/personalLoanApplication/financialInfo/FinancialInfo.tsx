import React from 'react';
import { Form, Input, Button, Radio, Divider, Card, message } from 'antd';
import { NextButtonArrow } from '../../../../assets';
import styles from './FinancialInfo.module.css';

interface FinancialInfoProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

interface FinancialInfoForm {
  salary: string;
  rent: string;
  other1: string;
  rentandutility: string;
  food: string;
  education: string;
  repayment: string;
  other2: string;
  radiogroupCredit: string;
  odf?: string;
  cars: string;
  radiogroupDirectiorship?: string;
}

const FinancialInfo: React.FC<FinancialInfoProps> = ({ onNext, onPrevious }) => {
  const [form] = Form.useForm<FinancialInfoForm>();

  const handleSubmit = async (values: FinancialInfoForm) => {
    try {
      message.success('Financial information saved successfully');
      onNext?.();
    } catch (error) {
      message.error('Failed to save financial information');
    }
  };

  const handleBack = () => {
    onPrevious?.();
  };

  return (
    <Card className={styles.container}>
      <div className={styles.header}>
        <h2>Financial Information</h2>
        <p>Enter your financial information</p>
      </div>

      <Form form={form} onFinish={handleSubmit} layout="vertical" className={styles.form}>
        <div className={styles.section}>
          <h4 className={styles.sectionTitle}>Primary Monthly Income</h4>
          <div className={styles.formRow}>
            <Form.Item
              name="salary"
              label="Salary"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Salary is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter salary amount" />
            </Form.Item>
            <Form.Item
              name="rent"
              label="Rent Income"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Rent income is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter rent income" />
            </Form.Item>
            <Form.Item
              name="other1"
              label="Other Income"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Other income is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter other income" />
            </Form.Item>
          </div>
        </div>

        <div className={styles.section}>
          <h4 className={styles.sectionTitle}>Monthly Expenses</h4>
          <div className={styles.formRow}>
            <Form.Item
              name="rentandutility"
              label="Rent & Utility"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Rent & utility is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter rent & utility" />
            </Form.Item>
            <Form.Item
              name="food"
              label="Food & Clothing"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Food & clothing is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter food & clothing" />
            </Form.Item>
            <Form.Item
              name="education"
              label="Education"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Education expense is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter education expense" />
            </Form.Item>
          </div>
          <div className={styles.formRow}>
            <Form.Item
              name="repayment"
              label="Loan Repayment"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Loan repayment is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter loan repayment" />
            </Form.Item>
            <Form.Item
              name="other2"
              label="Other Expenses"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Other expenses is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid amount' }
              ]}
            >
              <Input placeholder="Enter other expenses" />
            </Form.Item>
          </div>
        </div>

        <Divider />

        <div className={styles.section}>
          <div className={styles.radioSection}>
            <span className={styles.radioLabel}>Credit facility with other banks*</span>
            <Form.Item name="radiogroupCredit" initialValue="No">
              <Radio.Group>
                <Radio value="Yes">Yes</Radio>
                <Radio value="No">No</Radio>
              </Radio.Group>
            </Form.Item>
          </div>

          <div className={styles.formRow}>
            <Form.Item
              name="odf"
              label="Interest Rate of OD Facility (If any)"
              className={styles.formItem}
            >
              <Input placeholder="Enter interest rate" />
            </Form.Item>
            <Form.Item
              name="cars"
              label="Number of Cars"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Number of cars is required' },
                { pattern: /^\d+$/, message: 'Please enter a valid number' }
              ]}
            >
              <Input placeholder="Enter number of cars" />
            </Form.Item>
          </div>
        </div>

        <div className={styles.buttonContainer}>
          {onPrevious && (
            <Button onClick={handleBack} className={styles.backButton}>
              BACK
            </Button>
          )}
          <Button
            type="primary"
            htmlType="submit"
            className={styles.nextButton}
          >
            NEXT
            <img src={NextButtonArrow} alt="Next" className={styles.buttonIcon} />
          </Button>
        </div>
      </Form>
    </Card>
  );
};

export default FinancialInfo;