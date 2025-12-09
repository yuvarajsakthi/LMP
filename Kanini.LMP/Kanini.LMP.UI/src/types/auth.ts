export interface LoginCredentials {
  Username: string;
  PasswordHash: string;
}

export interface LoginResponse {
  token: string;
  username: string;
  role: string;
}

export interface DecodedToken {
  role: string;
  FullName?: string;
  customerId?: string;
  CustomerId?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
  [key: string]: any;
}

export interface User {
  role: string;
  FullName?: string;
}
