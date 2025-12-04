import { useState } from "react";
import { Typography, Input, Button, message } from "antd";
import { useDispatch, useSelector } from 'react-redux';
import { LoanAcceleratorLogo } from "../../assets";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import LoginComponentCss from "./LoginComponent.module.css";
import { useAuth } from "../../context";
import type { LoginCredentials, InputChangeEvent } from "../../types";
import { COMMON_ROUTES, CUSTOMER_ROUTES, USER_ROLES } from "../../config";
import { validateField } from "../../utils";
import { loginUser } from "../../store/slices/authSlice";
import type { RootState, AppDispatch } from "../../store";
const { Title } = Typography;

const Login = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { isLoading } = useSelector((state: RootState) => state.auth);
  const { setToken } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");
  const [otp, setOTP] = useState("");
  const [useOTPLogin, setUseOTPLogin] = useState(false);
  const [otpSent, setOtpSent] = useState(false);



  const handleEmailChange = async (e: InputChangeEvent) => {
    const emailValue = e.target.value;
    setEmail(emailValue);
    const error = await validateField('email', emailValue, 'login');
    setEmailError(error);
  };

  const handlePasswordChange = async (e: InputChangeEvent) => {
    const passwordValue = e.target.value;
    setPassword(passwordValue);
    const error = await validateField('password', passwordValue, 'login');
    setPasswordError(error);
  };
  const handleSendOTP = async () => {
    if (!email) {
      message.error('Please enter your email');
      return;
    }
    if (emailError) {
      return;
    }
    try {
      const { authAPI } = await import('../../services');
      await authAPI.sendLoginOTP({ email });
      setOtpSent(true);
      message.success('OTP sent to your email');
    } catch (error: any) {
      console.error('Failed to send OTP:', error);
    }
  };

  const handleSubmit = async () => {
    if (useOTPLogin) {
      if (!otp || otp.length !== 6) {
        message.error('Please enter valid 6-digit OTP');
        return;
      }
      try {
        const { authAPI } = await import('../../services');
        
        await authAPI.verifyOTP({ email, otp });
        message.success('Account verified! Logging you in...');
        
        const loginData: LoginCredentials = { username: email, password };
        const result = await dispatch(loginUser(loginData)).unwrap();
        
        if ('token' in result && result.token) {
          const { authMiddleware } = await import('../../middleware');
          authMiddleware.setToken(result.token);
          setToken(result.user);
          
          const userRole = result.user.role;
          if (userRole?.toLowerCase() === USER_ROLES.MANAGER.toLowerCase()) {
            navigate(COMMON_ROUTES.APPLIED_LOAN);
          } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER.toLowerCase()) {
            navigate(CUSTOMER_ROUTES.CUSTOMER_DASHBOARD);
          }
        }
      } catch (error: any) {
        message.error(error.message || 'Verification failed');
      }
    } else {
      if (emailError || passwordError) {
        return;
      }
      if (!email || !password) {
        message.error('Email and password are required');
        return;
      }
      
      const loginData: LoginCredentials = {
        username: email,
        password: password,
      };
      
      try {
        const result = await dispatch(loginUser(loginData)).unwrap();
        
        if ('requiresVerification' in result && result.requiresVerification) {
          message.warning('Account not verified. OTP sent to your email.');
          setUseOTPLogin(true);
          setOtpSent(true);
          return;
        }
        
        if ('token' in result && result.token) {
          const { authMiddleware } = await import('../../middleware');
          authMiddleware.setToken(result.token);
          setToken(result.user);
          message.success('Logged in successfully!');
          
          const userRole = result.user.role;
          if (userRole?.toLowerCase() === USER_ROLES.MANAGER.toLowerCase()) {
            navigate(COMMON_ROUTES.APPLIED_LOAN);
          } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER.toLowerCase()) {
            navigate(CUSTOMER_ROUTES.CUSTOMER_DASHBOARD);
          }
        }
      } catch (error: any) {
        message.error(error.message || 'Invalid credentials');
      }
    }
  };

  return (
    <div className={LoginComponentCss.loginContainer}>
      <div className={LoginComponentCss.header}>
        <img src={LoanAcceleratorLogo} alt="Loan Accelerator" className={LoginComponentCss.logo} />
        <h2 className={LoginComponentCss.brandName}>
          <span className={LoginComponentCss.brandHighlight}>Loan</span> Accelerator
        </h2>
      </div>

      <Title level={3} className={LoginComponentCss.title}>
        Sign In
      </Title>

      <div className={LoginComponentCss.form}>
        <div className={LoginComponentCss.inputGroup}>
          <label className={LoginComponentCss.label}>Email ID</label>
          <Input
            type="text"
            className={LoginComponentCss.input}
            name="mail"
            value={email}
            onChange={handleEmailChange}
            status={emailError ? "error" : ""}
            placeholder="name@email.com"
          />
          {emailError && (
            <div className={LoginComponentCss.errorMessage}>{emailError}</div>
          )}
        </div>

        {useOTPLogin ? (
          <div className={LoginComponentCss.inputGroup}>
            <label className={LoginComponentCss.label}>Enter OTP</label>
            <div style={{ display: 'flex', gap: '10px' }}>
              <Input
                className={LoginComponentCss.input}
                value={otp}
                onChange={(e) => setOTP(e.target.value.replace(/\D/g, '').slice(0, 6))}
                placeholder="6-digit OTP"
                maxLength={6}
                disabled={!otpSent}
              />
              {!otpSent && (
                <Button onClick={handleSendOTP} loading={isLoading}>
                  Send OTP
                </Button>
              )}
            </div>
          </div>
        ) : (
          <div className={LoginComponentCss.inputGroup}>
            <label className={LoginComponentCss.label}>Password</label>
            <Input.Password
              className={LoginComponentCss.input}
              name="Password"
              value={password}
              onChange={handlePasswordChange}
              status={passwordError ? "error" : ""}
              placeholder="at least 8 characters"
            />
            {passwordError && (
              <div className={LoginComponentCss.errorMessage}>{passwordError}</div>
            )}
          </div>
        )}

        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <RouterLink to={COMMON_ROUTES.FORGOT_PASSWORD} className={LoginComponentCss.forgotPasswordLink}>
            Forgot your password?
          </RouterLink>
          <Button type="link" onClick={() => { setUseOTPLogin(!useOTPLogin); setOtpSent(false); setOTP(''); }}>
            {useOTPLogin ? 'Use Password' : 'Use OTP'}
          </Button>
        </div>

        <Button
          type="primary"
          className={LoginComponentCss.submitButton}
          onClick={handleSubmit}
          loading={isLoading}
        >
          {useOTPLogin ? 'LOGIN WITH OTP' : 'SIGN IN'}
        </Button>

        <Typography className={LoginComponentCss.newUserText}>
          New User? Create a{" "}
          <RouterLink to={COMMON_ROUTES.REGISTER}>
            New Account{" "}
          </RouterLink>
        </Typography>
      </div>
    </div>
  );
};

export default Login;
