import { useState } from "react";
import { Typography, Input, Button, message } from "antd";
import { useDispatch, useSelector } from 'react-redux';
import { LoanAcceleratorLogo } from "../../assets";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import LoginComponentCss from "./LoginComponent.module.css";
import { useAuth } from "../../context";
import type { LoginCredentials, InputChangeEvent } from "../../types";
import { USER_ROLES, ROUTES } from "../../config";
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
  const handleSubmit = async () => {
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
      setToken(result.user);
      
      const userRole = result.user.role;
      if (userRole?.toLowerCase() === USER_ROLES.MANAGER.toLowerCase()) {
        navigate(ROUTES.APPLIED_LOAN);
      } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER.toLowerCase()) {
        navigate(ROUTES.CUSTOMER_DASHBOARD);
      }
    } catch (error: any) {
      // Error is already handled by ApiService and shown via notifications
      console.error('Login failed:', error);
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

        <RouterLink to={ROUTES.FORGOT_PASSWORD} className={LoginComponentCss.forgotPasswordLink}>
          Forgot your password?
        </RouterLink>

        <Button
          type="primary"
          className={LoginComponentCss.submitButton}
          onClick={handleSubmit}
          loading={isLoading}
        >
          SIGN IN
        </Button>

        <Typography className={LoginComponentCss.newUserText}>
          New User? Create a{" "}
          <RouterLink to={ROUTES.REGISTER}>
            New Account{" "}
          </RouterLink>
        </Typography>
      </div>
    </div>
  );
};

export default Login;
