import React, { useEffect } from 'react';
import { Form, Button, Input, DatePicker, Select, Card, message, Radio } from 'antd';
import { NextButtonArrow } from '../../../../assets';
import { useLoanApplication, useAuth } from '../../../../context';
import { useAppDispatch, useAppSelector } from '../../../../hooks';
import { fetchCustomerById } from '../../../../store/slices/customerSlice';
import { Gender, EducationQualification, ResidentialStatus } from '../../../../types/loanApplicationCreate';
import styles from './PersonalInformationPage.module.css';
import dayjs from 'dayjs';

interface PersonalInformationProps {
  onNext?: () => void;
  onPrevious?: () => void;
}

const PersonalInformationPage: React.FC<PersonalInformationProps> = ({ onNext, onPrevious }) => {
  const { state, dispatch } = useLoanApplication();
  const { token } = useAuth();
  const reduxDispatch = useAppDispatch();
  const { currentCustomer } = useAppSelector((state) => state.customer);
  const [form] = Form.useForm();

  useEffect(() => {
    const customerId = parseInt(token?.CustomerId || token?.customerId || '0');
    if (customerId) {
      reduxDispatch(fetchCustomerById(customerId));
    }
  }, [token, reduxDispatch]);

  useEffect(() => {
    if (state.formData.personalDetails) {
      const data = state.formData.personalDetails;
      form.setFieldsValue({
        ...data,
        dateOfBirth: data.dateOfBirth ? dayjs(data.dateOfBirth) : null
      });
    } else if (currentCustomer) {
      const genderValue = typeof currentCustomer.gender === 'string' 
        ? (currentCustomer.gender.toLowerCase() === 'male' ? Gender.Male : Gender.Female)
        : currentCustomer.gender;
      
      form.setFieldsValue({
        fullName: token?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || token?.FullName || '',
        panNumber: currentCustomer.panNumber || '',
        dateOfBirth: currentCustomer.dateOfBirth ? dayjs(currentCustomer.dateOfBirth) : null,
        gender: genderValue
      });
    }
  }, [state.formData.personalDetails, currentCustomer, form, token]);

  const handleSubmit = async (values: any) => {
    try {
      const mappedData = {
        ...state.formData.personalDetails,
        fullName: values.fullName,
        dateOfBirth: values.dateOfBirth ? values.dateOfBirth.format('YYYY-MM-DD') : '',
        districtOfBirth: values.districtOfBirth,
        panNumber: values.panNumber,
        educationQualification: values.educationQualification,
        residentialStatus: values.residentialStatus,
        gender: values.gender
      };
      dispatch({ type: 'UPDATE_FORM_DATA', payload: { section: 'personalDetails', data: mappedData } });
      message.success('Personal information saved successfully');
      onNext?.();
    } catch (error) {
      message.error('Failed to save personal information');
    }
  };

  const handleBack = () => {
    onPrevious?.();
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
            name="fullName"
            label="Full Name"
            className={styles.formItem}
            rules={[{ required: true, message: 'Full Name is required' }]}
          >
            <Input placeholder="Enter full name" />
          </Form.Item>

          <Form.Item
            name="dateOfBirth"
            label="Date of Birth"
            className={styles.formItem}
            rules={[{ required: true, message: 'Date of Birth is required' }]}
          >
            <DatePicker placeholder="Select date of birth" style={{ width: '100%' }} />
          </Form.Item>

          <Form.Item
            name="districtOfBirth"
            label="District of Birth"
            className={styles.formItem}
            rules={[{ required: true, message: 'District of Birth is required' }]}
          >
            <Input placeholder="Enter district of birth" />
          </Form.Item>
        </div>

        <div className={styles.formRow}>
          <Form.Item
            name="panNumber"
            label="PAN Number"
            className={styles.formItem}
            rules={[
              { required: true, message: 'PAN Number is required' },
              { pattern: /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/, message: 'Enter valid PAN (e.g., ABCDE1234F)' }
            ]}
          >
            <Input placeholder="Enter PAN number" maxLength={10} style={{ textTransform: 'uppercase' }} />
          </Form.Item>

          <Form.Item
            name="educationQualification"
            label="Educational Qualification"
            className={styles.formItem}
            rules={[{ required: true, message: 'Educational qualification is required' }]}
          >
            <Select placeholder="Select educational qualification">
              <Select.Option value={EducationQualification.BelowMatric}>Below Matric</Select.Option>
              <Select.Option value={EducationQualification.Matric}>Matric</Select.Option>
              <Select.Option value={EducationQualification.HigherSecondary}>Higher Secondary</Select.Option>
              <Select.Option value={EducationQualification.Graduate}>Graduate</Select.Option>
              <Select.Option value={EducationQualification.PostGraduate}>Post Graduate</Select.Option>
              <Select.Option value={EducationQualification.Doctorate}>Doctorate</Select.Option>
              <Select.Option value={EducationQualification.Other}>Other</Select.Option>
            </Select>
          </Form.Item>

          <Form.Item
            name="residentialStatus"
            label="Residential Status"
            className={styles.formItem}
            rules={[{ required: true, message: 'Residential status is required' }]}
          >
            <Select placeholder="Select residential status">
              <Select.Option value={ResidentialStatus.Owned}>Owned</Select.Option>
              <Select.Option value={ResidentialStatus.Rented}>Rented</Select.Option>
              <Select.Option value={ResidentialStatus.Parental}>Parental</Select.Option>
              <Select.Option value={ResidentialStatus.CompanyProvided}>Company Provided</Select.Option>
              <Select.Option value={ResidentialStatus.PayingGuest}>Paying Guest</Select.Option>
              <Select.Option value={ResidentialStatus.Other}>Other</Select.Option>
            </Select>
          </Form.Item>
        </div>

        <div className={styles.formRow}>
          <Form.Item
            name="gender"
            label="Gender"
            className={styles.formItem}
            rules={[{ required: true, message: 'Gender is required' }]}
          >
            <Radio.Group>
              <Radio value={Gender.Male}>Male</Radio>
              <Radio value={Gender.Female}>Female</Radio>
            </Radio.Group>
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