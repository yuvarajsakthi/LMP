export interface RegisterCredentials {
  email: string;
  password: string;
  fullName: string;
  role: string;
}

export interface RegisterResponse {
  token: string;
}