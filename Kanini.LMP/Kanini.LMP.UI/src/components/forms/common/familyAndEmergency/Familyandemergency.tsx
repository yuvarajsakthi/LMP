import React, { useEffect } from 'react';
import { Input, Form, Button, Card, message } from 'antd';
import { NextButtonArrow } from '../../../../assets';
import { useLoanApplication } from '../../../../context';
import styles from './Familyandemergency.module.css';

interface FamilyEmergencyProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

interface FamilyEmergencyForm {
  name: string;
  relationship: string;
  mobile2: string;
  address2: string;
}

const Familyandemergency: React.FC<FamilyEmergencyProps> = ({ onNext, onPrevious }) => {
  const { state, dispatch } = useLoanApplication();
  const [form] = Form.useForm<FamilyEmergencyForm>();

  useEffect(() => {
    if (state.formData.familyEmergencyDetails) {
      const data = state.formData.familyEmergencyDetails;
      form.setFieldsValue({
        name: data.fullName,
        relationship: data.relationshipWithApplicant,
        mobile2: data.mobileNumber,
        address2: data.address
      });
    }
  }, [state.formData.familyEmergencyDetails, form]);

  const handleSubmit = async (values: FamilyEmergencyForm) => {
    try {
      // Map to backend field names
      const mappedData = {
        fullName: values.name,
        relationshipWithApplicant: values.relationship,
        mobileNumber: values.mobile2,
        address: values.address2
      };
      dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'familyEmergencyDetails', data: mappedData } });
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
          <h4 className={styles.sectionTitle}>Emergency Contact Details</h4>
          
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
              



