// Enums matching backend
export enum Gender {
  Male = 0,
  Female = 1
}

export enum IndianStates {
  AndhraPradesh = 0,
  ArunachalPradesh = 1,
  Assam = 2,
  Bihar = 3,
  Chhattisgarh = 4,
  Goa = 5,
  Gujarat = 6,
  Haryana = 7,
  HimachalPradesh = 8,
  Jharkhand = 9,
  Karnataka = 10,
  Kerala = 11,
  MadhyaPradesh = 12,
  Maharashtra = 13,
  Manipur = 14,
  Meghalaya = 15,
  Mizoram = 16,
  Nagaland = 17,
  Odisha = 18,
  Punjab = 19,
  Rajasthan = 20,
  Sikkim = 21,
  TamilNadu = 22,
  Telangana = 23,
  Tripura = 24,
  UttarPradesh = 25,
  Uttarakhand = 26,
  WestBengal = 27,
  AndamanAndNicobarIslands = 28,
  Chandigarh = 29,
  DadraAndNagarHaveliAndDamanAndDiu = 30,
  Delhi = 31,
  JammuAndKashmir = 32,
  Ladakh = 33,
  Lakshadweep = 34,
  Puducherry = 35
}

export enum EducationQualification {
  BelowMatric = 1,
  Matric = 2,
  HigherSecondary = 3,
  Graduate = 4,
  PostGraduate = 5,
  Doctorate = 6,
  Other = 7
}

export enum ResidentialStatus {
  Owned = 1,
  Rented = 2,
  Parental = 3,
  CompanyProvided = 4,
  PayingGuest = 5,
  Other = 6
}

export enum DocumentType {
  IdentityProof = 0,
  AddressProof = 1,
  IncomeProof = 2,
  PropertyDocuments = 3,
  VehicleDocuments = 4,
  ApplicationPDF = 5,
  Other = 6
}

// DTOs matching backend structure
export interface DocumentUploadDTO {
  documentName: string;
  documentType: DocumentType;
  documentFile?: File;
}

export interface PersonalDetailsDTO {
  fullName: string;
  dateOfBirth: string;
  districtOfBirth: string;
  panNumber: string;
  educationQualification: EducationQualification;
  residentialStatus: ResidentialStatus;
  gender: Gender;
  signatureImage: File;
  idProofImage: File;
}

export interface AddressInformationDTO {
  presentAddress: string;
  permanentAddress: string;
  district: string;
  state: IndianStates;
  zipCode: string;
  emailId?: string;
  mobileNumber1: string;
  mobileNumber2?: string;
}

export interface FamilyEmergencyDetailsDTO {
  fullName: string;
  relationshipWithApplicant: string;
  mobileNumber: string;
  address: string;
}

export interface DeclarationDTO {
  name: string;
  amount: number;
  description: string;
  purpose: string;
}

// Personal Loan Application
export interface PersonalLoanApplicationCreateDTO {
  customerId: number;
  tenureMonths: number;
  requestedLoanAmount: number;
  employmentType: number;
  monthlyIncome: number;
  workExperienceYears: number;
  loanPurpose: number;
  documents: DocumentUploadDTO[];
  personalDetails: PersonalDetailsDTO;
  addressInformation: AddressInformationDTO;
  familyEmergencyDetails: FamilyEmergencyDetailsDTO;
  declaration: DeclarationDTO;
}

// Home Loan Application
export interface HomeLoanApplicationCreateDTO {
  customerId: number;
  tenureMonths: number;
  requestedLoanAmount: number;
  propertyType: number;
  propertyAddress: string;
  city: string;
  zipCode: number;
  ownershipType: number;
  propertyCost: number;
  downPayment: number;
  loanPurpose: number;
  documents: DocumentUploadDTO[];
  personalDetails: PersonalDetailsDTO;
  addressInformation: AddressInformationDTO;
  familyEmergencyDetails: FamilyEmergencyDetailsDTO;
  declaration: DeclarationDTO;
}

// Vehicle Loan Application
export interface VehicleLoanApplicationCreateDTO {
  customerId: number;
  tenureMonths: number;
  requestedLoanAmount: number;
  vehicleType: number;
  manufacturer: string;
  model: string;
  manufacturingYear: number;
  onRoadPrice: number;
  downPayment: number;
  loanPurposeVehicle: number;
  documents: DocumentUploadDTO[];
  personalDetails: PersonalDetailsDTO;
  addressInformation: AddressInformationDTO;
  familyEmergencyDetails: FamilyEmergencyDetailsDTO;
  declaration: DeclarationDTO;
}
