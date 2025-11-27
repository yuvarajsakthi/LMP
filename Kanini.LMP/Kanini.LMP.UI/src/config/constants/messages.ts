export const ERROR_MESSAGES = {
  INVALID_EMAIL: 'Invalid email. Please enter a valid email address.',
  INVALID_PASSWORD: 'Password must be 8+ chars with A-Z, a-z, 0-9, and !@#$%^&*()',
  LOGIN_FAILED: 'Login failed. Please try again.',
  REGISTRATION_FAILED: 'Registration failed. Please try again.',
  NETWORK_ERROR: 'An error occurred. Please check your network connection.',
  NAME_REQUIRED: 'Name is required.',
  NAME_NO_NUMBERS: 'Name should not contain numbers.',
  OTP_SEND_FAILED: 'Failed to send OTP. Please try again.',
  OTP_VERIFY_FAILED: 'Invalid OTP. Please try again.',
  OTP_REQUIRED: 'Please verify OTP first.',
  PASSWORD_UPDATE_FAILED: 'An error occurred while updating password.',
} as const;

export const SUCCESS_MESSAGES = {
  LOGIN_MANAGER: 'Logged in as Manager successfully.',
  LOGIN_CUSTOMER: 'Logged in as Customer successfully.',
  REGISTER_SUCCESS: 'Registered successfully!',
  REGISTER_MANAGER: 'Registered as Manager successfully.',
  REGISTER_CUSTOMER: 'Registered successfully.',
  OTP_SENT: 'OTP sent to your email.',
  OTP_VERIFIED: 'OTP verified successfully.',
  PASSWORD_UPDATED: 'Password updated successfully.',
} as const;