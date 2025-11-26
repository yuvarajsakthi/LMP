import React, { useState, useRef } from "react";
import { Typography, Input, Button, message } from 'antd';
import group from '../../assets/images/LoanAcceleratorLogo.svg';
import axios from '../../axiosInstance';
import { Link, useNavigate, Link as RouterLink } from 'react-router-dom';
import emailjs from 'emailjs-com';
import ForgotPassword from "./ForgotPasswordComponent.module.css";

const { Title, Text } = Typography;

const ForgotPasswordComponent = () => {
    const [showOTPInput, setShowOTPInput] = useState(false);
    const [password, setPassword] = useState('');
    const navigate = useNavigate();
    const [emailError, setEmailError] = useState('');
    const formDataRef = useRef(null);
    const [otpVerified, setOtpVerified] = useState(false);
    const [email, setEmail] = useState('');
    const [otp, setOTP] = useState([]);
    const [status, setStatus] = useState('');
    const [showOTPField, setShowOTPField] = useState(true);
    const [generatedOTP, setGeneratedOTP] = useState('');
    const inputRefs = useRef([]);

    function generateOTP() {
        return Math.floor(100000 + Math.random() * 900000);
    }
    const handleSendOTP = async () => {
        if (emailError) {
            message.error('Invalid email. Please enter a valid email address.');
            return;
        }
        try {
            // Check if email exists in the database
            const response = await axios.get(`/user/check-email/${email} `);
            console.log(response.data);
            if (response.data) {
                // Email exists, proceed to send OTP
                const newGeneratedOTP = generateOTP();
                setGeneratedOTP(newGeneratedOTP);

                const serviceID = 'Loan_Accelerator';
                const templateID = 'template_otp';
                const userID = 'Ilw2K9cIP-4SZDosa';

                const templateParams = {
                    to_email: email,
                    message: `Your OTP is: ${newGeneratedOTP}`,
                };

                emailjs.send(serviceID, templateID, templateParams, userID)
                    .then(() => {
                        setStatus('OTP sent successfully.');
                        setOTP([]);
                        if (inputRefs.current.length > 0) {
                            inputRefs.current[0].focus();
                        }
                        setTimeout(() => {
                            setGeneratedOTP('');
                        }, 60000);
                    })
                    .catch(() => {
                        setStatus('Failed to send OTP.');
                    });
                setShowOTPInput(true);
                setStatus('OTP sent successfully.');
                setOTP([]);

                if (inputRefs.current.length > 0) {
                    inputRefs.current[0].focus();
                }
                setTimeout(() => {
                    setGeneratedOTP('');
                }, 60000);
            } else {
                setStatus('Email does not exist in the database.');
                message.error('Email is not registered ');
            }
        } catch (error) {
            console.error('Error checking email:', error);
            setStatus('An error occurred while checking email.');
        }
    };

    const handleVerifyOTP = () => {

        const generatedOTPCorrect = parseInt(generatedOTP, 10);
        const userOTPCorrect = parseInt(otp.join(''), 10);


        if (userOTPCorrect === generatedOTPCorrect) {
            setStatus('OTP verified successfully.');
            setOtpVerified(true);
        } else {
            setStatus('Invalid OTP. Please try again.');
            setOtpVerified(false);
            message.error('Invalid OTP. Please try again.');

        }
    };
    const handleArrowNavigation = (index, event) => {
        if (event.key === 'ArrowLeft' && inputRefs.current[index - 1]) {

            inputRefs.current[index - 1].focus();
        } else if (event.key === 'ArrowRight' && inputRefs.current[index + 1]) {

            inputRefs.current[index + 1].focus();
        }
    };

    const handleBackspace = (index, event) => {
        if (event.key === 'Backspace' && !otp[index] && inputRefs.current[index - 1]) {

            const updatedOTP = [...otp];
            updatedOTP[index - 1] = '';
            setOTP(updatedOTP);
            inputRefs.current[index - 1].focus();
        }
    };
    const handleInputChangeOTP = (index, value) => {

        const updatedOTP = [...otp];
        updatedOTP[index] = value.replace(/\D/g, '');

        setOTP(updatedOTP);

        if (value && inputRefs.current[index + 1]) {
            inputRefs.current[index + 1].focus();
        }
    };
    const validateEmail = (email) => {
        const emailRegex = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;

        if (!emailRegex.test(email)) {
            return 'Invalid email';
        }
        return '';
    };


    const handleEmailChange = (e) => {
        const emailValue = e.target.value;
        setEmail(emailValue);
        setEmailError(validateEmail(emailValue));
    };

    const handlePasswordChange = (e) => {
        const passwordValue = e.target.value;
        setPassword(passwordValue);
    };

    const handleSubmit = async () => {
        try {

            if (!otpVerified) {
                message.error('Please verify OTP first.');
                return;
            }

            const emailId = email;
            const newPassword = password;

            const response = await axios.put('/user/update-password', {
                emailId: emailId,
                password: newPassword,
            });

            console.log('Response:', response);

            if (response.status === 200) {
                message.success('Password updated successfully');
                navigate('/');
            } else {
                message.error('An error occurred while updating password');
            }
        } catch (error) {
            console.error('Error updating password:', error);
            message.error('An error occurred while updating password');
        }
    };


    return (
        <div className={ForgotPassword.forgotPasswordContainer}>
            <div className={ForgotPassword.header}>
                <img src={group} alt="Loan Accelerator" className={ForgotPassword.logo} />
                <h2 className={ForgotPassword.brandName}>
                    <span className={ForgotPassword.brandHighlight}>Loan</span> Accelerator
                </h2>
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
                            onChange={(e) => {
                                setEmail(e.target.value);
                                if (emailError) setEmailError('');
                            }}
                        />
                    </div>

                    <Button type="primary" className={ForgotPassword.submitButton} onClick={handleSendOTP}>
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
                                <Button type="primary" onClick={handleSubmit} className={ForgotPassword.verifyButton}>Submit</Button>
                                <Link component={RouterLink} to="/">
                                    <Button className={ForgotPassword.cancelButton}>Cancel</Button>
                                </Link>
                            </div>
                        </div>
                    ) : (
                        <>
                            <Title level={5} className={ForgotPassword.subtitle}>Enter OTP</Title>
                            <div className={ForgotPassword.otpInput}>
                                {Array.from({ length: 6 }, (_, index) => (
                                    <input
                                        key={index}
                                        type="tel"
                                        value={otp[index] || ''}
                                        onChange={(e) => handleInputChangeOTP(index, e.target.value)}
                                        onKeyDown={(e) => {
                                            handleArrowNavigation(index, e);
                                            handleBackspace(index, e);
                                        }}
                                        ref={(el) => (inputRefs.current[index] = el)}
                                        style={{
                                            width: '40px',
                                            height: '40px',
                                            fontSize: '18px',
                                            margin: '4px',
                                            textAlign: 'center',
                                            border: '1px solid #ccc',
                                            borderRadius: '4px',
                                            outline: 'none',
                                        }}
                                        maxLength={1}
                                        inputMode="numeric"
                                    />
                                ))}
                            </div>
                            <div className={ForgotPassword.buttonGroup}>
                                <Button onClick={handleVerifyOTP} className={ForgotPassword.verifyButton}>Verify OTP</Button>
                                <Link component={RouterLink} to="/">
                                    <Button className={ForgotPassword.cancelButton}>Cancel</Button>
                                </Link>
                            </div>
                        </>
                    )}
                </div>
            )}
        </div>
    );
};


export default ForgotPasswordComponent;

