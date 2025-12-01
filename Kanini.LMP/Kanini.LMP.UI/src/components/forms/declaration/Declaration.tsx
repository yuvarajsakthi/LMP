import React, { useState } from "react";
import { Input, Checkbox, Space, Button, Form, Card } from "antd";
import { ArrowLeftOutlined, DollarOutlined } from '@ant-design/icons';
import { useSnackbar } from 'notistack';
import styles from './Declaration.module.css';

interface DeclarationFormData {
  purpose: string;
  expectedAmount: number;
  termsAccepted: boolean;
}

interface DeclarationProps {
  loanId: string;
  fullname: string;
  amount: number;
  onBackStep: () => void;
  onNext: () => void;
  onPrev: () => void;
  initialData?: DeclarationFormData;
  onDataChange?: (data: DeclarationFormData) => void;
  onSubmit?: (data: DeclarationFormData) => void;
}

const Declaration: React.FC<DeclarationProps> = ({
  loanId,
  fullname,
  amount,
  onBackStep,
  onNext,
  onPrev,
  initialData,
  onDataChange,
  onSubmit
}) => {
  const { enqueueSnackbar } = useSnackbar();
  const [form] = Form.useForm();
  const [purpose, setPurpose] = useState(initialData?.purpose || '');
  const [expectedAmount, setExpectedAmount] = useState(initialData?.expectedAmount || 0);
  const [termsAccepted, setTermsAccepted] = useState(initialData?.termsAccepted || false);

  const handleBack = () => {
    onPrev();
    onBackStep();
  };

  const handleSubmit = () => {
    if (!purpose.trim()) {
      enqueueSnackbar('Please enter the purpose of loan', { variant: 'error' });
      return;
    }

    if (!expectedAmount || expectedAmount <= 0) {
      enqueueSnackbar('Please enter a valid expected amount', { variant: 'error' });
      return;
    }

    if (!termsAccepted) {
      enqueueSnackbar('Please accept the terms and conditions', { variant: 'error' });
      return;
    }

    const formData: DeclarationFormData = {
      purpose,
      expectedAmount,
      termsAccepted
    };

    if (onDataChange) {
      onDataChange(formData);
    }

    if (onSubmit) {
      onSubmit(formData);
    } else {
      onNext();
    }

    enqueueSnackbar('Declaration submitted successfully', { variant: 'success' });
  };

  const handlePurposeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setPurpose(e.target.value);
  };

  const handleAmountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setExpectedAmount(Number(e.target.value));
  };

  const handleTermsChange = (e: any) => {
    setTermsAccepted(e.target.checked);
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.title}>Declaration</h1>
        <p className={styles.subtitle}>
          Please declare the total value of expenditure for the purpose of loan
        </p>
      </div>

      <Card className={styles.declarationCard}>
        <div className={styles.declarationContent}>
          <p className={styles.declarationText}>
            I/We <strong>{fullname}</strong> like to obtain a personal loan of{' '}
            <strong>
              <DollarOutlined /> ₹{amount.toLocaleString()}
            </strong>{' '}
            for the purpose of{' '}
            <Input
              className={styles.inlineInput}
              placeholder="Enter purpose"
              value={purpose}
              onChange={handlePurposeChange}
              required
            />
            . Expected expenditure for this purpose will be approx{' '}
            <Input
              type="number"
              className={styles.inlineInput}
              placeholder="Amount"
              value={expectedAmount || ''}
              onChange={handleAmountChange}
              prefix="₹"
              required
            />
            .
          </p>
        </div>

        <div className={styles.termsContainer}>
          <Checkbox
            checked={termsAccepted}
            onChange={handleTermsChange}
            className={styles.termsCheckbox}
          >
            <span className={styles.termsText}>
              I/We also understand the maximum amount of loan will be determined by the bank 
              at its discretion if approved. However the bank reserves the right to accept or 
              reject my application at its discretion without assigning any notice whatsoever.
            </span>
          </Checkbox>
        </div>
      </Card>

      <div className={styles.buttonContainer}>
        <Space size="large">
          <Button
            onClick={handleBack}
            className={styles.backButton}
            icon={<ArrowLeftOutlined />}
          >
            BACK
          </Button>
          
          <Button
            type="primary"
            onClick={handleSubmit}
            className={styles.submitButton}
            disabled={!purpose || !expectedAmount || !termsAccepted}
          >
            SUBMIT DECLARATION
          </Button>
        </Space>
      </div>
    </div>
  );
};

export default Declaration;