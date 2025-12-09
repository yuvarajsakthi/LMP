import axiosInstance from './axiosInstance';
import type {
  PersonalLoanApplicationCreateDTO,
  HomeLoanApplicationCreateDTO,
  VehicleLoanApplicationCreateDTO
} from '../../types/loanApplicationCreate';


export const loanApplicationAPI = {
  createPersonalLoan: async (customerId: number, data: PersonalLoanApplicationCreateDTO) => {
    const formData = new FormData();
    
    // Basic loan details
    formData.append('CustomerId', customerId.toString());
    formData.append('TenureMonths', data.tenureMonths.toString());
    formData.append('RequestedLoanAmount', data.requestedLoanAmount.toString());
    formData.append('EmploymentType', data.employmentType.toString());
    formData.append('MonthlyIncome', data.monthlyIncome.toString());
    formData.append('WorkExperienceYears', data.workExperienceYears.toString());
    formData.append('LoanPurpose', data.loanPurpose.toString());

    // Documents
    data.documents.forEach((doc, index) => {
      formData.append(`Documents[${index}].DocumentName`, doc.documentName);
      formData.append(`Documents[${index}].DocumentType`, doc.documentType.toString());
      if (doc.documentFile) {
        formData.append(`Documents[${index}].DocumentFile`, doc.documentFile);
      }
    });

    // Personal Details
    if (data.personalDetails.fullName) formData.append('PersonalDetails.FullName', data.personalDetails.fullName);
    if (data.personalDetails.dateOfBirth) formData.append('PersonalDetails.DateOfBirth', data.personalDetails.dateOfBirth);
    if (data.personalDetails.districtOfBirth) formData.append('PersonalDetails.DistrictOfBirth', data.personalDetails.districtOfBirth);
    if (data.personalDetails.panNumber) formData.append('PersonalDetails.PANNumber', data.personalDetails.panNumber);
    if (data.personalDetails.educationQualification !== undefined) formData.append('PersonalDetails.EducationQualification', data.personalDetails.educationQualification.toString());
    if (data.personalDetails.residentialStatus !== undefined) formData.append('PersonalDetails.ResidentialStatus', data.personalDetails.residentialStatus.toString());
    if (data.personalDetails.gender !== undefined) formData.append('PersonalDetails.Gender', data.personalDetails.gender.toString());
    if (data.personalDetails.signatureImage) formData.append('PersonalDetails.SignatureImage', data.personalDetails.signatureImage);
    if (data.personalDetails.idProofImage) formData.append('PersonalDetails.IDProofImage', data.personalDetails.idProofImage);

    // Address Information
    if (data.addressInformation.presentAddress) formData.append('AddressInformation.PresentAddress', data.addressInformation.presentAddress);
    if (data.addressInformation.permanentAddress) formData.append('AddressInformation.PermanentAddress', data.addressInformation.permanentAddress);
    if (data.addressInformation.district) formData.append('AddressInformation.District', data.addressInformation.district);
    if (data.addressInformation.state !== undefined) formData.append('AddressInformation.State', data.addressInformation.state.toString());
    if (data.addressInformation.zipCode) formData.append('AddressInformation.ZipCode', data.addressInformation.zipCode);
    if (data.addressInformation.emailId) formData.append('AddressInformation.EmailId', data.addressInformation.emailId);
    if (data.addressInformation.mobileNumber1) formData.append('AddressInformation.MobileNumber1', data.addressInformation.mobileNumber1);
    if (data.addressInformation.mobileNumber2) formData.append('AddressInformation.MobileNumber2', data.addressInformation.mobileNumber2);

    // Family Emergency Details
    if (data.familyEmergencyDetails.fullName) formData.append('FamilyEmergencyDetails.FullName', data.familyEmergencyDetails.fullName);
    if (data.familyEmergencyDetails.relationshipWithApplicant) formData.append('FamilyEmergencyDetails.RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    if (data.familyEmergencyDetails.mobileNumber) formData.append('FamilyEmergencyDetails.MobileNumber', data.familyEmergencyDetails.mobileNumber);
    if (data.familyEmergencyDetails.address) formData.append('FamilyEmergencyDetails.Address', data.familyEmergencyDetails.address);

    // Declaration
    if (data.declaration.name) formData.append('Declaration.Name', data.declaration.name);
    if (data.declaration.amount !== undefined) formData.append('Declaration.Amount', data.declaration.amount.toString());
    if (data.declaration.description) formData.append('Declaration.Description', data.declaration.description);
    if (data.declaration.purpose) formData.append('Declaration.Purpose', data.declaration.purpose);

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/personal/${customerId}`,
      formData
    );
    return response.data;
  },

  createHomeLoan: async (customerId: number, data: HomeLoanApplicationCreateDTO) => {
    const formData = new FormData();
    
    // Base fields
    formData.append('RequestedAmount', data.requestedLoanAmount.toString());
    formData.append('TenureMonths', data.tenureMonths.toString());
    if (data.familyEmergencyDetails.fullName) formData.append('RelationFullName', data.familyEmergencyDetails.fullName);
    if (data.familyEmergencyDetails.relationshipWithApplicant) formData.append('RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    if (data.familyEmergencyDetails.mobileNumber) formData.append('MobileNumber', data.familyEmergencyDetails.mobileNumber);
    if (data.familyEmergencyDetails.address) formData.append('RelationAddress', data.familyEmergencyDetails.address);
    if (data.addressInformation.presentAddress) formData.append('PresentAddress', data.addressInformation.presentAddress);
    if (data.addressInformation.permanentAddress) formData.append('PermanentAddress', data.addressInformation.permanentAddress);
    if (data.addressInformation.district) formData.append('District', data.addressInformation.district);
    if (data.addressInformation.state !== undefined) formData.append('State', data.addressInformation.state.toString());
    formData.append('ZipCode', data.zipCode.toString());
    if (data.personalDetails.signatureImage) {
      formData.append('SignatureImage', data.personalDetails.signatureImage);
    }
    
    // Home loan specific fields
    formData.append('PropertyType', data.propertyType.toString());
    formData.append('PropertyAddress', data.propertyAddress);
    formData.append('City', data.city);
    formData.append('OwnershipType', data.ownershipType.toString());
    formData.append('PropertyCost', data.propertyCost.toString());
    formData.append('DownPayment', data.downPayment.toString());
    formData.append('LoanPurpose', data.loanPurpose.toString());

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/home/${customerId}`,
      formData
    );
    return response.data;
  },

  createVehicleLoan: async (customerId: number, data: VehicleLoanApplicationCreateDTO) => {
    const formData = new FormData();
    
    // Base fields
    formData.append('RequestedAmount', data.requestedLoanAmount.toString());
    formData.append('TenureMonths', data.tenureMonths.toString());
    if (data.familyEmergencyDetails.fullName) formData.append('RelationFullName', data.familyEmergencyDetails.fullName);
    if (data.familyEmergencyDetails.relationshipWithApplicant) formData.append('RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    if (data.familyEmergencyDetails.mobileNumber) formData.append('MobileNumber', data.familyEmergencyDetails.mobileNumber);
    if (data.familyEmergencyDetails.address) formData.append('RelationAddress', data.familyEmergencyDetails.address);
    if (data.addressInformation.presentAddress) formData.append('PresentAddress', data.addressInformation.presentAddress);
    if (data.addressInformation.permanentAddress) formData.append('PermanentAddress', data.addressInformation.permanentAddress);
    if (data.addressInformation.district) formData.append('District', data.addressInformation.district);
    if (data.addressInformation.state !== undefined) formData.append('State', data.addressInformation.state.toString());
    if (data.addressInformation.zipCode) formData.append('ZipCode', data.addressInformation.zipCode);
    if (data.personalDetails.signatureImage) {
      formData.append('SignatureImage', data.personalDetails.signatureImage);
    }
    
    // Vehicle loan specific fields
    formData.append('VehicleType', data.vehicleType.toString());
    formData.append('Manufacturer', data.manufacturer);
    formData.append('Model', data.model);
    formData.append('ManufacturingYear', data.manufacturingYear.toString());
    formData.append('OnRoadPrice', data.onRoadPrice.toString());
    formData.append('DownPayment', data.downPayment.toString());
    formData.append('LoanPurposeVehicle', data.loanPurposeVehicle.toString());

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/vehicle/${customerId}`,
      formData
    );
    return response.data;
  }
};

export const loanAPI = loanApplicationAPI;
