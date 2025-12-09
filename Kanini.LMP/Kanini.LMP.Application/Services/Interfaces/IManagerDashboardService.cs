using Kanini.LMP.Database.EntitiesDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IManagerDashboardService
    {
        // 1. Dashboard
        Task<DashboardStatsDTO> GetDashboardStatsAsync();

        // 2. Applied Loans
        Task<List<LoanApplicationDetailDTO>> GetAllLoanApplicationsAsync();
        Task<LoanApplicationDetailDTO?> GetLoanApplicationByIdAsync(int id);
        Task<List<LoanApplicationDetailDTO>> GetLoanApplicationsByStatusAsync(string status);
        Task<bool> UpdateApplicationStatusAsync(UpdateApplicationStatusDTO dto);
        Task<bool> DisburseLoanAsync(int loanApplicationBaseId);
    }
}
