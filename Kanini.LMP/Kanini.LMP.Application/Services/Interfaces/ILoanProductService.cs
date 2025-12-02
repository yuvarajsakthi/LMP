using Kanini.LMP.Database.EntitiesDtos;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface ILoanProductService
    {
        Task<IReadOnlyList<LoanProductDto>> GetActiveLoanProductsAsync();
    }
}
