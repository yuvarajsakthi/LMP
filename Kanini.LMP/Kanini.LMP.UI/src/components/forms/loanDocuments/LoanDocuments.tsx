
import React, { useState } from 'react';
import { Form, Button, Card, Typography, message, Upload } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { NextButtonArrow } from '../../../assets';
import styles from './LoanDocuments.module.css';
import type { UploadFile, UploadProps } from 'antd';

interface LoanDocumentsProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

interface DocumentFile {
  uid: string;
  name: string;
  status: string;
  url?: string;
}

const LoanDocuments: React.FC<LoanDocumentsProps> = ({ onNext, onPrevious }) => {
  const [passportPhoto, setPassportPhoto] = useState<UploadFile[]>([]);
  const [signature, setSignature] = useState<UploadFile[]>([]);
  const [idProof, setIdProof] = useState<UploadFile[]>([]);

  const handleSubmit = () => {
    if (passportPhoto.length === 0) {
      message.warning('Please upload passport size photo');
      return;
    }
    if (signature.length === 0) {
      message.warning('Please upload signature');
      return;
    }
    if (idProof.length === 0) {
      message.warning('Please upload ID proof');
      return;
    }

    console.log('Documents uploaded:', { passportPhoto, signature, idProof });
    message.success('Documents uploaded successfully');
    onNext?.();
  };

  const handleBack = () => {
    onPrevious?.();
  };
  const uploadProps: UploadProps = {
    beforeUpload: (file) => {
      const isImage = file.type.startsWith('image/');
      if (!isImage) {
        message.error('You can only upload image files!');
      }
      const isLt2M = file.size / 1024 / 1024 < 2;
      if (!isLt2M) {
        message.error('Image must be smaller than 2MB!');
      }
      return false; // Prevent auto upload
    },
    accept: '.jpg,.jpeg,.png',
    maxCount: 1,
  };

  return (
    <Card className={styles.container}>
      <div className={styles.header}>
        <h2>Upload Documents</h2>
        <p>Upload the documents listed below for verification purposes</p>
      </div>

      <Form onFinish={handleSubmit} className={styles.form}>
        <div className={styles.uploadSection}>
          <Typography.Text className={styles.label}>Passport Size Photo *</Typography.Text>
          <Upload
            {...uploadProps}
            fileList={passportPhoto}
            onChange={({ fileList }) => setPassportPhoto(fileList)}
            className={styles.upload}
          >
            <Button icon={<UploadOutlined />} className={styles.uploadButton}>
              Select File
            </Button>
          </Upload>
          <Typography.Text className={styles.hint}>Upload passport size photo (jpg/png)</Typography.Text>
        </div>

        <div className={styles.uploadSection}>
          <Typography.Text className={styles.label}>Signature *</Typography.Text>
          <Upload
            {...uploadProps}
            fileList={signature}
            onChange={({ fileList }) => setSignature(fileList)}
            className={styles.upload}
          >
            <Button icon={<UploadOutlined />} className={styles.uploadButton}>
              Select File
            </Button>
          </Upload>
          <Typography.Text className={styles.hint}>Upload signature (jpg/png)</Typography.Text>
        </div>

        <div className={styles.uploadSection}>
          <Typography.Text className={styles.label}>ID Proof *</Typography.Text>
          <Upload
            {...uploadProps}
            fileList={idProof}
            onChange={({ fileList }) => setIdProof(fileList)}
            className={styles.upload}
          >
            <Button icon={<UploadOutlined />} className={styles.uploadButton}>
              Select File
            </Button>
          </Upload>
          <Typography.Text className={styles.hint}>Upload ID proof (jpg/png)</Typography.Text>
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