import { useState } from 'react';
import Logo from '../../layout/logo/Logo';
import { Col, Row } from 'antd';
import number1blue from '../../assets/images/1blue.svg';
import number2 from '../../assets/images/2grey.svg';
import number3 from '../../assets/images/3grey.svg';
import number4 from '../../assets/images/4grey.svg';
import number5 from '../../assets/images/5grey.svg';
import number6 from '../../assets/images/6grey.svg';
import number2blue from '../../assets/images/2blue.svg'
import number3blue from '../../assets/images/3blue.svg'
import number4blue from '../../assets/images/4blue.svg'
import number5blue from '../../assets/images/5blue.svg'
import number6blue from '../../assets/images/6blue.svg'
import Greentick from '../../assets/images/GreenTick.svg';
import triangle from '../../assets/images/SideMenuTriangle.svg'
import loandetails from '../../assets/images/LoanDetailsandFinancial.svg'
import '../../components/Stepper/index.css';
import arrowImage from '../../assets/images/SideMenuArrow.svg';
import { Space } from 'antd';
import './index.css'
import Navbar from '../../layout/navbar/Navbar';
import arrowdas from '../../assets/images/ArrowForm.svg';
import User from '../../assets/images/UserIcon.svg';
import { Link } from 'react-router-dom';

// Import form components with proper paths
import LoanDetails from '../../components/forms/loanDetails/LoanDetails';
import LoanDocuments from '../../components/forms/loanDocuments/LoanDocuments';
import PersonalInformationPage from '../../components/forms/personalInformationPage/PersonalInformationPage';
import Addressinformation from '../../components/forms/addressInformation/Addressinformation';
import Familyandemergency from '../../components/forms/familyAndEmergency/Familyandemergency';
import Employement from '../../components/forms/employement/Employement';
import FinancialInfo from '../../components/forms/financialInfo/FinancialInfo';
import Declaration from '../../components/forms/declaration/Declaration';

interface FormData {
  amount: number | null;
  tenure: number | null;
  fullname: string | null;
}

function Integration() {
  const [isHiddenVisible, setHiddenVisible] = useState(false);
  const [steps] = useState(1);
  const [uploadDocumentsImage] = useState(number2);
  const [Employeeimg] = useState(number4);
  const [Pesonalimg] = useState(number3);
  const [Financialimg] = useState(number5);
  const [uploadDocumentsTextColor] = useState('#676D74');
  const [PersonalTextColor] = useState('#676D74');
  const [EmploymentTextColor] = useState('#676D74');
  const [FinancialTextColor] = useState('#676D74');
  const [DeclarationTextColor] = useState('#676D74');
  const [col1ImageContent] = useState(loandetails);

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
    <>
      <div style={{ display: 'flex' }}>
        <div style={{ flex: 0 }}>
          <Logo />
        </div>
        <div style={{ flex: 12 }}>
          <Navbar />
        </div>
      </div>
      <div className='Integratebody' style={{ backgroundColor: '#F6F8FC', minHeight: '100vh', padding: '10px' }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ display: 'flex' }}>
            <p style={{
              color: '#676D74',
              backgroundImage: `url(${User})`,
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
              backgroundImage: `url(${arrowdas})`,
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

        <div>
          <Row gutter={[24, 24]} style={{ width: '100%', height: '895px' }}>
            <Col xs={24} sm={24} md={6} lg={6} xl={6} style={{ backgroundColor: 'white', padding: '30px', borderRadius: '8px', height: '', marginLeft: "20px", position: 'relative' }}>
              <div>
                <div className="loanDetailsContainer" style={{ marginTop: '20px' }}>
                  <Space>
                    <img
                      src={steps === 1 ? number1blue : Greentick}
                      alt="1"
                      className="blueNumberIcon"
                    />
                    <p className="loanDetailsText" style={{ color: steps === 1 ? '#2C76C9' : ' #BBBBBB', marginTop: '15px', fontWeight: '500', font: '100', fontSize: '20px' }}>
                      Loan Details
                    </p>
                    {steps === 1 && <img src={arrowImage} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img
                      src={steps === 2 ? number2blue : uploadDocumentsImage}
                      alt="2"
                    />
                    <p style={{ color: uploadDocumentsTextColor, marginTop: '15px', fontSize: '20px' }}>Upload Documents</p>
                    {steps === 2 && <img src={arrowImage} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space onClick={toggleHidden}>
                    <img
                      src={steps >= 3 && steps <= 5 ? number3blue : Pesonalimg}
                      alt="3"
                    />
                    <p style={{ color: PersonalTextColor, fontSize: '20px', marginTop: '15px' }}>Personal Details</p>
                  </Space>
                </div>

                {isHiddenVisible && (
                  <div id="Hidden" >
                    <p className="hide1" style={{ color: steps === 3 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }}>
                      Personal Information
                      {steps === 3 && <img src={arrowImage} alt="Arrow" className='SubRightArrow' />}
                    </p>
                    <p className="hide2" style={{ color: steps === 4 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }}>
                      Address Information
                      {steps === 4 && <img src={arrowImage} alt="Arrow" className='SubRightArrow' />}
                    </p>
                    <p className="hide3" style={{ color: steps === 5 ? '#979797' : '#37414A', marginTop: '5%', fontSize: '20px', marginLeft: '55px' }} >
                      Family & Emergency Details
                      {steps === 5 && <img src={arrowImage} alt="Arrow" className='SubRightArrow' />}
                    </p>
                  </div>
                )}

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={steps === 6 ? number4blue : Employeeimg}></img>
                    <p style={{ color: EmploymentTextColor, fontSize: '20px', marginTop: '15px' }}>Employment Details</p>
                    {steps === 6 && <img src={arrowImage} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={steps === 7 ? number5blue : Financialimg}></img>
                    <p style={{ color: FinancialTextColor, fontSize: '20px', marginTop: '15px' }}>Financial Information</p>
                    {steps === 7 && <img src={arrowImage} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div style={{ marginTop: '5%' }}>
                  <Space>
                    <img src={steps === 8 ? number6blue : number6}></img>
                    <p style={{ color: DeclarationTextColor, fontSize: '20px', marginTop: '15px' }}>Declaration</p>
                    {steps === 8 && <img src={arrowImage} alt="Arrow" className='RightArrow' />}
                  </Space>
                </div>

                <div className="col1images"  >
                  <div className='col1bottomImagetriangle'>
                    <img src={triangle} style={{ width: '500px' }} />
                  </div>
                  <div className="col1bottomImage" >
                    <img src={col1ImageContent} />
                  </div>
                </div>

              </div>
            </Col>
            <Col xs={24} sm={24} md={12} lg={12} xl={11} style={{ backgroundColor: 'white', padding: '20px', borderRadius: '8px', marginLeft: "25px" }}> 
              {RenderFormComponent()}
            </Col>
            <Col xs={24} sm={24} md={6} lg={6} xl={6} style={{ backgroundColor: 'white', padding: '20px', borderRadius: '8px', marginLeft: "25px" }}>
              <div>
                <h3>Application Summary</h3>
                <p>Your loan application progress will be shown here.</p>
              </div>
            </Col>
          </Row>
        </div>

      </div >
    </>
  );
}

export default Integration;