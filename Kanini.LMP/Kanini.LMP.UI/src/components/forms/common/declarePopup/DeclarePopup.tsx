import React, { useState } from "react";
import { Button, Modal } from "antd";
import { DownloadOutlined, DashboardOutlined } from '@ant-design/icons';
import { useSnackbar } from 'notistack';
import { useNavigate } from "react-router-dom";
import styles from './DeclarePopup.module.css';
import axios from 'axios';
import { useAuth } from '../../../../context';
import { ROUTES } from '../../../../config';
import { CircleCheck } from '../../../../assets';

interface DeclarationData {
  loanId: string;
  purpose: string;
  expenditureAmount: number;
  appliedAmount: number;
  requestedTenure: number;
  fullName: string;
  applicationId: string;
}

interface DeclarePopupProps {
  loanId: string;
  purpose: string;
  expectedAmount: number;
  checkboxChecked: boolean;
  declarationData: DeclarationData;
  onSuccess?: () => void;
}

const DeclarePopup: React.FC<DeclarePopupProps> = ({
  loanId,
  purpose,
  expectedAmount,
  checkboxChecked,
  declarationData,
  onSuccess
}) => {
  const { enqueueSnackbar } = useSnackbar();
  const { token } = useAuth();
  const navigate = useNavigate();
  
  const [loading, setLoading] = useState(false);
  const [open, setOpen] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const validateInputs = (): boolean => {
    if (!expectedAmount || !purpose) {
      enqueueSnackbar("Please enter values for Purpose and Amount.", { variant: 'error' });
      return false;
    }

    if (!/^[a-zA-Z\s]+$/.test(purpose)) {
      enqueueSnackbar("Purpose should contain only text characters.", { variant: 'error' });
      return false;
    }

    if (!/^\d+$/.test(expectedAmount.toString())) {
      enqueueSnackbar("Amount should contain only numbers.", { variant: 'error' });
      return false;
    }

    if (!checkboxChecked) {
      enqueueSnackbar("Please agree to the Terms and Conditions", { variant: 'error' });
      return false;
    }

    return true;
  };

  const submitDeclaration = async () => {
    if (!validateInputs()) return;

    setSubmitting(true);
    try {
      const declarationPayload = {
        loanId: loanId,
        appliedAmount: declarationData.appliedAmount,
        requestedTenure: declarationData.requestedTenure,
        appliedDate: new Date().toISOString(),
        purpose: purpose,
        applicationId: declarationData.applicationId,
        statusId: 1,
        expenditureAmount: expectedAmount,
      };

      await axios.put(`/declaration/${loanId}`, declarationPayload, {
        headers: { Authorization: `Bearer ${token}` }
      });

      enqueueSnackbar("Declaration submitted successfully!", { variant: 'success' });
      setOpen(true);
      
      if (onSuccess) {
        onSuccess();
      }
    } catch (error) {
      console.error('Error submitting declaration:', error);
      enqueueSnackbar("Failed to submit declaration. Please try again.", { variant: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const handleDownloadApplication = () => {
    setLoading(true);
    
    // Create a simple text-based application summary
    const applicationData = `
LOAN APPLICATION SUMMARY
========================

Application ID: ${declarationData.applicationId}
Full Name: ${declarationData.fullName}
Loan Amount: ₹${declarationData.appliedAmount.toLocaleString()}
Tenure: ${declarationData.requestedTenure} years
Purpose: ${purpose}
Expected Expenditure: ₹${expectedAmount.toLocaleString()}
Application Date: ${new Date().toLocaleDateString()}

This is a system-generated application summary.
    `;

    // Create and download as text file
    const blob = new Blob([applicationData], { type: 'text/plain' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `loan-application-${declarationData.applicationId}.txt`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);

    setTimeout(() => {
      setLoading(false);
      handleGoToDashboard();
    }, 1000);
  };

  const handleGoToDashboard = () => {
    setOpen(false);
    navigate(ROUTES.CUSTOMER_DASHBOARD);
  };

  const handleCancel = () => {
    setOpen(false);
  };

  return (
    <div>
      <Button 
        type="primary" 
        onClick={submitDeclaration}
        loading={submitting}
        className={styles.submitButton}
      >
        Submit Declaration
      </Button>

      <Modal
        open={open}
        onCancel={handleCancel}
        footer={[
          <Button
            key="download"
            type="primary"
            loading={loading}
            onClick={handleDownloadApplication}
            icon={<DownloadOutlined />}
            className={styles.downloadButton}
          >
            Download Application
          </Button>,
          <Button
            key="dashboard"
            type="primary"
            onClick={handleGoToDashboard}
            icon={<DashboardOutlined />}
            className={styles.dashboardButton}
          >
            Go to Dashboard
          </Button>,
        ]}
        width={600}
        className={styles.successModal}
      >
        <div className={styles.modalContent}>
          <div className={styles.successIcon}>
            <img src={CircleCheck} alt="Success" className={styles.checkIcon} />
          </div>
          
          <h2 className={styles.successTitle}>
            Loan Application Submitted Successfully
          </h2>
          
          <p className={styles.successMessage}>
            Your loan application has been submitted successfully. 
            Our team will review your application and get back to you soon.
          </p>

          <div className={styles.applicationSummary}>
            <div className={styles.summaryItem}>
              <span className={styles.label}>Application ID:</span>
              <span className={styles.value}>{declarationData.applicationId}</span>
            </div>
            <div className={styles.summaryItem}>
              <span className={styles.label}>Loan Amount:</span>
              <span className={styles.value}>₹{declarationData.appliedAmount.toLocaleString()}</span>
            </div>
            <div className={styles.summaryItem}>
              <span className={styles.label}>Purpose:</span>
              <span className={styles.value}>{purpose}</span>
            </div>
          </div>
        </div>
      </Modal>
    </div>
  );
};

export default DeclarePopup;