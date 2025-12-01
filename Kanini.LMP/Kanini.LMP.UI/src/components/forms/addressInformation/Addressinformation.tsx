import React from 'react';
import { Input, Form, Space, Button, Row, Col } from 'antd';
import { ArrowLeftOutlined } from '@ant-design/icons';
import styles from './Addressinformation.module.css';
import axios from 'axios';
import { useAuth } from '../../../context';
import { NextButtonArrow } from '../../../assets';

interface AddressFormData {
  present: string;
  permanent: string;
  district: string;
  country: string;
  emailid: string;
  mobile1: string;
  mobile2?: string;
  telephone: string;
}

interface AddressinformationProps {
  loanId: string;
  addressInfoId?: string;
  onNext: () => void;
  onPrev: () => void;
  onNextStep: () => void;
  onBackStep: () => void;
  initialData?: AddressFormData;
  onDataChange?: (data: AddressFormData) => void;
}

const Addressinformation: React.FC<AddressinformationProps> = ({
  loanId,
  addressInfoId,
  onNext,
  onPrev,
  onNextStep,
  onBackStep,
  initialData,
  onDataChange
}) => {
  const { token } = useAuth();
  const [form] = Form.useForm();

  const handleSubmit = async (values: AddressFormData) => {
    try {
      const addressData = {
        ...values,
        presentAddress: values.present,
        permanentAddress: values.permanent,
        telephoneNo: values.telephone,
        loanId: loanId,
      };

      if (onDataChange) {
        onDataChange(values);
      }

      if (addressInfoId) {
        // Update existing address
        await axios.put(`/address/${addressInfoId}`, addressData, {
          headers: { Authorization: `Bearer ${token}` }
        });
      } else {
        // Create new address
        await axios.post('/address', addressData, {
          headers: { Authorization: `Bearer ${token}` }
        });
      }

      onNext();
      onNextStep();
    } catch (error) {
      console.error('Error saving address information:', error);
    }
  };

  const handleBack = () => {
    onPrev();
    onBackStep();
  };

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h3 className={styles.title}>Address Information</h3>
        <p className={styles.subtitle}>Enter your address information as mentioned in IDs</p>
      </div>

      <Form 
        form={form}
        onFinish={handleSubmit} 
        initialValues={initialData}
        layout="vertical"
        className={styles.form}
      >
        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="present"
              label="Present Address (Residential)"
              rules={[
                { required: true, message: 'Present address is required' }
              ]}
            >
              <Input 
                placeholder="Enter your present address" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={24}>
            <Form.Item
              name="permanent"
              label="Permanent Address"
              rules={[
                { required: true, message: 'Permanent address is required' }
              ]}
            >
              <Input 
                placeholder="Enter your permanent address" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item
              name="district"
              label="District"
              rules={[
                { required: true, message: 'District is required' }
              ]}
            >
              <Input 
                placeholder="Enter district" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="country"
              label="Country"
              rules={[
                { required: true, message: 'Country is required' }
              ]}
            >
              <Input 
                placeholder="Enter country" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="emailid"
              label="Email ID"
              rules={[
                { required: true, message: 'Email address is required' },
                { type: 'email', message: 'Please enter a valid email address' }
              ]}
            >
              <Input 
                placeholder="Enter email address" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item
              name="mobile1"
              label="Mobile 1"
              rules={[
                { required: true, message: 'Mobile number is required' },
                {
                  pattern: /^[0-9]{10}$/,
                  message: 'Please enter a valid 10-digit mobile number'
                }
              ]}
            >
              <Input 
                placeholder="Enter mobile number" 
                className={styles.input}
                maxLength={10}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="mobile2"
              label="Mobile 2 (Optional)"
              rules={[
                {
                  pattern: /^[0-9]{10}$/,
                  message: 'Please enter a valid 10-digit mobile number'
                }
              ]}
            >
              <Input 
                placeholder="Enter alternate mobile number" 
                className={styles.input}
                maxLength={10}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="telephone"
              label="Telephone Number"
              rules={[
                { required: true, message: 'Telephone number is required' }
              ]}
            >
              <Input 
                placeholder="Enter telephone number" 
                className={styles.input}
              />
            </Form.Item>
          </Col>
        </Row>

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
              htmlType="submit"
              className={styles.nextButton}
            >
              NEXT
              <img 
                src={NextButtonArrow} 
                alt="Next" 
                className={styles.nextIcon}
              />
            </Button>
          </Space>
        </div>
      </Form>
    </div>
  );
};

export default Addressinformation;