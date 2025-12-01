import React, { useState, useEffect } from 'react';
import { Form, Button, Input, DatePicker, Select, Card, message } from 'antd';
import { NextButtonArrow } from '../../../assets';
import styles from './PersonalInformationPage.module.css';

interface PersonalInformationProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

interface PersonalInformationForm {
  fullname: string;
  dob: string;
  district?: string;
  country: string;
  taxid: string;
  education: string;
  resident: string;
  residing: string;
}

interface DistrictOption {
  label: string;
  value: string;
}

const PersonalInformationPage: React.FC<PersonalInformationProps> = ({ onNext, onPrevious }) => {
  const [form] = Form.useForm<PersonalInformationForm>();
  const [districtOptions, setDistrictOptions] = useState<DistrictOption[]>([]);

  useEffect(() => {
    const mockDistricts = [
      { label: 'Mumbai', value: 'mumbai' },
      { label: 'Delhi', value: 'delhi' },
      { label: 'Bangalore', value: 'bangalore' },
      { label: 'Chennai', value: 'chennai' },
      { label: 'Kolkata', value: 'kolkata' }
    ];
    setDistrictOptions(mockDistricts);
  }, []);

  const handleSubmit = async (values: PersonalInformationForm) => {
    try {
      console.log('Personal information:', values);
      message.success('Personal information saved successfully');
      onNext?.();
    } catch (error) {
      message.error('Failed to save personal information');
    }
  };

  const handleBack = () => {
    onPrevious?.();
  };

  const validateFullName = (_: any, value: string) => {
    const nameRegex = /^[A-Za-z\s]+$/;
    if (value && !nameRegex.test(value)) {
      return Promise.reject('Please enter valid name!');
    }
    return Promise.resolve();
  };

  const validateTaxIdNumber = (_: any, value: string) => {
    const taxIdNumberRegex = /^[0-9]+$/;
    if (value && !taxIdNumberRegex.test(value)) {
      return Promise.reject('Enter valid Tax ID');
    }
    return Promise.resolve();
  };

  return (
    <Card className={styles.container}>
      <div className={styles.header}>
        <h2>Personal Information</h2>
        <p>Enter your personal information as mentioned in IDs</p>
      </div>

      <Form form={form} onFinish={handleSubmit} layout="vertical" className={styles.form}>
        <div className={styles.formRow}>
          <Form.Item
            name="fullname"
            label="Full Name"
            help="Same as ID Proof"
            className={styles.formItem}
            rules={[
              { required: true, message: 'Full Name is required!' },
              { validator: validateFullName }
            ]}
          >
            <Input placeholder="Enter full name" />
          </Form.Item>

          <Form.Item
            name="dob"
            label="Date of Birth"
            className={styles.formItem}
            rules={[{ required: true, message: 'Date of Birth is required' }]}
          >
            <DatePicker placeholder="Select date of birth" style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="district"
            label="District of Birth"
            className={styles.formItem}
          >
            <Select
              showSearch
              placeholder="Select district of birth"
              options={districtOptions}
              filterOption={(input, option) =>
                (option?.label ?? '').toLowerCase().includes(input.toLowerCase())
              }
            />
          </Form.Item>
        </div>

        <div className={styles.formRow}>
          <Form.Item
            name="country"
            label="Country of Birth"
            className={styles.formItem}
            rules={[{ required: true, message: 'Country of birth is required!' }]}
          >
            <Select
              placeholder="Select country of birth"
              options={[{ value: 'india', label: 'India' }]}
            />
          </Form.Item>

          <Form.Item
            name="taxid"
            label="Tax ID Number (eTIN)"
            className={styles.formItem}
            rules={[
              { required: true, message: 'Tax ID is required!' },
              { validator: validateTaxIdNumber }
            ]}
          >
            <Input placeholder="Enter tax ID number" />
          </Form.Item>

          <Form.Item
            name="education"
            label="Educational Qualification"
            className={styles.formItem}
            rules={[{ required: true, message: 'Educational qualification is required!' }]}
          >
            <Select
              placeholder="Select educational qualification"
              options={[
                { value: 'student', label: 'Student' },
                { value: 'degree', label: 'Degree Holder' },
                { value: 'highschool', label: 'High School Education' }
              ]}
            />
          </Form.Item>
        </div>

        <div className={styles.formRow}>
          <Form.Item
            name="resident"
            label="Residential Status"
            className={styles.formItem}
            rules={[{ required: true, message: 'Residential status is required!' }]}
          >
            <Select
              placeholder="Select residential status"
              options={[
                { value: 'citizen', label: 'Citizen' },
                { value: 'permanent resident', label: 'Permanent Resident' },
                { value: 'temporary Resident', label: 'Temporary Resident' },
                { value: 'refugee', label: 'Refugee' },
                { value: 'military personnel', label: 'Military Personnel' }
              ]}
            />
          </Form.Item>

          <Form.Item
            name="residing"
            label="Residing For (In Years)"
            className={styles.formItem}
            rules={[
              { required: true, message: 'Residing years is required!' },
              { pattern: /^[0-9]+$/, message: 'Enter valid residing years' }
            ]}
          >
            <Input placeholder="Enter residing years" />
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

export default PersonalInformationPage;