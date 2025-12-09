import * as yup from 'yup';

export type RegisterFormData = yup.InferType<typeof registerSchema>;
import { ERROR_MESSAGES } from '../config';

export const loginSchema = yup.object({
  email: yup.string().email(ERROR_MESSAGES.INVALID_EMAIL).matches(/@gmail\.com$/, 'Email must end with @gmail.com').required(ERROR_MESSAGES.INVALID_EMAIL),
  password: yup.string()
    .min(8)
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()])[A-Za-z\d!@#$%^&*()]{8,}$/, ERROR_MESSAGES.INVALID_PASSWORD)
    .required(ERROR_MESSAGES.INVALID_PASSWORD),
});

export const registerSchema = yup.object({
  name: yup.string()
    .required(ERROR_MESSAGES.NAME_REQUIRED)
    .test('no-numbers', ERROR_MESSAGES.NAME_NO_NUMBERS, (value) => !/\d/.test(value || '')),
  email: yup.string().email(ERROR_MESSAGES.INVALID_EMAIL).matches(/@gmail\.com$/, 'Email must end with @gmail.com').required(ERROR_MESSAGES.INVALID_EMAIL),
  password: yup.string()
    .min(8)
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()])[A-Za-z\d!@#$%^&*()]{8,}$/, ERROR_MESSAGES.INVALID_PASSWORD)
    .required(ERROR_MESSAGES.INVALID_PASSWORD),
  confirmPassword: yup.string()
    .oneOf([yup.ref('password')], 'Passwords do not match')
    .required('Confirm password is required'),
  dateOfBirth: yup.string().required('Date of birth is required'),
  phoneNumber: yup.string()
    .matches(/^[6-9]\d{9}$/, 'Phone number must be 10 digits starting with 6-9')
    .required('Phone number is required'),
  panNumber: yup.string()
    .matches(/^[A-Z]{5}[0-9]{4}[A-Z]{1}$/, 'Invalid PAN format (e.g., ABCDE1234F)')
    .required('PAN number is required'),
  aadhaarNumber: yup.string()
    .matches(/^\d{12}$/, 'Aadhaar must be 12 digits')
    .required('Aadhaar number is required'),
  annualIncome: yup.number()
    .min(0, 'Annual income must be positive')
    .required('Annual income is required'),
});

export const validatePassword = (password: string): string => {
  const missing = [];
  if (password.length < 8) missing.push('8+ chars');
  if (!/[A-Z]/.test(password)) missing.push('A-Z');
  if (!/[a-z]/.test(password)) missing.push('a-z');
  if (!/\d/.test(password)) missing.push('0-9');
  if (!/[!@#$%^&*()]/.test(password)) missing.push('!@#$%^&*()');
  return missing.length ? `Password needs: ${missing.join(', ')}` : '';
};

export const validateField = async (field: string, value: string, schema: 'login' | 'register'): Promise<string> => {
  if (field === 'password') {
    return validatePassword(value);
  }
  
  try {
    const validationSchema = schema === 'login' ? loginSchema : registerSchema;
    await validationSchema.validateAt(field, { [field]: value });
    return '';
  } catch (error: any) {
    return error.message;
  }
};