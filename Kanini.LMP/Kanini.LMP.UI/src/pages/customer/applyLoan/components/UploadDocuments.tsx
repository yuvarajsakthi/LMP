import React from 'react';
import { Upload, Button, Typography, Card, message } from 'antd';
import { UploadOutlined } from '@ant-design/icons';
import { useLoanApplication } from '../context/LoanApplicationContext';

const { Title } = Typography;

const UploadDocuments: React.FC = () => {
  const { dispatch } = useLoanApplication();

  const handleNext = () => {
    dispatch({ type: 'NEXT_STEP' });
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  const uploadProps = {
    name: 'file',
    multiple: true,
    beforeUpload: () => false, // Prevent auto upload
    onChange: (info: any) => {
      message.success(`${info.file.name} file selected successfully`);
    }
  };

  return (
    <Card>
      <Title level={4}>Upload Documents</Title>
      <div style={{ marginBottom: 16 }}>
        <p>Please upload the following documents:</p>
        <ul>
          <li>Identity Proof (Aadhar Card, PAN Card)</li>
          <li>Address Proof</li>
          <li>Income Proof (Salary Slips, Bank Statements)</li>
          <li>Employment Proof</li>
        </ul>
      </div>

      <Upload.Dragger {...uploadProps}>
        <p className="ant-upload-drag-icon">
          <UploadOutlined />
        </p>
        <p className="ant-upload-text">Click or drag file to this area to upload</p>
        <p className="ant-upload-hint">
          Support for single or bulk upload. Strictly prohibited from uploading company data or other banned files.
        </p>
      </Upload.Dragger>

      <div style={{ marginTop: 16 }}>
        <Button onClick={handlePrev} style={{ marginRight: 8 }}>
          Previous
        </Button>
        <Button type="primary" onClick={handleNext}>
          Next
        </Button>
      </div>
    </Card>
  );
};

export default UploadDocuments;