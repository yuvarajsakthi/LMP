import React, { createContext, useContext, useReducer, type ReactNode } from 'react';
import type {
  PersonalDetailsDTO,
  AddressInformationDTO,
  FamilyEmergencyDetailsDTO,
  DeclarationDTO,
  DocumentUploadDTO
} from '../types/loanApplicationCreate';

interface LoanDetailsData {
  tenureMonths?: number;
  requestedLoanAmount?: number;
  employmentType?: number;
  monthlyIncome?: number;
  workExperienceYears?: number;
  loanPurpose?: number;
  // Home loan specific
  propertyType?: number;
  propertyAddress?: string;
  city?: string;
  zipCode?: number;
  ownershipType?: number;
  propertyCost?: number;
  downPayment?: number;
  // Vehicle loan specific
  vehicleType?: number;
  manufacturer?: string;
  model?: string;
  manufacturingYear?: number;
  onRoadPrice?: number;
  loanPurposeVehicle?: number;
}

interface LoanApplicationState {
  currentStep: number;
  loanType: 'personal' | 'home' | 'vehicle' | null;
  formData: {
    loanDetails: LoanDetailsData;
    documents: DocumentUploadDTO[];
    personalDetails: Partial<PersonalDetailsDTO>;
    addressInformation: Partial<AddressInformationDTO>;
    familyEmergencyDetails: Partial<FamilyEmergencyDetailsDTO>;
    declaration: Partial<DeclarationDTO>;
  };
}

type LoanApplicationAction =
  | { type: 'SET_STEP'; payload: number }
  | { type: 'NEXT_STEP' }
  | { type: 'PREV_STEP' }
  | { type: 'UPDATE_FORM_DATA'; payload: { section: keyof LoanApplicationState['formData']; data: any } }
  | { type: 'SET_LOAN_TYPE'; payload: 'personal' | 'home' | 'vehicle' }
  | { type: 'RESET_FORM' };

const initialState: LoanApplicationState = {
  currentStep: 0,
  loanType: null,
  formData: {
    loanDetails: {},
    documents: [],
    personalDetails: {},
    addressInformation: {},
    familyEmergencyDetails: {},
    declaration: {}
  }
};

const loanApplicationReducer = (state: LoanApplicationState, action: LoanApplicationAction): LoanApplicationState => {
  switch (action.type) {
    case 'SET_STEP':
      return { ...state, currentStep: action.payload };
    case 'NEXT_STEP':
      return { ...state, currentStep: Math.min(state.currentStep + 1, 7) };
    case 'PREV_STEP':
      return { ...state, currentStep: Math.max(state.currentStep - 1, 0) };
    case 'UPDATE_FORM_DATA':
      return {
        ...state,
        formData: {
          ...state.formData,
          [action.payload.section]: action.payload.data
        }
      };
    case 'SET_LOAN_TYPE':
      return { ...state, loanType: action.payload };
    case 'RESET_FORM':
      return initialState;
    default:
      return state;
  }
};

const LoanApplicationContext = createContext<{
  state: LoanApplicationState;
  dispatch: React.Dispatch<LoanApplicationAction>;
} | null>(null);

export const LoanApplicationProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(loanApplicationReducer, initialState);

  return (
    <LoanApplicationContext.Provider value={{ state, dispatch }}>
      {children}
    </LoanApplicationContext.Provider>
  );
};

export const useLoanApplication = () => {
  const context = useContext(LoanApplicationContext);
  if (!context) {
    throw new Error('useLoanApplication must be used within LoanApplicationProvider');
  }
  return context;
};
