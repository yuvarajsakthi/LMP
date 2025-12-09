
import React, { useState, useEffect } from 'react';
import { Form, Button, Card, Typography, message } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { useDropzone } from 'react-dropzone';
import { NextButtonArrow } from '../../../../assets';
import { useLoanApplication } from '../../../../context';
import styles from './LoanDocuments.module.css';

interface LoanDocumentsProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

const LoanDocuments: React.FC<LoanDocumentsProps> = ({ onNext, onPrevious }) => {
  const { state, dispatch } = useLoanApplication();
  const [signature, setSignature] = useState<File | null>(null);
  const [idProof, setIdProof] = useState<File | null>(null);

  useEffect(() => {
    const personalDetails = state.formData.personalDetails;
    if (personalDetails) {
      if (personalDetails.signatureImage) setSignature(personalDetails.signatureImage as File);
      if (personalDetails.idProofImage) setIdProof(personalDetails.idProofImage as File);
    }
  }, []);

  const handleSubmit = () => {
    if (!signature) {
      message.warning('Please upload signature');
      return;
    }
    if (!idProof) {
      message.warning('Please upload ID proof');
      return;
    }

    // Store signature and ID proof in personalDetails for backend mapping
    const personalDetailsUpdate = {
      ...state.formData.personalDetails,
      signatureImage: signature,
      idProofImage: idProof
    };
    
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'personalDetails', data: personalDetailsUpdate } });
    message.success('Documents uploaded successfully');
    onNext?.();
  };

  const handleBack = () => {
    onPrevious?.();
  };
  const createDropzone = (onDrop: (file: File) => void) => useDropzone({
    accept: { 'image/*': ['.jpg', '.jpeg', '.png'] },
    maxFiles: 1,
    maxSize: 2 * 1024 * 1024,
    onDrop: (acceptedFiles) => {
      if (acceptedFiles.length > 0) {
        onDrop(acceptedFiles[0]);
      }
    },
    onDropRejected: () => {
      message.error('Please upload a valid image file (jpg/png) smaller than 2MB');
    }
  });

  const signatureDropzone = createDropzone(setSignature);
  const idProofDropzone = createDropzone(setIdProof);

  return (
    <Card className={styles.container}>
      <div className={styles.header}>
        <h2>Upload Documents</h2>
        <p>Upload the documents listed below for verification purposes</p>
      </div>

      <Form onFinish={handleSubmit} className={styles.form}>
        <div className={styles.uploadSection}>
          <Typography.Text className={styles.label}>Signature *</Typography.Text>
          <div {...signatureDropzone.getRootProps()} className={styles.dropzone}>
            <input {...signatureDropzone.getInputProps()} />
            <UploadOutlined style={{ fontSize: 24, marginBottom: 8 }} />
            {signature ? (
              <Typography.Text>{signature.name}</Typography.Text>
            ) : (
              <Typography.Text>Drag & drop or click to select file</Typography.Text>
            )}
          </div>
          <Typography.Text className={styles.hint}>Upload signature (jpg/png, max 2MB)</Typography.Text>
        </div>

        <div className={styles.uploadSection}>
          <Typography.Text className={styles.label}>ID Proof *</Typography.Text>
          <div {...idProofDropzone.getRootProps()} className={styles.dropzone}>
            <input {...idProofDropzone.getInputProps()} />
            <UploadOutlined style={{ fontSize: 24, marginBottom: 8 }} />
            {idProof ? (
              <Typography.Text>{idProof.name}</Typography.Text>
            ) : (
              <Typography.Text>Drag & drop or click to select file</Typography.Text>
            )}
          </div>
          <Typography.Text className={styles.hint}>Upload ID proof (jpg/png, max 2MB)</Typography.Text>
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

export default LoanDocuments;