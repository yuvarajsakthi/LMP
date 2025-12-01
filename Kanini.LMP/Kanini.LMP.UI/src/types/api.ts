// Backend API Response structure (matches C# ApiResponse<T>)
export interface ApiResponse<T = any> {
  success: boolean;
  data?: T;
  message?: string;
  errors?: string[];
}

// Global Exception Middleware Error Response
export interface ErrorResponse {
  statusCode: number;
  message: string;
  details: string;
  timestamp: string;
}

// Custom error class for API errors
export class ApiException extends Error {
  public statusCode: number;
  public errors?: string[];
  public success: boolean = false;
  public details?: string;

  constructor(message: string, statusCode: number = 500, errors?: string[], details?: string) {
    super(message);
    this.name = 'ApiException';
    this.statusCode = statusCode;
    this.errors = errors;
    this.details = details;
  }
}