using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            
        }

        public async Task<CustomerDto> Add(CustomerDto entity)
        {

                var created = await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer created successfully with ID: {CustomerId}", created.CustomerId);
                return _mapper.Map<CustomerDto>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding customer for user ID: {UserId}", entity.UserId);
                throw new InvalidOperationException("Failed to create customer profile", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", id);
                    throw new KeyNotFoundException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                await _unitOfWork.Customers.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer deleted successfully with ID: {CustomerId}", id);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", id);
                throw new InvalidOperationException("Failed to delete customer", ex);
            }
        }

        public async Task<IReadOnlyList<CustomerDto>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all customers");

                var customers = await _unitOfWork.Customers.GetAllAsync();
                var customerDtos = _mapper.Map<List<CustomerDto>>(customers);

                _logger.LogInformation("Retrieved {Count} customers", customerDtos.Count);
                return customerDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                throw new InvalidOperationException("Failed to retrieve customers", ex);
            }
        }

        public async Task<CustomerDto?> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Retrieving customer with ID: {CustomerId}", id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", id);
                    return null;
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);
                _logger.LogInformation("Customer retrieved successfully with ID: {CustomerId}", id);
                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", id);
                throw new InvalidOperationException("Failed to retrieve customer", ex);
            }
        }

        public async Task<CustomerDto> Update(CustomerDto entity)
        {
            try
            {
                _logger.LogInformation("Updating customer with ID: {CustomerId}", entity.CustomerId);

                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(entity.CustomerId);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for update", entity.CustomerId);
                    throw new KeyNotFoundException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                var customer = _mapper.Map<Customer>(entity);
                customer.UpdatedAt = DateTime.UtcNow;
                customer.ProfileImage = existingCustomer.ProfileImage; // Preserve existing image

                var updated = await _unitOfWork.Customers.UpdateAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer updated successfully with ID: {CustomerId}", entity.CustomerId);
                return _mapper.Map<CustomerDto>(updated);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", entity.CustomerId);
                throw new InvalidOperationException("Failed to update customer", ex);
            }
        }

        public async Task<CustomerDto?> GetByUserIdAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Retrieving customer by user ID: {UserId}", userId);

                var customer = await _unitOfWork.Customers.GetAsync(c => c.UserId == userId);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with user ID {UserId} not found", userId);
                    return null;
                }

                var customerDto = _mapper.Map<CustomerDto>(customer);
                _logger.LogInformation("Customer retrieved successfully by user ID: {UserId}", userId);
                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer by user ID: {UserId}", userId);
                throw new InvalidOperationException("Failed to retrieve customer by user ID", ex);
            }
        }


    }
}
