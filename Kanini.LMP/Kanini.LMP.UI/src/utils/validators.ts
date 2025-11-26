import { loginSchema, registerSchema } from './validationSchemas';

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