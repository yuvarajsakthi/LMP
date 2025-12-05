import React, { createContext, useContext, useReducer, type ReactNode } from 'react';

interface LoanApplicationState {
  currentStep: number;
  formData: {
    loanDetails: any;
    personalDetails: any;
    addressInformation: any;
    familyEmergencyDetails: any;
    employmentDetails: any;
    financialInformation: any;
    declaration: any;
    documents: any[];
  };
  selectedCategory: any;
}

type LoanApplicationAction =
  | { type: 'SET_STEP'; payload: number }
  | { type: 'NEXT_STEP' }
  | { type: 'PREV_STEP' }
  | { type: 'UPDATE_FORM_DATA'; payload: { section: string; data: any } }
  | { type: 'SET_CATEGORY'; payload: any };

const initialState: LoanApplicationState = {
  currentStep: 0,
  formData: {
    loanDetails: {},
    personalDetails: {},
    addressInformation: {},
    familyEmergencyDetails: {},
    employmentDetails: {},
    financialInformation: {},
    declaration: {},
    documents: []
  },
  selectedCategory: null
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
    case 'SET_CATEGORY':
      return { ...state, selectedCategory: action.payload };
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