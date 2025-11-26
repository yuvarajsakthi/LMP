export interface RegisterCredentials {
  role: string;
  emailId: string;
  password: string;
  fullName: string;
  isActive: string;
}

export interface RegisterResponse {
  token: string;
}