using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.Common;
using System.Text;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class PdfService : IPdfService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PdfService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ByteArrayDTO> GenerateLoanApplicationPdfAsync(IdDTO applicationId)
        {
            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(applicationId.Id);
            if (personalLoan != null)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(personalLoan.CustomerId);
                if (customer == null) throw new ArgumentException("Customer not found");
                return new ByteArrayDTO { Data = Encoding.UTF8.GetBytes(GeneratePersonalLoanPdf(personalLoan, customer)) };
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(applicationId.Id);
            if (homeLoan != null)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(homeLoan.CustomerId);
                if (customer == null) throw new ArgumentException("Customer not found");
                return new ByteArrayDTO { Data = Encoding.UTF8.GetBytes(GenerateHomeLoanPdf(homeLoan, customer)) };
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(applicationId.Id);
            if (vehicleLoan != null)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(vehicleLoan.CustomerId);
                if (customer == null) throw new ArgumentException("Customer not found");
                return new ByteArrayDTO { Data = Encoding.UTF8.GetBytes(GenerateVehicleLoanPdf(vehicleLoan, customer)) };
            }

            throw new ArgumentException("Application not found");
        }

        private string GeneratePersonalLoanPdf(PersonalLoanApplication loan, dynamic customer)
        {
            return $@"
========================================
  PERSONAL LOAN APPLICATION DOCUMENT
========================================

Application ID: {loan.LoanApplicationBaseId}
Application Date: {loan.SubmissionDate}
Status: {loan.Status}

----------------------------------------
CUSTOMER INFORMATION
----------------------------------------
Phone: {customer?.PhoneNumber ?? "N/A"}
Date of Birth: {customer?.DateOfBirth}
Gender: {customer?.Gender}

----------------------------------------
LOAN DETAILS
----------------------------------------
Requested Amount: ₹{loan.RequestedLoanAmount:N2}
Tenure: {loan.TenureMonths} months
Interest Rate: {loan.InterestRate ?? 0}%

----------------------------------------
EMPLOYMENT DETAILS
----------------------------------------
Employment Type: {loan.EmploymentType}
Monthly Income: ₹{loan.MonthlyIncome:N2}
Work Experience: {loan.WorkExperienceYears} years
Loan Purpose: {loan.LoanPurpose}

----------------------------------------

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

This is a system-generated document.

========================================
";
        }

        private string GenerateHomeLoanPdf(HomeLoanApplication loan, dynamic customer)
        {
            return $@"
========================================
    HOME LOAN APPLICATION DOCUMENT
========================================

Application ID: {loan.LoanApplicationBaseId}
Application Date: {loan.SubmissionDate}
Status: {loan.Status}

----------------------------------------
CUSTOMER INFORMATION
----------------------------------------
Phone: {customer?.PhoneNumber ?? "N/A"}
Date of Birth: {customer?.DateOfBirth}
Gender: {customer?.Gender}

----------------------------------------
LOAN DETAILS
----------------------------------------
Requested Amount: ₹{loan.RequestedLoanAmount:N2}
Tenure: {loan.TenureMonths} months
Interest Rate: {loan.InterestRate ?? 0}%

----------------------------------------
PROPERTY DETAILS
----------------------------------------
Property Type: {loan.PropertyType}
Property Address: {loan.PropertyAddress}
City: {loan.City}
Zip Code: {loan.ZipCode}
Ownership Type: {loan.OwnershipType}
Property Cost: ₹{loan.PropertyCost:N2}
Down Payment: ₹{loan.DownPayment:N2}
Loan Purpose: {loan.LoanPurpose}

----------------------------------------

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

This is a system-generated document.

========================================
";
        }

        private string GenerateVehicleLoanPdf(VehicleLoanApplication loan, dynamic customer)
        {
            return $@"
========================================
   VEHICLE LOAN APPLICATION DOCUMENT
========================================

Application ID: {loan.LoanApplicationBaseId}
Application Date: {loan.SubmissionDate}
Status: {loan.Status}

----------------------------------------
CUSTOMER INFORMATION
----------------------------------------
Phone: {customer?.PhoneNumber ?? "N/A"}
Date of Birth: {customer?.DateOfBirth}
Gender: {customer?.Gender}

----------------------------------------
LOAN DETAILS
----------------------------------------
Requested Amount: ₹{loan.RequestedLoanAmount:N2}
Tenure: {loan.TenureMonths} months
Interest Rate: {loan.InterestRate ?? 0}%

----------------------------------------
VEHICLE DETAILS
----------------------------------------
Vehicle Type: {loan.VehicleType}
Manufacturer: {loan.Manufacturer}
Model: {loan.Model}
Manufacturing Year: {loan.ManufacturingYear}
On-Road Price: ₹{loan.OnRoadPrice:N2}
Down Payment: ₹{loan.DownPayment:N2}
Loan Purpose: {loan.LoanPurposeVehicle}

----------------------------------------

Generated on: {DateTime.Now:dd MMM yyyy HH:mm}

This is a system-generated document.

========================================
";
        }
    }
}
