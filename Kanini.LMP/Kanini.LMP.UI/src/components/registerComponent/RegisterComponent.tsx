import { useState, ChangeEvent } from "react";
import { Typography, Input, Button, message, Select, Steps } from "antd";
import { LoanAcceleratorLogo } from "../../assets";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import RegisterCss from "./RegisterComponent.module.css";
import { COMMON_ROUTES, ERROR_MESSAGES } from "../../config";
import { registerSchema } from "../../utils/validationSchemas";
import { registerUser, verifyOTP } from "../../store/slices/authSlice";
import { useAppDispatch, useAppSelector } from "../../hooks";
const { Title } = Typography;

const RegisterComponent = () => {
  const dispatch = useAppDispatch();
  const { isLoading } = useAppSelector((state) => state.auth);
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(0);
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [gender, setGender] = useState(0);
  const [phoneNumber, setPhoneNumber] = useState("");
  const [panNumber, setPanNumber] = useState("");
  const [aadhaarNumber, setAadhaarNumber] = useState("");
  const [annualIncome, setAnnualIncome] = useState("");
  const [homeOwnershipStatus, setHomeOwnershipStatus] = useState<number | undefined>(undefined);
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");
  const [phoneError, setPhoneError] = useState("");
  const [panError, setPanError] = useState("");
  const [aadhaarError, setAadhaarError] = useState("");
  const [showOTPVerification, setShowOTPVerification] = useState(false);
  const [otp, setOTP] = useState("");



  const handleNameChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setName(value);
    try {
      await registerSchema.validateAt('name', { name: value });
      setNameError('');
    } catch (err: any) {
      setNameError(err.message);
    }
  };

  const handleEmailChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setEmail(value);
    try {
      await registerSchema.validateAt('email', { email: value });
      setEmailError('');
    } catch (err: any) {
      setEmailError(err.message);
    }
  };

  const handlePasswordChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setPassword(value);
    try {
      await registerSchema.validateAt('password', { password: value });
      setPasswordError('');
    } catch (err: any) {
      setPasswordError(err.message);
    }
    if (confirmPassword) {
      try {
        await registerSchema.validateAt('confirmPassword', { password: value, confirmPassword });
        setConfirmPasswordError('');
      } catch (err: any) {
        setConfirmPasswordError(err.message);
      }
    }
  };

  const handleConfirmPasswordChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setConfirmPassword(value);
    try {
      await registerSchema.validateAt('confirmPassword', { password, confirmPassword: value });
      setConfirmPasswordError('');
    } catch (err: any) {
      setConfirmPasswordError(err.message);
    }
  };

  const handlePhoneChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.replace(/\D/g, '');
    setPhoneNumber(value);
    try {
      await registerSchema.validateAt('phoneNumber', { phoneNumber: value });
      setPhoneError('');
    } catch (err: any) {
      setPhoneError(err.message);
    }
  };

  const handlePanChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.toUpperCase();
    setPanNumber(value);
    try {
      await registerSchema.validateAt('panNumber', { panNumber: value });
      setPanError('');
    } catch (err: any) {
      setPanError(err.message);
    }
  };

  const handleAadhaarChange = async (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value.replace(/\D/g, '');
    setAadhaarNumber(value);
    try {
      await registerSchema.validateAt('aadhaarNumber', { aadhaarNumber: value });
      setAadhaarError('');
    } catch (err: any) {
      setAadhaarError(err.message);
    }
  };

  const validateStep = async () => {
    try {
      if (currentStep === 0) {
        await registerSchema.validateAt('name', { name });
        await registerSchema.validateAt('email', { email });
        await registerSchema.validateAt('password', { password });
        await registerSchema.validateAt('confirmPassword', { password, confirmPassword });
        return true;
      }
      if (currentStep === 1) {
        await registerSchema.validateAt('dateOfBirth', { dateOfBirth });
        await registerSchema.validateAt('phoneNumber', { phoneNumber });
        return true;
      }
      if (currentStep === 2) {
        await registerSchema.validateAt('panNumber', { panNumber });
        await registerSchema.validateAt('aadhaarNumber', { aadhaarNumber });
        await registerSchema.validateAt('annualIncome', { annualIncome: parseFloat(annualIncome) });
        return true;
      }
    } catch (err: any) {
      message.error(err.message);
      return false;
    }
    return false;
  };

  const handleNext = async () => {
    if (await validateStep()) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handlePrevious = () => {
    setCurrentStep(currentStep - 1);
  };

  const handleSubmit = async () => {
    if (!(await validateStep())) return;
    
    try {
      await dispatch(registerUser({
        fullName: name,
        email,
        password,
        dateOfBirth,
        gender,
        phoneNumber,
        panNumber,
        aadhaarNumber,
        annualIncome: parseFloat(annualIncome),
        homeOwnershipStatus
      })).unwrap();
      setShowOTPVerification(true);
      message.success('Registration successful! OTP sent to your email.');
    } catch (error: any) {
      message.error(error.message || ERROR_MESSAGES.REGISTRATION_FAILED);
    }
  };

  const handleVerifyOTP = async () => {
    if (!otp || otp.length !== 6) {
      message.error('Please enter valid 6-digit OTP');
      return;
    }
    
    try {
      await dispatch(verifyOTP({ email, otp, purpose: 'REGISTER' })).unwrap();
      message.success('Account verified successfully! You can now login.');
      navigate(COMMON_ROUTES.LOGIN);
    } catch (error: any) {
      message.error(error.message || 'Verification failed. Please try again.');
    }
  };

  if (showOTPVerification) {
    return (
      <div className={RegisterCss.registerContainer}>
        <div className={RegisterCss.header}>
          <img src={LoanAcceleratorLogo} alt="LMP" className={RegisterCss.logo} />
          <h2 className={RegisterCss.brandName}>
            <span className={RegisterCss.brandHighlight}>LMP</span>
          </h2>
        </div>

        <Title level={3} className={RegisterCss.title}>
          Verify Your Account
        </Title>

        <div className={RegisterCss.form}>
          <div className={RegisterCss.inputGroup}>
            <label className={RegisterCss.label}>Enter OTP sent to {email}</label>
            <Input
              className={RegisterCss.input}
              value={otp}
              onChange={(e) => setOTP(e.target.value.replace(/\D/g, '').slice(0, 6))}
              placeholder="6-digit OTP"
              maxLength={6}
            />
          </div>

          <Button
            type="primary"
            className={RegisterCss.submitButton}
            onClick={handleVerifyOTP}
            loading={isLoading}
          >
            VERIFY ACCOUNT
          </Button>

          <Typography className={RegisterCss.existingUserText}>
            Already verified?{" "}
            <RouterLink to={COMMON_ROUTES.LOGIN}>
              Login Here{" "}
            </RouterLink>
          </Typography>
        </div>
      </div>
    );
  }

  return (
    <div className={RegisterCss.registerContainer}>
      <div className={RegisterCss.header}>
        <img src={LoanAcceleratorLogo} alt="LMP" className={RegisterCss.logo} />
        <h2 className={RegisterCss.brandName}>
          <span className={RegisterCss.brandHighlight}>LMP</span>
        </h2>
      </div>

      <Title level={3} className={RegisterCss.title}>
        Sign Up
      </Title>

      <Steps current={currentStep} style={{ marginBottom: '24px', padding: '0 20px' }} size="small">
        <Steps.Step title="Account" />
        <Steps.Step title="Personal" />
        <Steps.Step title="Financial" />
      </Steps>

      <div className={RegisterCss.form}>
        {currentStep === 0 && (
          <>
            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Full Name</label>
              <Input
                type="text"
                className={RegisterCss.input}
                value={name}
                onChange={handleNameChange}
                status={nameError ? "error" : ""}
                placeholder="Full Name"
              />
              {nameError && <div className={RegisterCss.errorMessage}>{nameError}</div>}
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Email ID</label>
              <Input
                type="text"
                className={RegisterCss.input}
                value={email}
                onChange={handleEmailChange}
                status={emailError ? "error" : ""}
                placeholder="name@email.com"
              />
              {emailError && <div className={RegisterCss.errorMessage}>{emailError}</div>}
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Password</label>
              <Input.Password
                className={RegisterCss.input}
                value={password}
                onChange={handlePasswordChange}
                status={passwordError ? "error" : ""}
                placeholder="at least 8 characters"
              />
              {passwordError && <div className={RegisterCss.errorMessage}>{passwordError}</div>}
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Confirm Password</label>
              <Input.Password
                className={RegisterCss.input}
                value={confirmPassword}
                onChange={handleConfirmPasswordChange}
                status={confirmPasswordError ? "error" : ""}
                placeholder="confirm your password"
              />
              {confirmPasswordError && <div className={RegisterCss.errorMessage}>{confirmPasswordError}</div>}
            </div>

            <Button type="primary" className={RegisterCss.submitButton} onClick={handleNext}>
              NEXT
            </Button>
          </>
        )}

        {currentStep === 1 && (
          <>
            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Date of Birth</label>
              <Input
                type="date"
                className={RegisterCss.input}
                value={dateOfBirth}
                onChange={(e) => setDateOfBirth(e.target.value)}
              />
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Gender</label>
              <Select
                className={RegisterCss.input}
                value={gender}
                onChange={(value) => setGender(value)}
                options={[
                  { value: 0, label: 'Male' },
                  { value: 1, label: 'Female' }
                ]}
              />
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Phone Number</label>
              <Input
                type="tel"
                className={RegisterCss.input}
                value={phoneNumber}
                onChange={handlePhoneChange}
                status={phoneError ? "error" : ""}
                placeholder="10 digit phone number"
                maxLength={10}
              />
              {phoneError && <div className={RegisterCss.errorMessage}>{phoneError}</div>}
            </div>

            <div style={{ display: 'flex', gap: '10px' }}>
              <Button onClick={handlePrevious} style={{ flex: 1 }}>BACK</Button>
              <Button type="primary" onClick={handleNext} style={{ flex: 1 }}>NEXT</Button>
            </div>
          </>
        )}

        {currentStep === 2 && (
          <>
            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>PAN Number</label>
              <Input
                className={RegisterCss.input}
                value={panNumber}
                onChange={handlePanChange}
                status={panError ? "error" : ""}
                placeholder="ABCDE1234F"
                maxLength={10}
              />
              {panError && <div className={RegisterCss.errorMessage}>{panError}</div>}
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Aadhaar Number</label>
              <Input
                className={RegisterCss.input}
                value={aadhaarNumber}
                onChange={handleAadhaarChange}
                status={aadhaarError ? "error" : ""}
                placeholder="12 digit Aadhaar number"
                maxLength={12}
              />
              {aadhaarError && <div className={RegisterCss.errorMessage}>{aadhaarError}</div>}
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Annual Income</label>
              <Input
                type="number"
                className={RegisterCss.input}
                value={annualIncome}
                onChange={(e) => setAnnualIncome(e.target.value)}
                placeholder="Enter annual income"
              />
            </div>

            <div className={RegisterCss.inputGroup}>
              <label className={RegisterCss.label}>Home Ownership Status (Optional)</label>
              <Select
                className={RegisterCss.input}
                value={homeOwnershipStatus}
                onChange={(value) => setHomeOwnershipStatus(value)}
                placeholder="Select status"
                options={[
                  { value: 0, label: 'Owned' },
                  { value: 1, label: 'Rented' },
                  { value: 2, label: 'Mortgaged' }
                ]}
              />
            </div>

            <div style={{ display: 'flex', gap: '10px' }}>
              <Button onClick={handlePrevious} style={{ flex: 1 }}>BACK</Button>
              <Button type="primary" onClick={handleSubmit} loading={isLoading} style={{ flex: 1 }}>
                SIGN UP
              </Button>
            </div>
          </>
        )}

        <Typography className={RegisterCss.existingUserText}>
          Existing User?{" "}
          <RouterLink to={COMMON_ROUTES.LOGIN}>
            Login Here{" "}
          </RouterLink>
        </Typography>
      </div>
    </div>
  );
};

export default RegisterComponent;
