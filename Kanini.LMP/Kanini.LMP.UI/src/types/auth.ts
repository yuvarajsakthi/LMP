export interface LoginCredentials {
  username: string;
  password: string;
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