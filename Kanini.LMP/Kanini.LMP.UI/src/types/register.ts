export interface RegisterCredentials {
  fullName: string;
  email: string;
  password: string;
  dateOfBirth: string;
  gender: number;
  phoneNumber: string;
  panNumber: string;
  aadhaarNumber: string;
  annualIncome: number;
  homeOwnershipStatus?: number;
}

export interface RegisterResponse {
  message: string;
  userId: number;
  email: string;
  fullName: string;
}