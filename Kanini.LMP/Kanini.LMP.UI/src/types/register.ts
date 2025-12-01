export interface RegisterCredentials {
  fullName: string;
  email: string;
  password: string;
  dateOfBirth: string;
  gender: number;
  phoneNumber: string;
}

export interface RegisterResponse {
  message: string;
  userId: number;
  email: string;
  fullName: string;
}