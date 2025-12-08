import React from "react";
import { Input, Space, Form, Select, Switch, Button, Row, Col } from 'antd';
import {  ArrowLeftOutlined } from '@ant-design/icons';
import styles from './Employement.module.css';
import axios from 'axios';
import { useAuth } from '../../../../context';
import { NextButtonArrow } from '../../../../assets';

interface EmploymentFormData {
  employmentType: boolean; // true for Business, false for Salaried
  name: string;
  designation: string;
  status: string;
  experience: number;
  phone?: string;
  emailid: string;
  address: string;
}

interface EmploymentProps {
  loanId: string;
  employmentId?: string;
  onNext: () => void;
  onPrev: () => void;
  onNextStep: () => void;
  onBackStep: () => void;
  initialData?: EmploymentFormData;
  onDataChange?: (data: EmploymentFormData) => void;
}

const Employment: React.FC<EmploymentProps> = ({
  loanId,
  employmentId,
  onNext,
  onPrev,
  onNextStep,
  onBackStep,
  initialData,
  onDataChange
}) => {
  const { token } = useAuth();
  const [form] = Form.useForm();

  const handleSubmit = async (values: EmploymentFormData) => {
    try {
      const employmentType = values.employmentType ? "Business" : "Salaried";
      const employmentData = {
        employmentType: employmentType,
        companyName: values.name,
        designation: values.designation,
        employeeStatus: values.status,
        experience: values.experience,
        officeNo: values.phone || '',
        emailId: values.emailid,
        officeAddress: values.address,
        loanId: loanId,
      };

      if (onDataChange) {
        onDataChange(values);
      }

      if (employmentId) {
        // Update existing employment
        await axios.put(`/employment/${employmentId}`, employmentData, {
          headers: { Authorization: `Bearer ${token}` }
        });
      } else {
        // Create new employment
        await axios.post('/employment', employmentData, {
          headers: { Authorization: `Bearer ${token}` }
        });
      }

      onNext();
      onNextStep();
    } catch (error) {
      console.error('Error saving employment details:', error);
    }
  };

  const handleBack = () => {
    onPrev();
    onBackStep();
  };

  const employeeStatusOptions = [
    { value: 'employee', label: 'Employee' },
    { value: 'business', label: 'Business Owner' },
    { value: 'self-employed', label: 'Self-employed' },
    { value: 'consultant', label: 'Consultant' },
    { value: 'freelancer', label: 'Freelancer' },
  ];

  return (
    <div className={styles.container}>
      <div className={styles.header}>
        <h1 className={styles.primaryText}>Employment Details</h1>
        <p className={styles.secondaryText}>
          Enter your past and current employment details
        </p>
      </div>

      <Form 
        form={form} 
        onFinish={handleSubmit} 
        initialValues={initialData}
        layout="vertical"
        className={styles.form}
      >
        <div className={styles.employmentTypeSection}>
          <Space align="center" size="middle">
            <span>Salaried</span>
            <Form.Item name="employmentType" valuePropName="checked" className={styles.switchItem}>
              <Switch />
            </Form.Item>
            <span>Business</span>
          </Space>
        </div>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item
              name="name"
              label="Company Name"
              rules={[{ required: true, message: 'Please enter company name' }]}
            >
              <Input 
                placeholder="Name of the company" 
                className={styles.company}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="designation"
              label="Designation"
              rules={[{ required: true, message: 'Please enter designation' }]}
            >
              <Input 
                placeholder="Your designation" 
                className={styles.designation}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="status"
              label="Employee Status"
              rules={[{ required: true, message: 'Please select employee status' }]}
            >
              <Select
                placeholder="Select employee status"
                className={styles.status}
                options={employeeStatusOptions}
                showSearch
                filterOption={(input, option) =>
                  (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
                }
              />
            </Form.Item>
          </Col>
        </Row>

        <Row gutter={16}>
          <Col span={8}>
            <Form.Item
              name="experience"
              label="Service Experience (Years)"
              rules={[
                { required: true, message: 'Please enter service experience' },
                {
                  pattern: /^[0-9]+$/,
                  message: 'Please enter a valid number'
                }
              ]}
            >
              <Input 
                placeholder="Years of experience" 
                className={styles.service}
                type="number"
                min={0}
                max={50}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="phone"
              label="Office Phone (Optional)"
              rules={[
                {
                  pattern: /^[0-9]{10}$/,
                  message: 'Please enter a valid 10-digit phone number'
                }
              ]}
            >
              <Input 
                placeholder="Office phone number" 
                className={styles.office}
                maxLength={10}
              />
            </Form.Item>
          </Col>
          
          <Col span={8}>
            <Form.Item
              name="emailid"
              label="Email ID"
              rules={[
                { required: true, message: 'Please enter email address' },
                { type: 'email', message: 'Please enter a valid email address' }
              ]}
            >
              <Input 
                placeholder="Your email address" 
                className={styles.mail}
              />
            </Form.Item>
          </Col>
        </Row>

        <Row>
          <Col span={24}>
            <Form.Item
              name="address"
              label="Office Address"
              rules={[{ required: true, message: 'Please enter office address' }]}
            >
              <Input.TextArea 
                placeholder="Complete office address" 
                className={styles.officeAddress}
                rows={3}
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

export default Employment;