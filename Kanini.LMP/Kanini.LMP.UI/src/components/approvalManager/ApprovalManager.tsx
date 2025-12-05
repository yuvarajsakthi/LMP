import React, { useState, useEffect } from 'react';
import { Button, Card, Divider, Drawer, Slider, message, Steps, Space } from 'antd';
import { ClockCircleOutlined, DownOutlined } from '@ant-design/icons';
import styles from './ApprovalManager.module.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../../context';
import { MANAGER_ROUTES } from '../../config';
import {
  LoanApplied,
  Review,
  DocumentVerificationI,
  DocumentVerificationII,
  Decision,
  KycEligibility,
  LoanDefault,
  OverallLoan,
  GreenTick,
  XMark,
  ProfileImage
} from '../../assets';

interface CustomerData {
  userId: string;
  customerId: string;
  customerName: string;
  loanId: string;
  occupation: string;
  annualIncome: number;
  age: number;
  showPhoto: boolean;
  expanded?: boolean;
}

interface DocumentImage {
  link: string;
}

interface ApprovalManagerProps {
  applicationNumber?: string;
}

const ApprovalManager: React.FC<ApprovalManagerProps> = ({ applicationNumber }) => {
  const navigate = useNavigate();
  const { token } = useAuth();
  
  const [pendingUsers, setPendingUsers] = useState<CustomerData[]>([]);
  const [amount, setAmount] = useState<number>(0);
  const [years, setYears] = useState<number>(1);
  const [interest, setInterest] = useState<number>(1);
  const [images, setImages] = useState<DocumentImage[]>([]);
  const [showKycScore, setShowKycScore] = useState(false);
  const [showLoanScore, setShowLoanScore] = useState(false);
  const [showOverallScore, setShowOverallScore] = useState(false);
  const [open, setOpen] = useState(false);

  const selectedApplicationNumber = applicationNumber || '12345';

  useEffect(() => {
    getData();
    getImages();
  }, []);

  const getData = async () => {
    try {
      const res = await axios.get(`/customer/CustomerDetails/${selectedApplicationNumber}`, {
        headers: { Authorization: `Bearer ${token}` }
      });

      const changedData = res.data.map((item: any) => {
        setAmount(item.requestedLoanAmount);
        setYears(item.requestedTenure);
        
        const birthDate = new Date(item.dob);
        const currentDate = new Date();
        let age = currentDate.getFullYear() - birthDate.getFullYear();
        
        if (
          currentDate.getMonth() < birthDate.getMonth() ||
          (currentDate.getMonth() === birthDate.getMonth() && currentDate.getDate() < birthDate.getDate())
        ) {
          age--;
        }

        return {
          userId: item.customerId,
          customerId: item.customerId,
          customerName: item.fullname,
          loanId: selectedApplicationNumber,
          occupation: item.occupation,
          annualIncome: item.annualIncome,
          age,
          showPhoto: true,
        };
      });
      
      setPendingUsers(changedData);
    } catch (error) {
      console.error('Failed to fetch customer data:', error);
    }
  };

  const getImages = async () => {
    try {
      const res = await axios.get(`/loanDocuments/loan-documents-by-id/${selectedApplicationNumber}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      
      const output = res.data.map((item: any) => ({
        link: item.documentLink,
      }));
      
      setImages(output);
    } catch (error) {
      console.error('Failed to fetch documents:', error);
    }
  };

  const handleExpandCard = (userId: string) => {
    setPendingUsers((prevUsers) =>
      prevUsers.map((user) =>
        user.userId === userId ? { ...user, expanded: !user.expanded } : user
      )
    );
  };

  const updateLoan = async (statusId: number) => {
    try {
      const updateData = {
        ApprovedTenure: years,
        ApprovedDate: new Date().toISOString(),
        ApprovedAmount: amount,
        Interest: interest,
        StatusId: statusId,
      };

      await axios.put(`/loan/loan-approval/${selectedApplicationNumber}`, updateData, {
        headers: { Authorization: `Bearer ${token}` }
      });

      if (statusId === 2) {
        message.success("Loan approved successfully");
      } else {
        message.error("Loan rejected successfully");
      }
      
      navigate(MANAGER_ROUTES.APPLIED_LOAN);
    } catch (error) {
      message.error("Error in updating loan status");
      console.error('Update failed:', error);
    }
  };

  const steps = [
    { title: 'Loan Applied', icon: <img src={LoanApplied} alt="Loan Applied" className={styles.stepIcon} /> },
    { title: 'Review', icon: <img src={Review} alt="Review" className={styles.stepIcon} /> },
    { title: 'Document Verification I', icon: <img src={DocumentVerificationI} alt="Doc Verification I" className={styles.stepIcon} /> },
    { title: 'Document Verification II', icon: <img src={DocumentVerificationII} alt="Doc Verification II" className={styles.stepIcon} /> },
    { title: 'Decision', icon: <img src={Decision} alt="Decision" className={styles.stepIcon} /> },
  ];

  return (
    <div className={styles.mainContainer}>
      <div className={styles.approvalContainer}>
        <div className={styles.header}>
          <h2>Customer Details for Loan Approval</h2>
        </div>
        
        <Divider />

        {pendingUsers.map((user) => (
          <div key={user.userId} className={styles.userCard}>
            <div className={styles.userHeader}>
              <div className={styles.userInfo}>
                <img 
                  src={ProfileImage} 
                  alt="Profile" 
                  className={styles.profileImage}
                />
                <div>
                  <div className={styles.userName}>
                    {user.customerName}
                    <DownOutlined
                      className={styles.expandIcon}
                      onClick={() => handleExpandCard(user.userId)}
                    />
                  </div>
                  <p className={styles.customerId}>Customer ID: {user.customerId}</p>
                </div>
              </div>
            </div>

            {user.expanded && (
              <div className={styles.expandedContent}>
                <Card className={styles.detailsCard}>
                  <div className={styles.customerDetails}>
                    <h3>Customer Information</h3>
                    <div className={styles.detailsGrid}>
                      <div className={styles.detailItem}>
                        <span className={styles.label}>Occupation:</span>
                        <span>{user.occupation}</span>
                      </div>
                      <div className={styles.detailItem}>
                        <span className={styles.label}>Annual Income:</span>
                        <span>₹{user.annualIncome.toLocaleString()}</span>
                      </div>
                      <div className={styles.detailItem}>
                        <span className={styles.label}>Age:</span>
                        <span>{user.age} years</span>
                      </div>
                    </div>
                  </div>

                  <Divider />

                  <div className={styles.actions}>
                    <Space>
                      <Button onClick={() => setOpen(true)}>
                        View Documents
                      </Button>
                      <Button onClick={() => setOpen(true)}>
                        Revise Amount
                      </Button>
                      <Button 
                        type="primary" 
                        className={styles.approveButton}
                        onClick={() => updateLoan(2)}
                      >
                        <img src={GreenTick} alt="Approve" className={styles.buttonIcon} />
                        Approve
                      </Button>
                      <Button 
                        danger 
                        className={styles.rejectButton}
                        onClick={() => updateLoan(3)}
                      >
                        <img src={XMark} alt="Reject" className={styles.buttonIcon} />
                        Reject
                      </Button>
                    </Space>
                  </div>
                </Card>
              </div>
            )}
          </div>
        ))}

        <Card className={styles.stepperCard}>
          <Steps 
            current={4} 
            className={styles.steps}
            items={steps.map((step, index) => ({
              key: index,
              title: step.title,
              icon: step.icon,
              description: (
                <div className={styles.stepDescription}>
                  Completed <ClockCircleOutlined /> May 15, 2020
                </div>
              )
            }))}
          />
        </Card>

        <div className={styles.scoreCards}>
          <Card className={styles.scoreCard}>
            <div className={styles.scoreContent}>
              <img src={KycEligibility} alt="KYC Eligibility" className={styles.scoreIcon} />
              <h5 className={styles.scoreTitle}>KYC Eligibility Score</h5>
              {showKycScore ? (
                <p className={styles.scoreValue}>55%</p>
              ) : (
                <Button 
                  type="link" 
                  onClick={() => setShowKycScore(true)}
                  className={styles.checkButton}
                >
                  Check Eligibility
                </Button>
              )}
            </div>
          </Card>

          <Card className={styles.scoreCard}>
            <div className={styles.scoreContent}>
              <img src={LoanDefault} alt="Loan Default" className={styles.scoreIcon} />
              <h5 className={styles.scoreTitle}>Loan Default Score</h5>
              {showLoanScore ? (
                <p className={styles.scoreValue}>24%</p>
              ) : (
                <Button 
                  type="link" 
                  onClick={() => setShowLoanScore(true)}
                  className={styles.checkButton}
                >
                  Check Probability
                </Button>
              )}
            </div>
          </Card>

          <Card className={styles.scoreCard}>
            <div className={styles.scoreContent}>
              <img src={OverallLoan} alt="Overall Loan" className={styles.scoreIcon} />
              <h5 className={styles.scoreTitle}>Overall Loan Eligibility Score</h5>
              {showOverallScore ? (
                <p className={styles.scoreValue}>15%</p>
              ) : (
                <Button 
                  type="link" 
                  onClick={() => setShowOverallScore(true)}
                  className={styles.checkButton}
                >
                  Check Score
                </Button>
              )}
            </div>
          </Card>
        </div>

        <Drawer
          title="Loan Details"
          placement="right"
          onClose={() => setOpen(false)}
          open={open}
          width={400}
        >
          <div className={styles.drawerContent}>
            <div className={styles.amountDisplay}>
              <h3>Application Amount</h3>
              <div className={styles.amountValue}>₹{amount.toLocaleString()}</div>
            </div>

            <div className={styles.sliderContainer}>
              <div className={styles.sliderItem}>
                <label>Loan Amount</label>
                <Slider
                  min={0}
                  max={1000000}
                  step={100000}
                  value={amount}
                  onChange={setAmount}
                  tooltip={{ formatter: (value) => `₹${value?.toLocaleString()}` }}
                />
                <div className={styles.rangeDisplay}>
                  <span>₹0</span>
                  <span>₹10,00,000</span>
                </div>
              </div>

              <div className={styles.sliderItem}>
                <label>Loan Tenure</label>
                <Slider
                  min={1}
                  max={20}
                  value={years}
                  onChange={setYears}
                  tooltip={{ formatter: (value) => `${value} years` }}
                />
                <div className={styles.rangeDisplay}>
                  <span>1 year</span>
                  <span>20 years</span>
                </div>
              </div>

              <div className={styles.sliderItem}>
                <label>Interest Rate</label>
                <Slider
                  min={0}
                  max={20}
                  step={0.1}
                  value={interest}
                  onChange={setInterest}
                  tooltip={{ formatter: (value) => `${value}%` }}
                />
                <div className={styles.rangeDisplay}>
                  <span>0%</span>
                  <span>20%</span>
                </div>
              </div>
            </div>

            <Button 
              type="primary" 
              className={styles.updateButton}
              onClick={() => setOpen(false)}
            >
              UPDATE
            </Button>

            <Divider />

            <div className={styles.documentsContainer}>
              <h4>Documents</h4>
              {images.map((image, index) => (
                <div key={index} className={styles.documentItem}>
                  <img 
                    src={image.link} 
                    alt={`Document ${index + 1}`}
                    className={styles.documentImage}
                  />
                </div>
              ))}
            </div>
          </div>
        </Drawer>
      </div>
    </div>
  );
};

export default ApprovalManager;