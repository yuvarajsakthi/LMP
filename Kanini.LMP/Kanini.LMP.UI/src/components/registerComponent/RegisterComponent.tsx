import { useState } from "react";
import { Typography, Input, Button, message, Select } from "antd";
import { useDispatch, useSelector } from 'react-redux';
import { LoanAcceleratorLogo } from "../../assets";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import RegisterCss from "./RegisterComponent.module.css";
import type { InputChangeEvent } from "../../types";
import { COMMON_ROUTES, ERROR_MESSAGES } from "../../config";
import { validateField } from "../../utils";
import { useAuth } from "../../context";
import { registerUser } from "../../store/slices/authSlice";
import type { RootState, AppDispatch } from "../../store";
const { Title } = Typography;

const RegisterComponent = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { isLoading } = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();
  const {} = useAuth();
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [dateOfBirth, setDateOfBirth] = useState("");
  const [gender, setGender] = useState(0);
  const [phoneNumber, setPhoneNumber] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [confirmPasswordError, setConfirmPasswordError] = useState("");
  const [phoneError, setPhoneError] = useState("");
  const [showOTPVerification, setShowOTPVerification] = useState(false);
  const [otp, setOTP] = useState("");



  const handleNameChange = async (e: InputChangeEvent) => {
    const nameValue = e.target.value;
    setName(nameValue);
    const error = await validateField('name', nameValue, 'register');
    setNameError(error);
  };

  const handleEmailChange = async (e: InputChangeEvent) => {
    const emailValue = e.target.value;
    setEmail(emailValue);
    const error = await validateField('email', emailValue, 'register');
    setEmailError(error);
  };

  const handlePasswordChange = async (e: InputChangeEvent) => {
    const passwordValue = e.target.value;
    setPassword(passwordValue);
    const error = await validateField('password', passwordValue, 'register');
    setPasswordError(error);
    
    if (confirmPassword && passwordValue !== confirmPassword) {
      setConfirmPasswordError('Passwords do not match');
    } else {
      setConfirmPasswordError('');
    }
  };

  const handleConfirmPasswordChange = (e: InputChangeEvent) => {
    const confirmPasswordValue = e.target.value;
    setConfirmPassword(confirmPasswordValue);
    
    if (password !== confirmPasswordValue) {
      setConfirmPasswordError('Passwords do not match');
    } else {
      setConfirmPasswordError('');
    }
  };

  const handlePhoneChange = (e: InputChangeEvent) => {
    const phoneValue = e.target.value.replace(/\D/g, '');
    setPhoneNumber(phoneValue);
    if (phoneValue.length !== 10) {
      setPhoneError('Phone number must be 10 digits');
    } else {
      setPhoneError('');
    }
  };

  const handleSubmit = async () => {
    if (nameError || emailError || passwordError || confirmPasswordError || phoneError || !dateOfBirth || !phoneNumber) {
      message.error('Please fill all fields correctly');
      return;
    }

    if (password !== confirmPassword) {
      message.error('Passwords do not match');
      return;
    }
    
    try {
      await dispatch(registerUser({
        fullName: name,
        email,
        password,
        dateOfBirth,
        gender,
        phoneNumber
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
      const { authAPI } = await import('../../services');
      await authAPI.verifyOTP({ email, otp });
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
          <img src={LoanAcceleratorLogo} alt="Loan Accelerator" className={RegisterCss.logo} />
          <h2 className={RegisterCss.brandName}>
            <span className={RegisterCss.brandHighlight}>Loan</span> Accelerator
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
        <img src={LoanAcceleratorLogo} alt="Loan Accelerator" className={RegisterCss.logo} />
        <h2 className={RegisterCss.brandName}>
          <span className={RegisterCss.brandHighlight}>Loan</span> Accelerator
        </h2>
      </div>

      <Title level={3} className={RegisterCss.title}>
        Sign Up
      </Title>


      <div className={RegisterCss.form}>
        <div className={RegisterCss.inputGroup}>
          <label className={RegisterCss.label}>Full Name</label>
          <Input
            type="text"
            className={RegisterCss.input}
            name="name"
            value={name}
            onChange={handleNameChange}
            status={nameError ? "error" : ""}
            placeholder="Full Name"
          />
          {nameError && (
            <div className={RegisterCss.errorMessage}>{nameError}</div>
          )}
        </div>

        <div className={RegisterCss.inputGroup}>
          <label className={RegisterCss.label}>Email ID</label>
          <Input
            type="text"
            className={RegisterCss.input}
            name="mail"
            value={email}
            onChange={handleEmailChange}
            status={emailError ? "error" : ""}
            placeholder="name@email.com"
          />
          {emailError && (
            <div className={RegisterCss.errorMessage}>{emailError}</div>
          )}
        </div>

        <div className={RegisterCss.inputGroup}>
          <label className={RegisterCss.label}>Password</label>
          <Input.Password
            className={RegisterCss.input}
            name="Password"
            value={password}
            onChange={handlePasswordChange}
            status={passwordError ? "error" : ""}
            placeholder="at least 8 characters"
          />
          {passwordError && (
            <div className={RegisterCss.errorMessage}>{passwordError}</div>
          )}
        </div>

        <div className={RegisterCss.inputGroup}>
          <label className={RegisterCss.label}>Confirm Password</label>
          <Input.Password
            className={RegisterCss.input}
            name="ConfirmPassword"
            value={confirmPassword}
            onChange={handleConfirmPasswordChange}
            status={confirmPasswordError ? "error" : ""}
            placeholder="confirm your password"
          />
          {confirmPasswordError && (
            <div className={RegisterCss.errorMessage}>{confirmPasswordError}</div>
          )}
        </div>

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
          {phoneError && (
            <div className={RegisterCss.errorMessage}>{phoneError}</div>
          )}
        </div>

        <Button
          type="primary"
          className={RegisterCss.submitButton}
          onClick={handleSubmit}
          loading={isLoading}
        >
          SIGN UP
        </Button>

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
