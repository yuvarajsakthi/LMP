import * as yup from 'yup';
import { ERROR_MESSAGES } from '../config';

export const loginSchema = yup.object({
  email: yup.string().email(ERROR_MESSAGES.INVALID_EMAIL).required(ERROR_MESSAGES.INVALID_EMAIL),
  password: yup.string()
    .min(8)
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()])[A-Za-z\d!@#$%^&*()]{8,}$/, ERROR_MESSAGES.INVALID_PASSWORD)
    .required(ERROR_MESSAGES.INVALID_PASSWORD),
});

export const registerSchema = yup.object({
  name: yup.string()
    .required(ERROR_MESSAGES.NAME_REQUIRED)
    .test('no-numbers', ERROR_MESSAGES.NAME_NO_NUMBERS, (value) => !/\d/.test(value || '')),
  email: yup.string().email(ERROR_MESSAGES.INVALID_EMAIL).required(ERROR_MESSAGES.INVALID_EMAIL),
  password: yup.string()
    .min(8)
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()])[A-Za-z\d!@#$%^&*()]{8,}$/, ERROR_MESSAGES.INVALID_PASSWORD)
    .required(ERROR_MESSAGES.INVALID_PASSWORD),
});