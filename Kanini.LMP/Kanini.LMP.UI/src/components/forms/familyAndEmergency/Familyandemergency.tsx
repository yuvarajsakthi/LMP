import React from 'react';
import { Input, Form, Space, Radio, Button, Divider, Card, message } from 'antd';
import { NextButtonArrow } from '../../../assets';
import styles from './Familyandemergency.module.css';

interface FamilyEmergencyProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

interface FamilyEmergencyForm {
  spouse?: string;
  profession?: string;
  organization?: string;
  mobile1?: string;
  contact?: string;
  emailid: string;
  address1: string;
  radiogroup: string;
  name: string;
  relationship: string;
  mobile2: string;
  address2: string;
}

const Familyandemergency: React.FC<FamilyEmergencyProps> = ({ onNext, onPrevious }) => {
  const [form] = Form.useForm<FamilyEmergencyForm>();

  const handleSubmit = async (values: FamilyEmergencyForm) => {
    try {
      console.log('Family and Emergency details:', values);
      message.success('Family and emergency details saved successfully');
      onNext?.();
    } catch (error) {
      message.error('Failed to save details');
    }
  };

  const handleBack = () => {
    onPrevious?.();
  };


  return (
    <Card className={styles.container}>
      <div className={styles.header}>
        <h3>Family & Emergency Details</h3>
        <p>Enter your family and emergency contact details</p>
      </div>

      <Form form={form} onFinish={handleSubmit} layout="vertical" className={styles.form}>
        <div className={styles.section}>
          <h4 className={styles.sectionTitle}>Family Details</h4>
          
          <div className={styles.formRow}>
            <Form.Item name="spouse" className={styles.formItem}>
              <Input placeholder="Spouse's Name" />
            </Form.Item>
            <Form.Item name="profession" className={styles.formItem}>
              <Input placeholder="Profession" />
            </Form.Item>
            <Form.Item name="organization" className={styles.formItem}>
              <Input placeholder="Name Of Organization" />
            </Form.Item>
          </div>

          <div className={styles.formRow}>
            <Form.Item
              name="mobile1"
              className={styles.formItem}
              rules={[
                {
                  pattern: /^\d{10}$/,
                  message: 'Please enter a valid 10-digit mobile number'
                }
              ]}
            >
              <Input placeholder="Mobile No" />
            </Form.Item>
            <Form.Item
              name="contact"
              className={styles.formItem}
              rules={[
                {
                  pattern: /^\d{10}$/,
                  message: 'Please enter a valid 10-digit contact number'
                }
              ]}
            >
              <Input placeholder="Office Contact No" />
            </Form.Item>
            <Form.Item
              name="emailid"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Please enter email address' },
                { type: 'email', message: 'Please enter a valid email address' }
              ]}
            >
              <Input placeholder="Email ID*" />
            </Form.Item>
          </div>

          <Form.Item
            name="address1"
            rules={[{ required: true, message: 'Office Address is required' }]}
          >
            <Input placeholder="Office Address*" />
          </Form.Item>

          <div className={styles.radioSection}>
            <span className={styles.radioLabel}>Is spouse applying jointly?</span>
            <Form.Item name="radiogroup" initialValue="No">
              <Radio.Group>
                <Radio value="Yes">Yes</Radio>
                <Radio value="No">No</Radio>
              </Radio.Group>
            </Form.Item>
          </div>
        </div>

        <Divider />

        <div className={styles.section}>
          <h4 className={styles.sectionTitle}>Emergency Contact</h4>
          
          <div className={styles.formRow}>
            <Form.Item
              name="name"
              className={styles.formItem}
              rules={[{ required: true, message: 'Name is required' }]}
            >
              <Input placeholder="Name*" />
            </Form.Item>
            <Form.Item
              name="relationship"
              className={styles.formItem}
              rules={[{ required: true, message: 'Relationship is required' }]}
            >
              <Input placeholder="Relationship with Applicant*" />
            </Form.Item>
            <Form.Item
              name="mobile2"
              className={styles.formItem}
              rules={[
                { required: true, message: 'Mobile number is required' },
                {
                  pattern: /^\d{10}$/,
                  message: 'Please enter a valid 10-digit mobile number'
                }
              ]}
            >
              <Input placeholder="Mobile No*" />
            </Form.Item>
          </div>

          <Form.Item
            name="address2"
            rules={[{ required: true, message: 'Address is required' }]}
          >
            <Input placeholder="Address*" />
          </Form.Item>
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

export default Familyandemergency;
              



