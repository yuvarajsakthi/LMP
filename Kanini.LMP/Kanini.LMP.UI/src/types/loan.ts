export interface LoanCategory {
  loanProductId: number;
  loanProductName: string;
  loanProductDescription: string;
  isActive: boolean;
}

export interface LoanCategoryResponse {
  success: boolean;
  data?: LoanCategory[];
  message?: string;
  errors?: string[];
}
