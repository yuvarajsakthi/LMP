import { useState } from "react";
import { Row, Col, Typography, Input, Button, Card, message } from "antd";
import { Customer, Manager, LoanAcceleratorLogo } from "../../assets";
import { jwtDecode } from "jwt-decode";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import RegisterCss from "./RegisterComponent.module.css";
import axiosInstance from "../../api/axiosInstance";
import type { RegisterCredentials, RegisterResponse, DecodedToken, InputChangeEvent } from "../../types";
import { USER_ROLES, ROUTES, API_ENDPOINTS, SUCCESS_MESSAGES, ERROR_MESSAGES } from "../../config";
import { registerSchema } from "../../utils/validationSchemas";
import { useAuth } from "../../context";
import { authMiddleware } from "../../middleware";
const { Title, Text } = Typography;

const Register = () => {
  const [selectedRole, setSelectedRole] = useState("");
  const navigate = useNavigate();
  const { setToken } = useAuth();
  const handleRoleSelect = (role: string) => {
    setSelectedRole(role);
  };
  const [name, setName] = useState("");
  const [nameError, setNameError] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [emailError, setEmailError] = useState("");
  const [passwordError, setPasswordError] = useState("");

  const validateField = async (field: string, value: string) => {
    try {
      await registerSchema.validateAt(field, { [field]: value });
      return "";
    } catch (error: any) {
      return error.message;
    }
  };

  const handleNameChange = async (e: InputChangeEvent) => {
    const nameValue = e.target.value;
    setName(nameValue);
    const error = await validateField('name', nameValue);
    setNameError(error);
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
    if (selectedRole === "" || nameError || emailError || passwordError) {
      return;
    }

    try {
      const registerData: RegisterCredentials = {
        role: selectedRole,
        emailId: email,
        password: password,
        fullName: name,
        isActive: "true",
      };
      const response = await axiosInstance.post<RegisterResponse>(API_ENDPOINTS.USER_REGISTER, registerData);
      message.success(SUCCESS_MESSAGES.REGISTER_SUCCESS);
      const encodedToken = response.data.token;
      authMiddleware.setToken(encodedToken);
      const decodedToken: DecodedToken = jwtDecode(encodedToken);
      const userRole = decodedToken.role;
      setToken(decodedToken);
      if (userRole?.toLowerCase() === USER_ROLES.MANAGER) {
        message.success(SUCCESS_MESSAGES.REGISTER_MANAGER);
        navigate(ROUTES.APPLIED_LOAN);
      } else if (userRole?.toLowerCase() === USER_ROLES.CUSTOMER) {
        navigate(ROUTES.CUSTOMER_DASHBOARD);
        message.success(SUCCESS_MESSAGES.REGISTER_CUSTOMER);
      }
    } catch (error: any) {
      if (error.response) {
        message.error(ERROR_MESSAGES.REGISTRATION_FAILED);
      } else {
        message.error(ERROR_MESSAGES.NETWORK_ERROR);
      }
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
      <Title level={5} className={RegisterCss.subtitle}>
        Please select your role
      </Title>

      <div className={RegisterCss.roleSelection}>
        <Row justify="center" gutter={16}>
          <Col>
            <Card
              className={`${RegisterCss.roleCard} ${selectedRole === "Customer" ? RegisterCss.selected : ""
                }`}
              hoverable
              onClick={() => handleRoleSelect("Customer")}
            >
              <img src={Customer} alt="Customer" />
              <Text
                className={`${RegisterCss.roleText} ${selectedRole === "Customer" ? RegisterCss.selectedText : ""
                  }`}
              >
                Customer
              </Text>
            </Card>
          </Col>
          <Col>
            <Card
              className={`${RegisterCss.roleCard} ${selectedRole === "Manager" ? RegisterCss.selected : ""
                }`}
              hoverable
              onClick={() => handleRoleSelect("Manager")}
            >
              <img src={Manager} alt="Manager" />
              <Text
                className={`${RegisterCss.roleText} ${selectedRole === "Manager" ? RegisterCss.selectedText : ""
                  }`}
              >
                Manager
              </Text>
            </Card>
          </Col>
        </Row>
      </div>

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

export default Register;
