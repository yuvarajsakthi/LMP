import React, { useState } from 'react';
import { Steps, Button, Card, Typography, Row, Col, Modal, message } from 'antd';
import { CheckCircleOutlined, ArrowLeftOutlined } from '@ant-design/icons';
import { useNavigate, useLocation } from 'react-router-dom';
import Layout from '../../../layout/Layout';
import { loanAPI } from '../../../services/api/loanAPI';
import { useAuth } from '../../../context';

const { Title } = Typography;

const LoanApplicationForm: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { token } = useAuth();
  const [currentStep, setCurrentStep] = useState(0);
  const [formData, setFormData] = useState<any>({});
  const [loading, setLoading] = useState(false);

  const selectedCategory = location.state?.selectedCategory;

  const steps = [
    { title: 'Loan Details' },
    { title: 'Upload Documents' },
    { title: 'Personal Details' },
    { title: 'Employment Details' },
    { title: 'Financial Information' },
    { title: 'Declaration' }
  ];

  const handleNext = () => {
    if (currentStep < steps.length - 1) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handlePrev = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleSubmit = async () => {
    try {
      setLoading(true);
      
      const applicationData = {
        loanProductType: selectedCategory?.loanProductName || 'PersonalLoan',
        submissionDate: new Date().toISOString().split('T')[0],
        loanDetails: {
          requestedAmount: formData.loanDetails?.requestedAmount || 100000,
          tenureMonths: formData.loanDetails?.tenureMonths || 24,
          appliedDate: new Date().toISOString()
        },
        personalDetails: formData.personalDetails || {},
        addressInformation: formData.personalDetails?.address || {},
        familyEmergencyDetails: formData.personalDetails?.emergency || {},
        employmentDetails: formData.employmentDetails || {},
        financialInformation: formData.financialInformation || {},
        declaration: { accepted: true, acceptedAt: new Date().toISOString() }
      };

      const customerId = token?.customerId || 1;
      await loanAPI.submitPersonalLoan(customerId, applicationData);
      
      Modal.success({
        title: 'Loan Application Submitted Successfully',
        content: 'Provider will be able to review the application, make updates, sign and submit the application.',
        okText: 'Go to Dashboard',
        onOk: () => navigate('/customer-dashboard')
      });
    } catch (error) {
      message.error('Failed to submit application. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <div style={{ padding: '24px' }}>
        <Row gutter={24}>
          <Col span={6}>
            <Card>
              <Button 
                type="text" 
                icon={<ArrowLeftOutlined />}
                onClick={() => navigate('/apply-loan')}
                style={{ marginBottom: 16 }}
              >
                Personal Loan
              </Button>
              
              <Steps
                direction="vertical"
                current={currentStep}
                items={steps.map((step, index) => ({
                  title: step.title,
                  icon: index < currentStep ? <CheckCircleOutlined /> : undefined
                }))}
              />
            </Card>
          </Col>

          <Col span={12}>
            <Card>
              <Title level={4}>{steps[currentStep].title}</Title>
              <p>Step {currentStep + 1} of {steps.length}</p>
              
              <div style={{ marginTop: 24, marginBottom: 24 }}>
                {/* Placeholder content for each step */}
                <p>Form content for {steps[currentStep].title} goes here...</p>
              </div>

              <div style={{ textAlign: 'right' }}>
                {currentStep > 0 && (
                  <Button onClick={handlePrev} style={{ marginRight: 8 }}>
                    BACK
                  </Button>
                )}
                {currentStep < steps.length - 1 ? (
                  <Button type="primary" onClick={handleNext}>
                    NEXT →
                  </Button>
                ) : (
                  <Button type="primary" onClick={handleSubmit} loading={loading}>
                    SUBMIT
                  </Button>
                )}
              </div>
            </Card>
          </Col>

          <Col span={6}>
            <Card title="Summary">
              <div>
                <p><strong>Loan Amount:</strong> ₹ 72,207</p>
                <p><strong>Tenure:</strong> 72 months</p>
                <p><strong>Document uploaded:</strong> Successfully</p>
              </div>
            </Card>
          </Col>
        </Row>
      </div>
    </Layout>
  );
};

export default LoanApplicationForm;