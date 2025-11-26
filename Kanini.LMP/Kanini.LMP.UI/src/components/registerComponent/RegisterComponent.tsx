import { useState } from "react";
import { Typography, Input, Button, message } from "antd";
import { useDispatch, useSelector } from 'react-redux';
import { LoanAcceleratorLogo } from "../../assets";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import RegisterCss from "./RegisterComponent.module.css";
import type { RegisterCredentials, InputChangeEvent } from "../../types";
import { USER_ROLES, ROUTES, SUCCESS_MESSAGES, ERROR_MESSAGES } from "../../config";
import { validateField } from "../../utils";
import { useAuth } from "../../context";
import { registerUser } from "../../store/slices/authSlice";
import type { RootState, AppDispatch } from "../../store";
const { Title } = Typography;

const RegisterComponent = () => {
  const dispatch = useDispatch<AppDispatch>();
  const { isLoading } = useSelector((state: RootState) => state.auth);
  const navigate = useNavigate();
  const { setToken } = useAuth();
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");



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
  };

  const handleSubmit = async () => {
    if (nameError || emailError || passwordError) {
      message.error('Please fill all fields correctly');
      return;
    }

    const registerData: RegisterCredentials = {
      email: email,
      password: password,
      fullName: name,
      role: "Customer",
    };
    
    try {
      const result = await dispatch(registerUser(registerData)).unwrap();
      message.success(SUCCESS_MESSAGES.REGISTER_SUCCESS);
      setToken(result.user);
      
      const userRole = result.user.role;
      if (userRole?.toLowerCase() === USER_ROLES.MANAGER.toLowerCase()) {
        message.success(SUCCESS_MESSAGES.REGISTER_MANAGER);
        navigate(ROUTES.APPLIED_LOAN);
      } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER.toLowerCase()) {
        navigate(ROUTES.CUSTOMER_DASHBOARD);
        message.success(SUCCESS_MESSAGES.REGISTER_CUSTOMER);
      }
    } catch (error: any) {
      message.error(error || ERROR_MESSAGES.REGISTRATION_FAILED);
    }
  };

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
          <RouterLink to={ROUTES.LOGIN}>
            Login Here{" "}
          </RouterLink>
        </Typography>
      </div>
    </div>
  );
};

export default RegisterComponent;
