import React, { useState } from "react";
import { Input, Checkbox, Space, Button, Card, message } from "antd";
import { ArrowLeftOutlined, DollarOutlined } from '@ant-design/icons';
import { useLoanSubmission } from '../../../../hooks/useLoanSubmission';
import { useAuth, useLoanApplication } from '../../../../context';
import { useNavigate } from 'react-router-dom';
import styles from './Declaration.module.css';

interface DeclarationProps {
  loanId: string;
  fullname: string;
  amount: number;
  onBackStep: () => void;
  onNext: () => void;
  onPrev: () => void;
}

const Declaration: React.FC<DeclarationProps> = ({
  fullname,
  amount,
  onBackStep,
  onPrev,
}) => {
  const navigate = useNavigate();
  const { token } = useAuth();
  const { state, dispatch } = useLoanApplication();
  const { submitPersonalLoan, submitHomeLoan, submitVehicleLoan, loading } = useLoanSubmission();
  
  const [purpose, setPurpose] = useState('');
  const [expectedAmount, setExpectedAmount] = useState(0);
  const [termsAccepted, setTermsAccepted] = useState(false);

  const handleBack = () => {
    onPrev();
    onBackStep();
  };

  const handleSubmit = async () => {
    if (!purpose.trim()) {
      message.error('Please enter the purpose of loan');
      return;
    }

    if (!expectedAmount || expectedAmount <= 0) {
      message.error('Please enter a valid expected amount');
      return;
    }

    if (!termsAccepted) {
      message.error('Please accept the terms and conditions');
      return;
    }

    const declarationData = {
      name: fullname,
      amount: expectedAmount,
      description: `Loan application for ${purpose}`,
      purpose: purpose
    };

    dispatch({
      type: 'UPDATE_FORM_DATA',
      payload: { section: 'declaration', data: declarationData }
    });

    try {
      const customerId = parseInt(token?.customerId || token?.CustomerId || '0');
      
      if (!customerId) {
        message.error('Customer ID not found');
        return;
      }

      const { loanType, formData } = state;

      if (loanType === 'personal') {
        const payload = {
          customerId,
          tenureMonths: formData.loanDetails.tenureMonths!,
          requestedLoanAmount: formData.loanDetails.requestedLoanAmount!,
          employmentType: formData.loanDetails.employmentType!,
          monthlyIncome: formData.loanDetails.monthlyIncome!,
          workExperienceYears: formData.loanDetails.workExperienceYears!,
          loanPurpose: formData.loanDetails.loanPurpose!,
          documents: formData.documents,
          personalDetails: formData.personalDetails as any,
          addressInformation: formData.addressInformation as any,
          familyEmergencyDetails: formData.familyEmergencyDetails as any,
          declaration: declarationData
        };
        await submitPersonalLoan(payload);
      } else if (loanType === 'home') {
        const payload = {
          customerId,
          tenureMonths: formData.loanDetails.tenureMonths!,
          requestedLoanAmount: formData.loanDetails.requestedLoanAmount!,
          propertyType: formData.loanDetails.propertyType!,
          propertyAddress: formData.loanDetails.propertyAddress!,
          city: formData.loanDetails.city!,
          zipCode: formData.loanDetails.zipCode!,
          ownershipType: formData.loanDetails.ownershipType!,
          propertyCost: formData.loanDetails.propertyCost!,
          downPayment: formData.loanDetails.downPayment!,
          loanPurpose: formData.loanDetails.loanPurpose!,
          documents: formData.documents,
          personalDetails: formData.personalDetails as any,
          addressInformation: formData.addressInformation as any,
          familyEmergencyDetails: formData.familyEmergencyDetails as any,
          declaration: declarationData
        };
        await submitHomeLoan(payload);
      } else if (loanType === 'vehicle') {
        const payload = {
          customerId,
          tenureMonths: formData.loanDetails.tenureMonths!,
          requestedLoanAmount: formData.loanDetails.requestedLoanAmount!,
          vehicleType: formData.loanDetails.vehicleType!,
          manufacturer: formData.loanDetails.manufacturer!,
          model: formData.loanDetails.model!,
          manufacturingYear: formData.loanDetails.manufacturingYear!,
          onRoadPrice: formData.loanDetails.onRoadPrice!,
          downPayment: formData.loanDetails.downPayment!,
          loanPurposeVehicle: formData.loanDetails.loanPurposeVehicle!,
          documents: formData.documents,
          personalDetails: formData.personalDetails as any,
          addressInformation: formData.addressInformation as any,
          familyEmergencyDetails: formData.familyEmergencyDetails as any,
          declaration: declarationData
        };
        await submitVehicleLoan(payload);
      }

      message.success('Loan application submitted successfully!');
      navigate('/customer/view-status');
    } catch (error: any) {
      message.error(error.message || 'Failed to submit loan application');
    }
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
            I/We <strong>{fullname}</strong> like to obtain a loan of{' '}
            <strong>
              <DollarOutlined /> ₹{amount.toLocaleString()}
            </strong>{' '}
            for the purpose of{' '}
            <Input
              className={styles.inlineInput}
              placeholder="Enter purpose"
              value={purpose}
              onChange={(e) => setPurpose(e.target.value)}
              required
            />
            . Expected expenditure for this purpose will be approx{' '}
            <Input
              type="number"
              className={styles.inlineInput}
              placeholder="Amount"
              value={expectedAmount || ''}
              onChange={(e) => setExpectedAmount(Number(e.target.value))}
              prefix="₹"
              required
            />
            .
          </p>
        </div>

        <div className={styles.termsContainer}>
          <Checkbox
            checked={termsAccepted}
            onChange={(e) => setTermsAccepted(e.target.checked)}
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
            disabled={loading}
          >
            BACK
          </Button>
          
          <Button
            type="primary"
            onClick={handleSubmit}
            className={styles.submitButton}
            disabled={!purpose || !expectedAmount || !termsAccepted}
            loading={loading}
          >
            SUBMIT APPLICATION
          </Button>
        </Space>
      </div>
    </div>
  );
};

export default Declaration;
