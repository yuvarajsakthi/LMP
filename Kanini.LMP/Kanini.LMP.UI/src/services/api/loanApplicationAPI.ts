import axiosInstance from './axiosInstance';


export const loanApplicationAPI = {
  createPersonalLoan: async (customerId: number, data: {
    RequestedAmount: number;
    TenureMonths: number;
    EmploymentType: number;
    MonthlyIncome: number;
    WorkExperienceYears: number;
    LoanPurpose: number;
    District: string;
    State: number;
    ZipCode: number;
    PresentAddress: string;
    PermanentAddress: string;
    RelationFullName: string;
    RelationshipWithApplicant: string;
    MobileNumber: number;
    RelationAddress: string;
    SignatureImage?: File;
    DocumentUpload?: File;
  }) => {
    const formData = new FormData();
    
    formData.append('RequestedAmount', data.RequestedAmount.toString());
    formData.append('TenureMonths', data.TenureMonths.toString());
    formData.append('EmploymentType', data.EmploymentType.toString());
    formData.append('MonthlyIncome', data.MonthlyIncome.toString());
    formData.append('WorkExperienceYears', data.WorkExperienceYears.toString());
    formData.append('LoanPurpose', data.LoanPurpose.toString());
    formData.append('District', data.District);
    formData.append('State', data.State.toString());
    formData.append('ZipCode', data.ZipCode.toString());
    formData.append('PresentAddress', data.PresentAddress);
    formData.append('PermanentAddress', data.PermanentAddress);
    formData.append('RelationFullName', data.RelationFullName);
    formData.append('RelationshipWithApplicant', data.RelationshipWithApplicant);
    formData.append('MobileNumber', data.MobileNumber.toString());
    formData.append('RelationAddress', data.RelationAddress);
    if (data.SignatureImage) formData.append('SignatureImage', data.SignatureImage);
    if (data.DocumentUpload) formData.append('DocumentUpload', data.DocumentUpload);

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/personal/${customerId}`,
      formData
    );
    return response.data;
  },

  createHomeLoan: async (customerId: number, data: {
    RequestedAmount: number;
    TenureMonths: number;
    PropertyType: number;
    PropertyAddress: string;
    City: string;
    OwnershipType: number;
    PropertyCost: number;
    DownPayment: number;
    LoanPurpose: number;
    District: string;
    State: number;
    ZipCode: number;
    PresentAddress: string;
    PermanentAddress: string;
    RelationFullName: string;
    RelationshipWithApplicant: string;
    MobileNumber: number;
    RelationAddress: string;
    SignatureImage?: File;
  }) => {
    const formData = new FormData();
    
    formData.append('RequestedAmount', data.RequestedAmount.toString());
    formData.append('TenureMonths', data.TenureMonths.toString());
    formData.append('PropertyType', data.PropertyType.toString());
    formData.append('PropertyAddress', data.PropertyAddress);
    formData.append('City', data.City);
    formData.append('OwnershipType', data.OwnershipType.toString());
    formData.append('PropertyCost', data.PropertyCost.toString());
    formData.append('DownPayment', data.DownPayment.toString());
    formData.append('LoanPurpose', data.LoanPurpose.toString());
    formData.append('District', data.District);
    formData.append('State', data.State.toString());
    formData.append('ZipCode', data.ZipCode.toString());
    formData.append('PresentAddress', data.PresentAddress);
    formData.append('PermanentAddress', data.PermanentAddress);
    formData.append('RelationFullName', data.RelationFullName);
    formData.append('RelationshipWithApplicant', data.RelationshipWithApplicant);
    formData.append('MobileNumber', data.MobileNumber.toString());
    formData.append('RelationAddress', data.RelationAddress);
    if (data.SignatureImage) formData.append('SignatureImage', data.SignatureImage);

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/home/${customerId}`,
      formData
    );
    return response.data;
  },

  createVehicleLoan: async (customerId: number, data: {
    RequestedAmount: number;
    TenureMonths: number;
    VehicleType: number;
    Manufacturer: string;
    Model: string;
    ManufacturingYear: number;
    OnRoadPrice: number;
    DownPayment: number;
    LoanPurposeVehicle: number;
    District: string;
    State: number;
    ZipCode: number;
    PresentAddress: string;
    PermanentAddress: string;
    RelationFullName: string;
    RelationshipWithApplicant: string;
    MobileNumber: number;
    RelationAddress: string;
    SignatureImage?: File;
    DocumentUpload?: File;
  }) => {
    const formData = new FormData();
    
    formData.append('RequestedAmount', data.RequestedAmount.toString());
    formData.append('TenureMonths', data.TenureMonths.toString());
    formData.append('VehicleType', data.VehicleType.toString());
    formData.append('Manufacturer', data.Manufacturer);
    formData.append('Model', data.Model);
    formData.append('ManufacturingYear', data.ManufacturingYear.toString());
    formData.append('OnRoadPrice', data.OnRoadPrice.toString());
    formData.append('DownPayment', data.DownPayment.toString());
    formData.append('LoanPurposeVehicle', data.LoanPurposeVehicle.toString());
    formData.append('District', data.District);
    formData.append('State', data.State.toString());
    formData.append('ZipCode', data.ZipCode.toString());
    formData.append('PresentAddress', data.PresentAddress);
    formData.append('PermanentAddress', data.PermanentAddress);
    formData.append('RelationFullName', data.RelationFullName);
    formData.append('RelationshipWithApplicant', data.RelationshipWithApplicant);
    formData.append('MobileNumber', data.MobileNumber.toString());
    formData.append('RelationAddress', data.RelationAddress);
    if (data.SignatureImage) formData.append('SignatureImage', data.SignatureImage);
    if (data.DocumentUpload) formData.append('DocumentUpload', data.DocumentUpload);

    const response = await axiosInstance.post(
      `/api/LoanApplicationFlow/vehicle/${customerId}`,
      formData
    );
    return response.data;
  },

  getApplicationsByCustomerId: async (customerId: number) => {
    const response = await axiosInstance.get(
      `/api/LoanApplicationFlow/customer/${customerId}`
    );
    return response.data;
  }
};

export const loanAPI = loanApplicationAPI;
