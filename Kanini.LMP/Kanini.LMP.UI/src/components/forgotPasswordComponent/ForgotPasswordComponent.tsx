import { useState, useRef } from "react";
import { Typography, Input, Button, message } from 'antd';
import { useDispatch, useSelector } from 'react-redux';
import { LeftOutlined } from '@ant-design/icons';
import { LoanAcceleratorLogo } from '../../assets';
import { useNavigate, Link as RouterLink } from 'react-router-dom';
import ForgotPassword from "./ForgotPasswordComponent.module.css";
import { ROUTES, ERROR_MESSAGES, SUCCESS_MESSAGES } from '../../config';
import { forgotPassword, resetPassword } from '../../store/slices/authSlice';
import type { RootState, AppDispatch } from '../../store';
import type { InputRef } from 'antd';

const { Title } = Typography;

const ForgotPasswordComponent = () => {
    const dispatch = useDispatch<AppDispatch>();
    const { isLoading } = useSelector((state: RootState) => state.auth);
    const [showOTPInput, setShowOTPInput] = useState(false);
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const [emailError, setEmailError] = useState('');
    const [otpVerified, setOtpVerified] = useState(false);
    const [email, setEmail] = useState('');
    const [otp, setOTP] = useState<string[]>(Array(6).fill(''));
    const [status, setStatus] = useState('');
    const inputRefs = useRef<(InputRef | null)[]>([]);
    const handleSendOTP = async () => {
        if (emailError) {
            message.error(ERROR_MESSAGES.INVALID_EMAIL);
            return;
        }
        try {
            await dispatch(forgotPassword(email)).unwrap();
            setShowOTPInput(true);
            setStatus(SUCCESS_MESSAGES.OTP_SENT);
            setOTP(Array(6).fill(''));
            if (inputRefs.current[0]) {
                inputRefs.current[0].focus();
            }
            message.success(SUCCESS_MESSAGES.OTP_SENT);
        } catch (error: any) {
            message.error(error || ERROR_MESSAGES.OTP_SEND_FAILED);
        }
    };

    const handleVerifyOTP = () => {
        try {
            const otpValue = otp.join('');
            if (!otpValue || otpValue.length !== 6 || !/^\d{6}$/.test(otpValue)) {
                setStatus(ERROR_MESSAGES.OTP_VERIFY_FAILED);
                setOtpVerified(false);
                message.error(ERROR_MESSAGES.OTP_VERIFY_FAILED);
                return;
            }
            setStatus(SUCCESS_MESSAGES.OTP_VERIFIED);
            setOtpVerified(true);
            message.success(SUCCESS_MESSAGES.OTP_VERIFIED);
        } catch (error) {
            setStatus(ERROR_MESSAGES.OTP_VERIFY_FAILED);
            setOtpVerified(false);
            message.error(ERROR_MESSAGES.OTP_VERIFY_FAILED);
        }
    };
    const handleArrowNavigation = (index: number, event: React.KeyboardEvent) => {
        if (event.key === 'ArrowLeft' && inputRefs.current[index - 1]) {
            inputRefs.current[index - 1]?.focus();
        } else if (event.key === 'ArrowRight' && inputRefs.current[index + 1]) {
            inputRefs.current[index + 1]?.focus();
        }
    };

    const handleBackspace = (index: number, event: React.KeyboardEvent) => {
        if (event.key === 'Backspace' && !otp[index] && inputRefs.current[index - 1]) {
            const updatedOTP = [...otp];
            updatedOTP[index - 1] = '';
            setOTP(updatedOTP);
            inputRefs.current[index - 1]?.focus();
        }
    };
    const handleInputChangeOTP = (index: number, value: string) => {
        const updatedOTP = [...otp];
        updatedOTP[index] = value.replace(/\D/g, '');
        setOTP(updatedOTP);
        
        if (value && inputRefs.current[index + 1]) {
            inputRefs.current[index + 1]?.focus();
        }
    };
    const validateEmail = (email: string) => {
        const emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;
        if (!emailRegex.test(email)) {
            return ERROR_MESSAGES.INVALID_EMAIL;
        }
        return '';
    };

    const handleEmailChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const emailValue = e.target.value;
        setEmail(emailValue);
        setEmailError(validateEmail(emailValue));
    };

    const handlePasswordChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const passwordValue = e.target.value;
        setPassword(passwordValue);
    };

    const handleSubmit = async () => {
        if (!otpVerified) {
            message.error(ERROR_MESSAGES.OTP_REQUIRED);
            return;
        }

        if (!password || password.length < 8) {
            message.error('Password must be at least 8 characters');
            return;
        }

        try {
            await dispatch(resetPassword({
                email: email,
                resetToken: otp.join(''),
                newPassword: password,
            })).unwrap();
            
            message.success(SUCCESS_MESSAGES.PASSWORD_UPDATED);
            navigate(ROUTES.LOGIN);
        } catch (error: any) {
            message.error(error || ERROR_MESSAGES.PASSWORD_UPDATE_FAILED);
        }
    };


    return (
        <div className={ForgotPassword.forgotPasswordContainer}>
            <div className={ForgotPassword.header}>
                <img src={LoanAcceleratorLogo} alt="Loan Accelerator" className={ForgotPassword.logo} />
                <h2 className={ForgotPassword.brandName}>
                    <span className={ForgotPassword.brandHighlight}>Loan</span> Accelerator
                </h2>
            </div>

            <div className={ForgotPassword.backButton}>
                <RouterLink to={ROUTES.LOGIN}>
                    <LeftOutlined /> Back to Login
                </RouterLink>
            </div>

            <Title level={3} className={ForgotPassword.title}>
                Forgot your password?
            </Title>

            {!showOTPInput ? (
                <div className={ForgotPassword.form}>
                    <div className={ForgotPassword.inputGroup}>
                        <label className={ForgotPassword.label}>Email ID</label>
                        <Input
                            type="text"
                            className={ForgotPassword.input}
                            name="mail"
                            status={emailError ? "error" : ""}
                            placeholder="name@email.com"
                            value={email}
                            onChange={handleEmailChange}
                        />
                    </div>

                    <Button type="primary" className={ForgotPassword.submitButton} onClick={handleSendOTP} loading={isLoading}>
                        SEND OTP
                    </Button>
                </div>
            ) : (
                <div className={ForgotPassword.otpContainer}>
                    {otpVerified ? (
                        <div className={ForgotPassword.form}>
                            <div className={ForgotPassword.inputGroup}>
                                <label className={ForgotPassword.label}>New Password</label>
                                <Input.Password
                                    className={ForgotPassword.input}
                                    name="Password"
                                    placeholder="at least 8 characters"
                                    value={password}
                                    onChange={handlePasswordChange}
                                />
                            </div>
                            <div className={ForgotPassword.buttonGroup}>
                                <Button type="primary" onClick={handleSubmit} className={ForgotPassword.verifyButton} loading={isLoading}>Submit</Button>
                                <RouterLink to={ROUTES.LOGIN}>
                                    <Button className={ForgotPassword.cancelButton}>Cancel</Button>
                                </RouterLink>
                            </div>
                        </div>
                    ) : (
                        <div className={ForgotPassword.form}>
                            <div className={ForgotPassword.inputGroup}>
                                <label className={ForgotPassword.label}>Enter OTP</label>
                                <div className={ForgotPassword.otpInputs}>
                                    {Array.from({ length: 6 }, (_, index) => (
                                        <Input
                                            key={index}
                                            ref={(el) => { inputRefs.current[index] = el; }}
                                            className={ForgotPassword.otpInput}
                                            maxLength={1}
                                            value={otp[index] || ''}
                                            onChange={(e) => handleInputChangeOTP(index, e.target.value)}
                                            onKeyDown={(e) => {
                                                handleArrowNavigation(index, e);
                                                handleBackspace(index, e);
                                            }}
                                        />
                                    ))}
                                </div>
                            </div>
                            <Button type="primary" onClick={handleVerifyOTP} className={ForgotPassword.verifyButton}>
                                Verify OTP
                            </Button>
                            {status && <div className={ForgotPassword.status}>{status}</div>}
                        </div>
                    )}
                </div>
            )}
        </div>
    );
};

export default ForgotPasswordComponent;

