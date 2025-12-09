export interface DashboardStats {
  totalApplications: number;
  pendingApplications: number;
  approvedApplications: number;
  rejectedApplications: number;
  disbursedApplications: number;
  totalDisbursedAmount: number;
  activeLoans: number;
  loanTypeDistribution: LoanTypeDistribution[];
  monthlyTrend: MonthlyTrend[];
}

export interface LoanTypeDistribution {
  loanType: string;
  count: number;
  totalAmount: number;
}

export interface MonthlyTrend {
  month: string;
  applicationCount: number;
}

export interface LoanApplicationDetail {
  loanApplicationBaseId: number;
  customerId: number;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  loanType: string;
  requestedAmount: number;
  tenureMonths: number;
  interestRate?: number;
  status: string;
  submissionDate: string;
  approvedDate?: string;
  rejectionReason?: string;
  emiStatus?: EMIStatus;
  documents: DocumentInfo[];
}

export interface EMIStatus {
  emiId: number;
  monthlyEMI: number;
  totalRepaymentAmount: number;
  totalInterestPaid: number;
  termMonths: number;
  status: string;
  isCompleted: boolean;
}

export interface DocumentInfo {
  documentId: number;
  documentName: string;
  documentType: string;
  uploadedAt: string;
  documentData?: string;
}
