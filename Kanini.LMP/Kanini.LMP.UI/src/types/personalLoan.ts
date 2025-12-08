export interface PersonalLoanDetails {
  employmentType: number;
  monthlyIncome: number;
  workExperienceYears: number;
  loanPurpose: number;
}

export enum EmploymentType {
  Salaried = 0,
  SelfEmployed = 1,
  Business = 2,
  Professional = 3
}

export enum LoanPurposePersonal {
  Medical = 0,
  Education = 1,
  Travel = 2,
  Wedding = 3,
  DebtConsolidation = 4,
  HomeRenovation = 5,
  Other = 6
}
