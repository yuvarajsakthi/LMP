export const ERROR_MESSAGES = {
  INVALID_EMAIL: 'Invalid email',
  INVALID_PASSWORD: 'Password must be at least 8 characters and contain at least one lowercase letter, one uppercase letter, one digit, and one special character.',
  LOGIN_FAILED: 'Login failed. Please try again.',
  REGISTRATION_FAILED: 'Registration failed. Please try again.',
  NETWORK_ERROR: 'An error occurred. Please check your network connection.',
  NAME_REQUIRED: 'Name is required',
  NAME_NO_NUMBERS: 'Name should not contain numbers',
} as const;

export const SUCCESS_MESSAGES = {
  LOGIN_MANAGER: 'Logged in as Manager Successfully',
  LOGIN_CUSTOMER: 'Logged in as Customer Successfully',
  REGISTER_SUCCESS: 'Registered successfully!',
  REGISTER_MANAGER: 'Registered as Manager Successfully',
  REGISTER_CUSTOMER: 'Registered Successfully',
} as const;