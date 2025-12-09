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
  const { state } = useLoanApplication();
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



    try {
      const customerId = parseInt(token?.customerId || token?.CustomerId || '0');
      
      if (!customerId) {
        message.error('Customer ID not found');
        return;
      }

      const { loanType, formData } = state;

      if (loanType === 'personal') {
        await submitPersonalLoan(customerId, {
          RequestedAmount: formData.loanDetails.requestedLoanAmount!,
          TenureMonths: formData.loanDetails.tenureMonths!,
          EmploymentType: formData.loanDetails.employmentType!,
          MonthlyIncome: formData.loanDetails.monthlyIncome!,
          WorkExperienceYears: formData.loanDetails.workExperienceYears!,
          LoanPurpose: formData.loanDetails.loanPurpose!,
          District: formData.addressInformation?.district!,
          State: formData.addressInformation?.state!,
          ZipCode: parseInt(formData.addressInformation?.zipCode || '0'),
          PresentAddress: formData.addressInformation?.presentAddress!,
          PermanentAddress: formData.addressInformation?.permanentAddress!,
          RelationFullName: formData.familyEmergencyDetails?.fullName!,
          RelationshipWithApplicant: formData.familyEmergencyDetails?.relationshipWithApplicant!,
          MobileNumber: parseInt(formData.familyEmergencyDetails?.mobileNumber || '0'),
          RelationAddress: formData.familyEmergencyDetails?.address!,
          SignatureImage: formData.personalDetails?.signatureImage,
          DocumentUpload: formData.personalDetails?.idProofImage
        });
      } else if (loanType === 'home') {
        await submitHomeLoan(customerId, {
          RequestedAmount: formData.loanDetails.requestedLoanAmount!,
          TenureMonths: formData.loanDetails.tenureMonths!,
          PropertyType: formData.loanDetails.propertyType!,
          PropertyAddress: formData.loanDetails.propertyAddress!,
          City: formData.loanDetails.city!,
          OwnershipType: formData.loanDetails.ownershipType!,
          PropertyCost: formData.loanDetails.propertyCost!,
          DownPayment: formData.loanDetails.downPayment!,
          LoanPurpose: formData.loanDetails.loanPurpose!,
          District: formData.addressInformation?.district!,
          State: formData.addressInformation?.state!,
          ZipCode: parseInt(formData.addressInformation?.zipCode || '0'),
          PresentAddress: formData.addressInformation?.presentAddress!,
          PermanentAddress: formData.addressInformation?.permanentAddress!,
          RelationFullName: formData.familyEmergencyDetails?.fullName!,
          RelationshipWithApplicant: formData.familyEmergencyDetails?.relationshipWithApplicant!,
          MobileNumber: parseInt(formData.familyEmergencyDetails?.mobileNumber || '0'),
          RelationAddress: formData.familyEmergencyDetails?.address!,
          SignatureImage: formData.personalDetails?.signatureImage
        });
      } else if (loanType === 'vehicle') {
        await submitVehicleLoan(customerId, {
          RequestedAmount: formData.loanDetails.requestedLoanAmount!,
          TenureMonths: formData.loanDetails.tenureMonths!,
          VehicleType: formData.loanDetails.vehicleType!,
          Manufacturer: formData.loanDetails.manufacturer!,
          Model: formData.loanDetails.model!,
          ManufacturingYear: formData.loanDetails.manufacturingYear!,
          OnRoadPrice: formData.loanDetails.onRoadPrice!,
          DownPayment: formData.loanDetails.downPayment!,
          LoanPurposeVehicle: formData.loanDetails.loanPurposeVehicle!,
          District: formData.addressInformation?.district!,
          State: formData.addressInformation?.state!,
          ZipCode: parseInt(formData.addressInformation?.zipCode || '0'),
          PresentAddress: formData.addressInformation?.presentAddress!,
          PermanentAddress: formData.addressInformation?.permanentAddress!,
          RelationFullName: formData.familyEmergencyDetails?.fullName!,
          RelationshipWithApplicant: formData.familyEmergencyDetails?.relationshipWithApplicant!,
          MobileNumber: parseInt(formData.familyEmergencyDetails?.mobileNumber || '0'),
          RelationAddress: formData.familyEmergencyDetails?.address!,
          SignatureImage: formData.personalDetails?.signatureImage,
          DocumentUpload: formData.personalDetails?.idProofImage
        });
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
