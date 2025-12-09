using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ILoanApplicationService
    {
        // Personal Loan
        Task<PersonalLoanApplicationDTO> CreatePersonalLoanAsync(PersonalLoanApplicationDTO dto, IdDTO customerId);
        Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetAllPersonalLoansAsync();
        Task<PersonalLoanApplicationDTO?> GetPersonalLoanByIdAsync(IdDTO id);

        // Home Loan
        Task<HomeLoanApplicationDTO> CreateHomeLoanAsync(HomeLoanApplicationDTO dto, IdDTO customerId);
        Task<IReadOnlyList<HomeLoanApplicationDTO>> GetAllHomeLoansAsync();
        Task<HomeLoanApplicationDTO?> GetHomeLoanByIdAsync(IdDTO id);

        // Vehicle Loan
        Task<VehicleLoanApplicationDTO> CreateVehicleLoanAsync(VehicleLoanApplicationDTO dto, IdDTO customerId);
        Task<IReadOnlyList<VehicleLoanApplicationDTO>> GetAllVehicleLoansAsync();
        Task<VehicleLoanApplicationDTO?> GetVehicleLoanByIdAsync(IdDTO id);

        // Customer Dashboard methods
        Task<IEnumerable<dynamic>> GetApplicationsByCustomerIdAsync(IdDTO customerId);
        Task<IEnumerable<dynamic>> GetRecentApplicationsAsync(IdDTO customerId, IdDTO count);
        Task<IEnumerable<dynamic>> GetCustomerApplicationsAsync(IdDTO customerId);
        Task<dynamic> UpdateLoanStatusAsync(IdDTO loanId, ApplicationStatus status, string? rejectionReason = null);
    }
}