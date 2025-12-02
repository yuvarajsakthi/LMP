using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.EntitiesDtos;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class LoanProductService : ILoanProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanProductService> _logger;

        public LoanProductService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LoanProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IReadOnlyList<LoanProductDto>> GetActiveLoanProductsAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all active loan products");

                var loanProducts = await _unitOfWork.LoanProducts.GetAllAsync(lp => lp.IsActive);
                var loanProductDtos = _mapper.Map<List<LoanProductDto>>(loanProducts);

                _logger.LogInformation("Retrieved {Count} active loan products", loanProductDtos.Count);
                return loanProductDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active loan products");
                throw new InvalidOperationException("Failed to retrieve active loan products", ex);
            }
        }
    }
}
