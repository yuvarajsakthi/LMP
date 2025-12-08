export interface HomeLoanDetails {
  propertyType: number;
  propertyAddress: string;
  city: string;
  zipCode: number;
  ownershipType: number;
  propertyCost: number;
  downPayment: number;
  loanPurpose: number;
}

export enum PropertyType {
  Residential = 0,
  Commercial = 1,
  Industrial = 2,
  SpecialPurpose = 3
}

export enum OwnershipType {
  Owned = 0,
  Builder = 1
}

export enum LoanPurposeHome {
  Purchase = 0,
  Construction = 1,
  Renovation = 2
}
