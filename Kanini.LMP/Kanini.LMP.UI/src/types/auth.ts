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
  [key: string]: any;
}

export interface User {
  role: string;
  FullName?: string;
}

export interface GuardProps {
  children: React.ReactNode;
}