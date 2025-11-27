const validateEndpoint = (endpoint: string): string => {
  if (!endpoint || typeof endpoint !== 'string' || !endpoint.startsWith('/')) {
    throw new Error('Invalid API endpoint format');
  }
  return endpoint;
};

export const API_ENDPOINTS = {
  // Token Controller endpoints
  USER_LOGIN: validateEndpoint('/api/Token/login'),
  USER_REGISTER: validateEndpoint('/api/Token/register'),
  FORGOT_PASSWORD: validateEndpoint('/api/Token/forgot-password'),
  RESET_PASSWORD: validateEndpoint('/api/Token/reset-password'),
  
  // User Controller endpoints
  USER_CHANGE_PASSWORD: validateEndpoint('/api/User/change-password'),
  GET_ALL_USERS: validateEndpoint('/api/User'),
  GET_USER_BY_ID: validateEndpoint('/api/User'),
  CREATE_USER: validateEndpoint('/api/User'),
  
  REFRESH_TOKEN: validateEndpoint('/auth/refresh-token'),
} as const;