import axios from 'axios';
import type {
  PersonalLoanApplicationCreateDTO,
  HomeLoanApplicationCreateDTO,
  VehicleLoanApplicationCreateDTO
} from '../../types/loanApplicationCreate';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

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
    formData.append('PersonalDetails.FullName', data.personalDetails.fullName);
    formData.append('PersonalDetails.DateOfBirth', data.personalDetails.dateOfBirth);
    formData.append('PersonalDetails.DistrictOfBirth', data.personalDetails.districtOfBirth);
    formData.append('PersonalDetails.PANNumber', data.personalDetails.panNumber);
    formData.append('PersonalDetails.EducationQualification', data.personalDetails.educationQualification.toString());
    formData.append('PersonalDetails.ResidentialStatus', data.personalDetails.residentialStatus.toString());
    formData.append('PersonalDetails.Gender', data.personalDetails.gender.toString());
    formData.append('PersonalDetails.SignatureImage', data.personalDetails.signatureImage);
    formData.append('PersonalDetails.IDProofImage', data.personalDetails.idProofImage);

    // Address Information
    formData.append('AddressInformation.PresentAddress', data.addressInformation.presentAddress);
    formData.append('AddressInformation.PermanentAddress', data.addressInformation.permanentAddress);
    formData.append('AddressInformation.District', data.addressInformation.district);
    formData.append('AddressInformation.State', data.addressInformation.state.toString());
    formData.append('AddressInformation.ZipCode', data.addressInformation.zipCode);
    if (data.addressInformation.emailId) {
      formData.append('AddressInformation.EmailId', data.addressInformation.emailId);
    }
    formData.append('AddressInformation.MobileNumber1', data.addressInformation.mobileNumber1);
    if (data.addressInformation.mobileNumber2) {
      formData.append('AddressInformation.MobileNumber2', data.addressInformation.mobileNumber2);
    }

    // Family Emergency Details
    formData.append('FamilyEmergencyDetails.FullName', data.familyEmergencyDetails.fullName);
    formData.append('FamilyEmergencyDetails.RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    formData.append('FamilyEmergencyDetails.MobileNumber', data.familyEmergencyDetails.mobileNumber);
    formData.append('FamilyEmergencyDetails.Address', data.familyEmergencyDetails.address);

    // Declaration
    formData.append('Declaration.Name', data.declaration.name);
    formData.append('Declaration.Amount', data.declaration.amount.toString());
    formData.append('Declaration.Description', data.declaration.description);
    formData.append('Declaration.Purpose', data.declaration.purpose);

    const response = await axios.post(
      `${API_BASE_URL}/api/LoanApplicationFlow/personal/${customerId}`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }
    );
    return response.data;
  },

  createHomeLoan: async (customerId: number, data: HomeLoanApplicationCreateDTO) => {
    const formData = new FormData();
    
    // Basic loan details
    formData.append('CustomerId', customerId.toString());
    formData.append('TenureMonths', data.tenureMonths.toString());
    formData.append('RequestedLoanAmount', data.requestedLoanAmount.toString());
    
    // Home loan specific
    formData.append('PropertyType', data.propertyType.toString());
    formData.append('PropertyAddress', data.propertyAddress);
    formData.append('City', data.city);
    formData.append('ZipCode', data.zipCode.toString());
    formData.append('OwnershipType', data.ownershipType.toString());
    formData.append('PropertyCost', data.propertyCost.toString());
    formData.append('DownPayment', data.downPayment.toString());
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
    formData.append('PersonalDetails.FullName', data.personalDetails.fullName);
    formData.append('PersonalDetails.DateOfBirth', data.personalDetails.dateOfBirth);
    formData.append('PersonalDetails.DistrictOfBirth', data.personalDetails.districtOfBirth);
    formData.append('PersonalDetails.PANNumber', data.personalDetails.panNumber);
    formData.append('PersonalDetails.EducationQualification', data.personalDetails.educationQualification.toString());
    formData.append('PersonalDetails.ResidentialStatus', data.personalDetails.residentialStatus.toString());
    formData.append('PersonalDetails.Gender', data.personalDetails.gender.toString());
    formData.append('PersonalDetails.SignatureImage', data.personalDetails.signatureImage);
    formData.append('PersonalDetails.IDProofImage', data.personalDetails.idProofImage);

    // Address Information
    formData.append('AddressInformation.PresentAddress', data.addressInformation.presentAddress);
    formData.append('AddressInformation.PermanentAddress', data.addressInformation.permanentAddress);
    formData.append('AddressInformation.District', data.addressInformation.district);
    formData.append('AddressInformation.State', data.addressInformation.state.toString());
    formData.append('AddressInformation.ZipCode', data.addressInformation.zipCode);
    if (data.addressInformation.emailId) {
      formData.append('AddressInformation.EmailId', data.addressInformation.emailId);
    }
    formData.append('AddressInformation.MobileNumber1', data.addressInformation.mobileNumber1);
    if (data.addressInformation.mobileNumber2) {
      formData.append('AddressInformation.MobileNumber2', data.addressInformation.mobileNumber2);
    }

    // Family Emergency Details
    formData.append('FamilyEmergencyDetails.FullName', data.familyEmergencyDetails.fullName);
    formData.append('FamilyEmergencyDetails.RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    formData.append('FamilyEmergencyDetails.MobileNumber', data.familyEmergencyDetails.mobileNumber);
    formData.append('FamilyEmergencyDetails.Address', data.familyEmergencyDetails.address);

    // Declaration
    formData.append('Declaration.Name', data.declaration.name);
    formData.append('Declaration.Amount', data.declaration.amount.toString());
    formData.append('Declaration.Description', data.declaration.description);
    formData.append('Declaration.Purpose', data.declaration.purpose);

    const response = await axios.post(
      `${API_BASE_URL}/api/LoanApplicationFlow/home/${customerId}`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }
    );
    return response.data;
  },

  createVehicleLoan: async (customerId: number, data: VehicleLoanApplicationCreateDTO) => {
    const formData = new FormData();
    
    // Basic loan details
    formData.append('CustomerId', customerId.toString());
    formData.append('TenureMonths', data.tenureMonths.toString());
    formData.append('RequestedLoanAmount', data.requestedLoanAmount.toString());
    
    // Vehicle loan specific
    formData.append('VehicleType', data.vehicleType.toString());
    formData.append('Manufacturer', data.manufacturer);
    formData.append('Model', data.model);
    formData.append('ManufacturingYear', data.manufacturingYear.toString());
    formData.append('OnRoadPrice', data.onRoadPrice.toString());
    formData.append('DownPayment', data.downPayment.toString());
    formData.append('LoanPurposeVehicle', data.loanPurposeVehicle.toString());

    // Documents
    data.documents.forEach((doc, index) => {
      formData.append(`Documents[${index}].DocumentName`, doc.documentName);
      formData.append(`Documents[${index}].DocumentType`, doc.documentType.toString());
      if (doc.documentFile) {
        formData.append(`Documents[${index}].DocumentFile`, doc.documentFile);
      }
    });

    // Personal Details
    formData.append('PersonalDetails.FullName', data.personalDetails.fullName);
    formData.append('PersonalDetails.DateOfBirth', data.personalDetails.dateOfBirth);
    formData.append('PersonalDetails.DistrictOfBirth', data.personalDetails.districtOfBirth);
    formData.append('PersonalDetails.PANNumber', data.personalDetails.panNumber);
    formData.append('PersonalDetails.EducationQualification', data.personalDetails.educationQualification.toString());
    formData.append('PersonalDetails.ResidentialStatus', data.personalDetails.residentialStatus.toString());
    formData.append('PersonalDetails.Gender', data.personalDetails.gender.toString());
    formData.append('PersonalDetails.SignatureImage', data.personalDetails.signatureImage);
    formData.append('PersonalDetails.IDProofImage', data.personalDetails.idProofImage);

    // Address Information
    formData.append('AddressInformation.PresentAddress', data.addressInformation.presentAddress);
    formData.append('AddressInformation.PermanentAddress', data.addressInformation.permanentAddress);
    formData.append('AddressInformation.District', data.addressInformation.district);
    formData.append('AddressInformation.State', data.addressInformation.state.toString());
    formData.append('AddressInformation.ZipCode', data.addressInformation.zipCode);
    if (data.addressInformation.emailId) {
      formData.append('AddressInformation.EmailId', data.addressInformation.emailId);
    }
    formData.append('AddressInformation.MobileNumber1', data.addressInformation.mobileNumber1);
    if (data.addressInformation.mobileNumber2) {
      formData.append('AddressInformation.MobileNumber2', data.addressInformation.mobileNumber2);
    }

    // Family Emergency Details
    formData.append('FamilyEmergencyDetails.FullName', data.familyEmergencyDetails.fullName);
    formData.append('FamilyEmergencyDetails.RelationshipWithApplicant', data.familyEmergencyDetails.relationshipWithApplicant);
    formData.append('FamilyEmergencyDetails.MobileNumber', data.familyEmergencyDetails.mobileNumber);
    formData.append('FamilyEmergencyDetails.Address', data.familyEmergencyDetails.address);

    // Declaration
    formData.append('Declaration.Name', data.declaration.name);
    formData.append('Declaration.Amount', data.declaration.amount.toString());
    formData.append('Declaration.Description', data.declaration.description);
    formData.append('Declaration.Purpose', data.declaration.purpose);

    const response = await axios.post(
      `${API_BASE_URL}/api/LoanApplicationFlow/vehicle/${customerId}`,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      }
    );
    return response.data;
  }
};

export const loanAPI = loanApplicationAPI;
