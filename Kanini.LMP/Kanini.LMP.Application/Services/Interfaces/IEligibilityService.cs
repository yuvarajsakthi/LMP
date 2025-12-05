using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.Application.Services.Interfaces
{
    public interface IEligibilityService
    {
        Task<EligibilityScoreDto> CalculateEligibilityAsync(IdDTO customerId, IdDTO loanProductId);
        Task<BoolDTO> IsEligibleForLoanAsync(IdDTO customerId, IdDTO? loanProductId = null);
        Task<List<IdDTO>> GetEligibleProductsAsync(IdDTO customerId);
        Task UpdateCustomerProfileAsync(IdDTO userId, EligibilityProfileRequest request);
    }
}