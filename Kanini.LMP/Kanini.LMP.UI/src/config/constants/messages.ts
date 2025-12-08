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

export const ERROR_BOUNDARY_MESSAGES = {
  TITLE: 'Something went wrong',
  DESCRIPTION: "We're sorry, but something unexpected happened.",
  RETRY_BUTTON: 'Try Again',
} as const;

export const ERROR_MESSAGES = {
  INVALID_EMAIL: 'Please enter a valid email address',
  INVALID_PASSWORD: 'Password must contain at least 8 characters with uppercase, lowercase, number and special character',
  NAME_REQUIRED: 'Name is required',
  NAME_NO_NUMBERS: 'Name cannot contain numbers',
  OTP_SEND_FAILED: 'Failed to send OTP. Please try again.',
  OTP_VERIFY_FAILED: 'Invalid OTP. Please try again.',
  OTP_REQUIRED: 'Please verify OTP first.',
  PASSWORD_UPDATE_FAILED: 'An error occurred while updating password.',
  REGISTRATION_FAILED: 'Registration failed. Please try again.',
} as const;