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

export enum VehicleType {
  Car = 0,
  Bike = 1,
  Truck = 2,
  Bus = 3,
  Scooter = 4,
  Van = 5,
  Tractor = 6,
  Bicycle = 7,
  AutoRickshaw = 8,
  Motorcycle = 9,
  ElectricCar = 10,
  PickupTruck = 11,
  SUV = 12,
  Minivan = 13
}

export enum LoanPurposeVehicle {
  New = 0,
  Used = 1
}

export interface PersonalDetailsDTO {
  fullName?: string;
  dateOfBirth?: string;
  districtOfBirth?: string;
  panNumber?: string;
  educationQualification?: EducationQualification;
  residentialStatus?: ResidentialStatus;
  gender?: Gender;
  signatureImage?: File;
  idProofImage?: File;
}

export interface AddressInformationDTO {
  presentAddress?: string;
  permanentAddress?: string;
  district?: string;
  state?: IndianStates;
  zipCode?: string;
}

export interface FamilyEmergencyDetailsDTO {
  fullName?: string;
  relationshipWithApplicant?: string;
  mobileNumber?: string;
  address?: string;
}
