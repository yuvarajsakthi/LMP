export interface LoanDetailsDTO {
    loanApplicationBaseId: number;
    loanApplicationId: number;
    requestedAmount: number;
    tenureMonths: number;
    appliedDate: string;
    interestRate?: number;
    monthlyInstallment?: number;
}

export interface PersonalDetailsDTO {
    personalDetailsId: number;
    loanApplicationBaseId: number;
    userId: number;
    fullName: string;
    dateOfBirth: string; // DateOnly as string
    districtOfBirth: string;
    countryOfBirth: string;
    panNumber: string;
    educationQualification: string;
    residentialStatus: string;
    gender: number; // Enum
    signatureImage: string; // byte[] as base64 string
    idProofImage: string; // byte[] as base64 string
}

export interface AddressInformationDTO {
    addressInformationId: number;
    loanApplicationBaseId: number;
    userId: number;
    presentAddress: string;
    permanentAddress: string;
    district: string;
    state: number; // Enum
    country: string;
    zipCode: string;
    emailId: string;
    mobileNumber1: string;
    mobileNumber2?: string;
}

export interface FamilyEmergencyDetailsDTO {
    familyEmergencyDetailsId: number;
    loanApplicationBaseId: number;
    userId: number;
    fullName: string;
    relationshipWithApplicant: string;
    mobileNumber: string;
    address: string;
}

export interface EmploymentDetailsDTO {
    employmentDetailsId: number;
    userId: number;
    companyName: string;
    designation: string;
    experience: number;
    emailId: string;
    officeAddress: string;
}

export interface FinancialInformationDTO {
    financialInformationId: number;
    userId: number;
    salary: number;
    rent: number;
    primaryOther: number;
    rentandUtility: number;
    foodandClothing: number;
    education: number;
    loanRepayment: number;
    expenseOther: number;
}

export interface DeclarationDTO {
    declarationId: number;
    loanApplicationBaseId: number;
    name: string;
    amount: number;
    description: string;
    purpose: string;
}

export interface PersonalLoanApplicationCreateDTO {
    loanProductType: string;
    submissionDate: string; // DateOnly as string
    loanDetails: LoanDetailsDTO;
    personalDetails: PersonalDetailsDTO;
    addressInformation: AddressInformationDTO;
    familyEmergencyDetails: FamilyEmergencyDetailsDTO;
    employmentDetails: EmploymentDetailsDTO;
    financialInformation: FinancialInformationDTO;
    declaration: DeclarationDTO;
    applicantIds?: number[];
    documentLinkIds?: number[];
}
