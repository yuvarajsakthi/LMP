import { useState } from "react";
import { Typography, Input, Button, message } from "antd";
import group from "../../assets/images/LoanAcceleratorLogo.svg";
import axiosInstance from "../../api/axiosInstance";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import LoginComponentCss from "./LoginComponent.module.css";
import { jwtDecode } from "jwt-decode";
import { useAuth } from "../../context";
import type { LoginCredentials, LoginResponse, DecodedToken, InputChangeEvent } from "../../types";
import { USER_ROLES, ERROR_MESSAGES, SUCCESS_MESSAGES, ROUTES, API_ENDPOINTS } from "../../config";
import { loginSchema } from "../../utils/validationSchemas";
import { authMiddleware } from "../../middleware";
const { Title } = Typography;

const Login = () => {
  const { setToken } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const validateField = async (field: string, value: string) => {
    try {
      await loginSchema.validateAt(field, { [field]: value });
      return "";
    } catch (error: any) {
      return error.message;
    }
  };

  const handleEmailChange = async (e: InputChangeEvent) => {
    const emailValue = e.target.value;
    setEmail(emailValue);
    const error = await validateField('email', emailValue);
    setEmailError(error);
  };

  const handlePasswordChange = async (e: InputChangeEvent) => {
    const passwordValue = e.target.value;
    setPassword(passwordValue);
    const error = await validateField('password', passwordValue);
    setPasswordError(error);
  };
  const handleSubmit = async () => {
    if (emailError) {
      message.error(ERROR_MESSAGES.INVALID_EMAIL);
      return;
    }
    if (passwordError) {
      message.error(ERROR_MESSAGES.INVALID_PASSWORD);
      return;
    }
    try {
      const loginData: LoginCredentials = {
        emailId: email,
        password: password,
      };
      const response = await axiosInstance.post<LoginResponse>(API_ENDPOINTS.USER_LOGIN, loginData);
      const encodedToken = response.data.token;
      authMiddleware.setToken(encodedToken);
      const decodedToken: DecodedToken = jwtDecode(encodedToken);
      const userRole = decodedToken.role;
      setToken(decodedToken);
      if (userRole?.toLowerCase() === USER_ROLES.MANAGER) {
        message.success(SUCCESS_MESSAGES.LOGIN_MANAGER);
        navigate(ROUTES.APPLIED_LOAN);
      } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER) {
        navigate(ROUTES.CUSTOMER_DASHBOARD);
        message.success(SUCCESS_MESSAGES.LOGIN_CUSTOMER);
      }
    } catch (error) {
      message.error(ERROR_MESSAGES.LOGIN_FAILED);
    }
  };

  return (
    <div className={LoginComponentCss.loginContainer}>
      <div className={LoginComponentCss.header}>
        <img src={group} alt="Loan Accelerator" className={LoginComponentCss.logo} />
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
        </div>

        <RouterLink to={ROUTES.FORGOT_PASSWORD} className={LoginComponentCss.forgotPasswordLink}>
          Forgot your password?
        </RouterLink>

        <Button
          type="primary"
          className={LoginComponentCss.submitButton}
          onClick={handleSubmit}
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
