import { useState, useMemo, useEffect } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Col, Row } from 'antd';
import { 
  OneBlue, TwoGrey, ThreeGrey, FourGrey,
  TwoBlue, ThreeBlue, FourBlue,
  GreenTick,
  ArrowForm, UserIcon,
} from '../../../assets';
import './Stepper.css';
import styles from './LoanApplicationForm.module.css';

import HomeLoanDetails from '../../../components/forms/homeLoan/HomeLoanDetails';
import VehicleLoanDetails from '../../../components/forms/vehicleLoan/VehicleLoanDetails';
import PersonalLoanDetails from '../../../components/forms/personalLoan/PersonalLoanDetails';
import LoanDocuments from '../../../components/forms/common/loanDocuments/LoanDocuments';
import PersonalInformationPage from '../../../components/forms/common/personalInformationPage/PersonalInformationPage';
import Addressinformation from '../../../components/forms/common/addressInformation/Addressinformation';
import Familyandemergency from '../../../components/forms/common/familyAndEmergency/Familyandemergency';
import Declaration from '../../../components/forms/common/declaration/Declaration';
import Layout from '../../../layout/Layout';
import ApplicationSummary from '../../../components/forms/common/applicationSummary/ApplicationSummary';
import { LoanApplicationProvider, useLoanApplication } from '../../../context';

function LoanApplicationFormContent() {
  const location = useLocation();
  const navigate = useNavigate();
  const selectedCategory = location.state?.selectedCategory;
  const loanType = selectedCategory?.loanProductName || 'Personal Loan';
  
  const { state, dispatch } = useLoanApplication();
  const [step, setStep] = useState(1);

  useEffect(() => {
    const loanTypeLower = loanType.toLowerCase();
    const isHome = loanTypeLower.includes('home') || loanTypeLower.includes('housing');
    const isVehicle = loanTypeLower.includes('vehicle') || loanTypeLower.includes('car') || loanTypeLower.includes('bike');
    
    if (isHome) {
      dispatch({ type: 'SET_LOAN_TYPE', payload: 'home' });
    } else if (isVehicle) {
      dispatch({ type: 'SET_LOAN_TYPE', payload: 'vehicle' });
    } else {
      dispatch({ type: 'SET_LOAN_TYPE', payload: 'personal' });
    }
  }, [loanType, dispatch]);

  const declarationData = useMemo(() => ({
    fullname: state.formData.personalDetails?.fullName || 'Applicant',
    amount: state.formData.loanDetails?.requestedLoanAmount || 0
  }), [state.formData.personalDetails?.fullName, state.formData.loanDetails?.requestedLoanAmount]);

  const handleNext = () => {
    setStep((prevStep) => prevStep + 1);
  };

  const handleBack = () => {
    setStep((prevStep) => prevStep - 1);
  };

  const RenderFormComponent = () => {
    const loanTypeLower = loanType.toLowerCase();
    const isHome = loanTypeLower.includes('home') || loanTypeLower.includes('housing');
    const isVehicle = loanTypeLower.includes('vehicle') || loanTypeLower.includes('car') || loanTypeLower.includes('bike');
    
    switch (step) {
      case 1:
        if (isHome) return <HomeLoanDetails onNext={handleNext} />;
        if (isVehicle) return <VehicleLoanDetails onNext={handleNext} />;
        return <PersonalLoanDetails onNext={handleNext} />;
      case 2:
        return <LoanDocuments onNext={handleNext} onPrevious={handleBack} />;
      case 3:
        return <PersonalInformationPage onNext={handleNext} onPrevious={handleBack} />;
      case 4:
        return <Addressinformation onNext={handleNext} onPrev={handleBack} />;
      case 5:
        return <Familyandemergency onNext={handleNext} onPrevious={handleBack} />;
      case 6:
        return <Declaration 
          loanId="1" 
          fullname={declarationData.fullname} 
          amount={declarationData.amount} 
          onBackStep={handleBack} 
          onNext={handleNext} 
          onPrev={handleBack} 
        />;
      default:
        return null;
    }
  };

  return (
    <Layout>
      <div className={styles.container}>
        <div className={styles.header}>
          <p className={styles.loanType} style={{ backgroundImage: `url(${UserIcon})` }}>
            {loanType}
          </p>
          <span className={styles.goBack} onClick={() => navigate(-1)} style={{ backgroundImage: `url(${ArrowForm})` }}>
            Go Back
          </span>
        </div>

        <div className={styles.content}>
          <div className={styles.stepper}>
            <div className={styles.stepperIcons}>
              <img src={step === 1 ? OneBlue : GreenTick} alt="1" className={styles.stepIcon} />
              <img src={step === 2 ? TwoBlue : (step > 2 ? GreenTick : TwoGrey)} alt="2" className={styles.stepIcon} />
              <img src={step >= 3 && step <= 5 ? ThreeBlue : (step > 5 ? GreenTick : ThreeGrey)} alt="3" className={styles.stepIcon} />
              <img src={step === 6 ? FourBlue : (step > 6 ? GreenTick : FourGrey)} alt="4" className={styles.stepIcon} />
            </div>
          </div>

          <Row gutter={[16, 16]} style={{ width: '100%' }}>
            <Col xs={24} sm={24} md={24} lg={18} xl={18} className={styles.formColumn}> 
              {RenderFormComponent()}
            </Col>
            
            <Col xs={24} sm={24} md={24} lg={6} xl={6}>
              <ApplicationSummary currentStep={step - 1} totalSteps={6} />
            </Col>
          </Row>
        </div>
      </div>
    </Layout>
  );
}

function LoanApplicationForm() {
  return (
    <LoanApplicationProvider>
      <LoanApplicationFormContent />
    </LoanApplicationProvider>
  );
}

export default LoanApplicationForm;
