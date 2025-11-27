export interface LoginCredentials {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

export interface DecodedToken {
  role: string;
  [key: string]: any;
}

export interface User {
  role: string;
}