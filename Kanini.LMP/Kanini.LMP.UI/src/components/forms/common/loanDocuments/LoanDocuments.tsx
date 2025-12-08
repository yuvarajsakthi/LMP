
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
  const [passportPhoto, setPassportPhoto] = useState<File | null>(null);
  const [signature, setSignature] = useState<File | null>(null);
  const [idProof, setIdProof] = useState<File | null>(null);

  useEffect(() => {
    const docs = state.formData.documents;
    if (docs && docs.length > 0) {
      // Restore files from context if available
      const passportDoc = docs.find(d => d.documentName === 'passportPhoto');
      const signatureDoc = docs.find(d => d.documentName === 'signature');
      const idProofDoc = docs.find(d => d.documentName === 'idProof');
      
      if (passportDoc?.documentFile) setPassportPhoto(passportDoc.documentFile);
      if (signatureDoc?.documentFile) setSignature(signatureDoc.documentFile);
      if (idProofDoc?.documentFile) setIdProof(idProofDoc.documentFile);
    }
  }, []);

  const handleSubmit = () => {
    if (!passportPhoto) {
      message.warning('Please upload passport size photo');
      return;
    }
    if (!signature) {
      message.warning('Please upload signature');
      return;
    }
    if (!idProof) {
      message.warning('Please upload ID proof');
      return;
    }

    const documents = [
      { documentName: 'passportPhoto', documentType: 0, documentFile: passportPhoto },
      { documentName: 'signature', documentType: 0, documentFile: signature },
      { documentName: 'idProof', documentType: 0, documentFile: idProof }
    ];
    
    dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'documents', data: documents } });
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

  const passportDropzone = createDropzone(setPassportPhoto);
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
          <Typography.Text className={styles.label}>Passport Size Photo *</Typography.Text>
          <div {...passportDropzone.getRootProps()} className={styles.dropzone}>
            <input {...passportDropzone.getInputProps()} />
            <UploadOutlined style={{ fontSize: 24, marginBottom: 8 }} />
            {passportPhoto ? (
              <Typography.Text>{passportPhoto.name}</Typography.Text>
            ) : (
              <Typography.Text>Drag & drop or click to select file</Typography.Text>
            )}
          </div>
          <Typography.Text className={styles.hint}>Upload passport size photo (jpg/png, max 2MB)</Typography.Text>
        </div>

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