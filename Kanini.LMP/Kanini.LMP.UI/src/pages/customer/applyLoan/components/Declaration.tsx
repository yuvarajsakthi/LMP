import React from 'react';
import { Form, Checkbox, Button, Typography, Card, message } from 'antd';
import { useLoanApplication } from '../context/LoanApplicationContext';
import { loanAPI } from '../../../../services/api/loanAPI';
import { useAuth } from '../../../../context';

const { Title, Paragraph } = Typography;

const Declaration: React.FC = () => {
  const { state, dispatch } = useLoanApplication();
  const { token } = useAuth();
  const [form] = Form.useForm();

  const handleSubmit = async (values: any) => {
    if (!values.declaration) {
      message.error('Please accept the declaration to proceed');
      return;
    }

    try {
      // Prepare the complete application data
      const applicationData = {
        loanProductType: 'PersonalLoan',
        submissionDate: new Date().toISOString().split('T')[0],
        loanDetails: {
          ...state.formData.loanDetails,
          appliedDate: new Date().toISOString()
        },
        personalDetails: state.formData.personalDetails,
        addressInformation: state.formData.addressInformation,
        familyEmergencyDetails: state.formData.familyEmergencyDetails,
        employmentDetails: state.formData.employmentDetails,
        financialInformation: state.formData.financialInformation,
        declaration: { accepted: true, acceptedAt: new Date().toISOString() }
      };

      // Submit to backend
      const customerId = token?.customerId || 1; // Get from token
      await loanAPI.submitPersonalLoan(customerId, applicationData);
      
      message.success('Loan application submitted successfully!');
      dispatch({ type: 'NEXT_STEP' });
    } catch (error) {
      message.error('Failed to submit application. Please try again.');
    }
  };

  const handlePrev = () => {
    dispatch({ type: 'PREV_STEP' });
  };

  return (
    <Card>
      <Title level={4}>Declaration</Title>
      <Form form={form} layout="vertical" onFinish={handleSubmit}>
        <Paragraph>
          I hereby declare that the information provided in this loan application is true and correct to the best of my knowledge. 
          I understand that any false information may result in the rejection of my application or cancellation of the loan.
        </Paragraph>
        
        <Paragraph>
          I authorize the bank to verify the information provided and to obtain my credit report from credit bureaus.
        </Paragraph>

        <Form.Item
          name="declaration"
          valuePropName="checked"
          rules={[{ required: true, message: 'Please accept the declaration' }]}
        >
          <Checkbox>
            I accept the terms and conditions and declare that all information provided is accurate.
          </Checkbox>
        </Form.Item>

        <Form.Item>
          <Button onClick={handlePrev} style={{ marginRight: 8 }}>
            Previous
          </Button>
          <Button type="primary" htmlType="submit">
            Submit Application
          </Button>
        </Form.Item>
      </Form>
    </Card>
  );
};

export default Declaration;