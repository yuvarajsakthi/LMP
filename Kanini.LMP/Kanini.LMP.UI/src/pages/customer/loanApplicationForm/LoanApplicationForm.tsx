import { useState } from 'react';
import { Col, Row, Space } from 'antd';
import { Link } from 'react-router-dom';
import { 
  OneBlue, TwoGrey, ThreeGrey, FourGrey, FiveGrey, SixGrey,
  TwoBlue, ThreeBlue, FourBlue, FiveBlue, SixBlue,
  GreenTick, SideMenuTriangle, LoanDetailsAndFinancial,
  SideMenuArrow, ArrowForm, UserIcon
} from '../../../assets';
import './Stepper.css';

// Import form components with proper paths
import LoanDetails from '../../../components/forms/loanDetails/LoanDetails';
import LoanDocuments from '../../../components/forms/loanDocuments/LoanDocuments';
import PersonalInformationPage from '../../../components/forms/personalInformationPage/PersonalInformationPage';
import Addressinformation from '../../../components/forms/addressInformation/Addressinformation';
import Familyandemergency from '../../../components/forms/familyAndEmergency/Familyandemergency';
import Employement from '../../../components/forms/employement/Employement';
import FinancialInfo from '../../../components/forms/financialInfo/FinancialInfo';
import Declaration from '../../../components/forms/declaration/Declaration';
import Summary from '../applyLoan/components/Summary';
import { LoanApplicationProvider } from '../applyLoan/context/LoanApplicationContext';
import Layout from '../../../layout/Layout';

interface FormData {
  amount: number | null;
  tenure: number | null;
  fullname: string | null;
}

function LoanApplicationFormContent() {
  const [isHiddenVisible, setHiddenVisible] = useState(false);
  const [uploadDocumentsImage] = useState(TwoGrey);
  const [Employeeimg] = useState(FourGrey);
  const [Pesonalimg] = useState(ThreeGrey);
  const [Financialimg] = useState(FiveGrey);
  const [uploadDocumentsTextColor] = useState('#676D74');
  const [PersonalTextColor] = useState('#676D74');
  const [EmploymentTextColor] = useState('#676D74');
  const [FinancialTextColor] = useState('#676D74');
  const [DeclarationTextColor] = useState('#676D74');
  const [col1ImageContent] = useState(LoanDetailsAndFinancial);

  const [step, setStep] = useState(1);

  const [formDataOne] = useState<FormData>({
    amount: null,
    tenure: null,
    fullname: null
  });

  const toggleHidden = () => {
    setHiddenVisible(!isHiddenVisible);
  };



  const RenderFormComponent = () => {
    const handleNext = () => {
      setStep((prevStep) => prevStep + 1);
    };

    const handleBack = () => {
      setStep((prevStep) => prevStep - 1);
    };

    switch (step) {
      case 1:
        return <LoanDetails onNext={handleNext} />;
      case 2:
        return <LoanDocuments onNext={handleNext} onPrevious={handleBack} />;
      case 3:
        return <PersonalInformationPage onNext={handleNext} onPrevious={handleBack} />;
      case 4:
        return <Addressinformation 
          loanId="1" 
          onNext={handleNext} 
          onPrev={handleBack} 
          onNextStep={handleNext} 
          onBackStep={handleBack} 
        />;
      case 5:
        return <Familyandemergency onNext={handleNext} onPrevious={handleBack} />;
      case 6:
        return <Employement 
          loanId="1" 
          onNext={handleNext} 
          onPrev={handleBack} 
          onNextStep={handleNext} 
          onBackStep={handleBack} 
        />;
      case 7:
        return <FinancialInfo onNext={handleNext} onPrevious={handleBack} />;
      case 8:
        return <Declaration 
          loanId="1" 
          fullname={formDataOne.fullname || "User"} 
          amount={formDataOne.amount || 0} 
          onBackStep={handleBack} 
          onNext={handleNext} 
          onPrev={handleBack} 
        />;
      default:
        return null;
    }
  }

  return (
      <Layout>
      <div className='Integratebody' style={{ backgroundColor: '#F6F8FC', minHeight: '100vh', padding: '10px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ display: 'flex' }}>
            <p style={{
              color: '#676D74',
              backgroundImage: `url(${UserIcon})`,
              backgroundPosition: 'left center',
              backgroundRepeat: 'no-repeat',
              paddingLeft: '25px',
              marginTop: '10px',
              marginLeft: '10px',
              marginBottom: '20px',
              fontSize: '18px',
              fontWeight: '500px',
            }}>Personal Loan</p>
          </div>
          <div style={{ display: 'flex' }}>
            <Link to="/customer-dashboard" style={{
              color: '#2C76C9',
              backgroundImage: `url(${ArrowForm})`,
              backgroundPosition: 'left center',
              backgroundRepeat: 'no-repeat',
              marginTop: '10px',
              marginBottom: '20px',
              fontSize: '18px',
              fontWeight: '500px',
              paddingLeft: '25px',
              cursor: 'pointer',
              textDecoration: 'none'
            }}>
              Go back to DashBoard
            </Link>
          </div>
        </div>

        <div style={{ padding: '0 20px' }}>
          <Row gutter={16} style={{ width: '100%' }}>
            <Col xs={24} sm={24} md={6} lg={5} xl={5} style={{ backgroundColor: 'white', padding: '30px', borderRadius: '8px', position: 'relative' }}>
              <div>
                <div className="loanDetailsContainer" style={{ marginTop: '20px' }}>
                  <Space>
                    <img
                      src={step === 1 ? OneBlue : GreenTick}
                      alt="1"
                      className="blueNumberIcon"
                    />
                    <p className="loanDetailsText" style={{ color: step === 1 ? '#2C76C9' : ' #BBBBBB', marginTop: '15px', fontWeight: '500', font: '100', fontSize: '20px' }}>
                      Loan Details
                    </p>
                    {step === 1 && <img src={SideMenuArrow} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img
                      src={step === 2 ? TwoBlue : (step > 2 ? GreenTick : uploadDocumentsImage)}
                      alt="2"
                    />
                    <p style={{ color: step === 2 ? '#2C76C9' : (step > 2 ? '#BBBBBB' : uploadDocumentsTextColor), marginTop: '15px', fontSize: '20px' }}>Upload Documents</p>
                    {step === 2 && <img src={SideMenuArrow} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space onClick={toggleHidden}>
                    <img
                      src={step >= 3 && step <= 5 ? ThreeBlue : (step > 5 ? GreenTick : Pesonalimg)}
                      alt="3"
                    />
                    <p style={{ color: step >= 3 && step <= 5 ? '#2C76C9' : (step > 5 ? '#BBBBBB' : PersonalTextColor), fontSize: '20px', marginTop: '15px' }}>Personal Details</p>
                  </Space>
                </div>

                {isHiddenVisible && (
                  <div id="Hidden" >
                    <p className="hide1" style={{ color: step === 3 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }}>
                      Personal Information
                      {step === 3 && <img src={SideMenuArrow} alt="Arrow" className='SubRightArrow' />}
                    </p>
                    <p className="hide2" style={{ color: step === 4 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }}>
                      Address Information
                      {step === 4 && <img src={SideMenuArrow} alt="Arrow" className='SubRightArrow' />}
                    </p>
                    <p className="hide3" style={{ color: step === 5 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }} >
                      Family & Emergency Details
                      {step === 5 && <img src={SideMenuArrow} alt="Arrow" className='SubRightArrow' />}
                    </p>
                  </div>
                )}

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={step === 6 ? FourBlue : (step > 6 ? GreenTick : Employeeimg)}></img>
                    <p style={{ color: step === 6 ? '#2C76C9' : (step > 6 ? '#BBBBBB' : EmploymentTextColor), fontSize: '20px', marginTop: '15px' }}>Employment Details</p>
                    {step === 6 && <img src={SideMenuArrow} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={step === 7 ? FiveBlue : (step > 7 ? GreenTick : Financialimg)}></img>
                    <p style={{ color: step === 7 ? '#2C76C9' : (step > 7 ? '#BBBBBB' : FinancialTextColor), fontSize: '20px', marginTop: '15px' }}>Financial Information</p>
                    {step === 7 && <img src={SideMenuArrow} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={step === 8 ? SixBlue : SixGrey}></img>
                    <p style={{ color: step === 8 ? '#2C76C9' : DeclarationTextColor, fontSize: '20px', marginTop: '15px' }}>Declaration</p>
                    {step === 8 && <img src={SideMenuArrow} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div className="col1images"  >
                  <div className='col1bottomImagetriangle'>
                    <img src={SideMenuTriangle} style={{ width: '500px' }} />
                  </div>
                  <div className="col1bottomImage" >
                    <img src={col1ImageContent} />
                  </div>
                </div>

              </div>
            </Col>
            <Col xs={24} sm={24} md={11} lg={13} xl={13} style={{ backgroundColor: 'white', padding: '20px', borderRadius: '8px' }}> 
              {RenderFormComponent()}
            </Col>
            <Col xs={24} sm={24} md={7} lg={6} xl={6}>
              <Summary />
            </Col>
          </Row>
        </div>

      </div >
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