using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class FaqService : IFaqService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FaqService> _logger;

        public FaqService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FaqService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<FaqDTO> Add(FaqDTO entity)
        {
            try
            {
                var faq = _mapper.Map<Faq>(entity);
                var created = await _unitOfWork.Faqs.AddAsync(faq);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<FaqDTO>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding FAQ for customer ID: {CustomerId}", entity.CustomerId);
                throw new InvalidOperationException("Failed to create FAQ", ex);
            }
        }

        public async Task<IReadOnlyList<FaqDTO>> GetAll()
        {
            try
            {
                var faqs = await _unitOfWork.Faqs.GetAllAsync();
                return _mapper.Map<List<FaqDTO>>(faqs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all FAQs");
                throw new InvalidOperationException("Failed to retrieve FAQs", ex);
            }
        }

        public async Task<FaqDTO?> GetById(IdDTO id)
        {
            try
            {
                var faq = await _unitOfWork.Faqs.GetByIdAsync(id.Id);
                return faq == null ? null : _mapper.Map<FaqDTO>(faq);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQ with ID: {Id}", id);
                throw new InvalidOperationException("Failed to retrieve FAQ", ex);
            }
        }

        public async Task<IReadOnlyList<FaqDTO>> GetByCustomerId(IdDTO customerId)
        {
            try
            {
                var faqs = await _unitOfWork.Faqs.GetAllAsync(f => f.CustomerId == customerId.Id);
                return _mapper.Map<List<FaqDTO>>(faqs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving FAQs for customer ID: {CustomerId}", customerId);
                throw new InvalidOperationException("Failed to retrieve customer FAQs", ex);
            }
        }

        public async Task<FaqDTO> Update(FaqDTO entity)
        {
            try
            {
                var faq = _mapper.Map<Faq>(entity);
                var updated = await _unitOfWork.Faqs.UpdateAsync(faq);
                await _unitOfWork.SaveChangesAsync();
                return _mapper.Map<FaqDTO>(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating FAQ with ID: {Id}", entity.Id);
                throw new InvalidOperationException("Failed to update FAQ", ex);
            }
        }

        public async Task Delete(IdDTO id)
        {
            try
            {
                await _unitOfWork.Faqs.DeleteAsync(id.Id);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting FAQ with ID: {Id}", id);
                throw new InvalidOperationException("Failed to delete FAQ", ex);
            }
        }
    }
}