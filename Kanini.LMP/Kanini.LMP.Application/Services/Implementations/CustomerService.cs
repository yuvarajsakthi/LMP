using AutoMapper;
using Kanini.LMP.Application.Constants;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;
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
            _logger = logger;
        }

        public async Task<CustomerDTO> Add(CustomerCreateDTO entity)
        {
            try
            {
                _logger.LogInformation("Adding new customer for user ID: {UserId}", entity.UserId);

                var customer = _mapper.Map<Customer>(entity);
                customer.ProfileImage = new byte[] { 0x00 };
                customer.UpdatedAt = DateTime.UtcNow;

                var created = await _unitOfWork.Customers.AddAsync(customer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer created successfully with ID: {CustomerId}", created.CustomerId);
                return _mapper.Map<CustomerDTO>(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding customer for user ID: {UserId}", entity.UserId);
                throw new InvalidOperationException("Failed to create customer profile", ex);
            }
        }

        public async Task Delete(IdDTO request)
        {
            try
            {
                _logger.LogInformation("Deleting customer with ID: {CustomerId}", request.Id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for deletion", request.Id);
                    throw new KeyNotFoundException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                await _unitOfWork.Customers.DeleteAsync(request.Id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer deleted successfully with ID: {CustomerId}", request.Id);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, "Error deleting customer with ID: {CustomerId}", request.Id);
                throw new InvalidOperationException("Failed to delete customer", ex);
            }
        }

        public async Task<IReadOnlyList<CustomerDTO>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all customers");

                var customers = await _unitOfWork.Customers.GetAllAsync();
                var customerDtos = _mapper.Map<List<CustomerDTO>>(customers);

                _logger.LogInformation("Retrieved {Count} customers", customerDtos.Count);
                return customerDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                throw new InvalidOperationException("Failed to retrieve customers", ex);
            }
        }

        public async Task<CustomerDTO?> GetById(IdDTO request)
        {
            try
            {
                _logger.LogInformation("Retrieving customer with ID: {CustomerId}", request.Id);

                var customer = await _unitOfWork.Customers.GetByIdAsync(request.Id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found", request.Id);
                    return null;
                }

                var customerDto = _mapper.Map<CustomerDTO>(customer);
                _logger.LogInformation("Customer retrieved successfully with ID: {CustomerId}", request.Id);
                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer with ID: {CustomerId}", request.Id);
                throw new InvalidOperationException("Failed to retrieve customer", ex);
            }
        }

        public async Task<CustomerDTO> Update(IdDTO customerId, CustomerUpdateDTO entity)
        {
            try
            {
                _logger.LogInformation("Updating customer with ID: {CustomerId}", customerId);

                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(customerId.Id);
                if (existingCustomer == null)
                {
                    _logger.LogWarning("Customer with ID {CustomerId} not found for update", customerId);
                    throw new KeyNotFoundException(ApplicationConstants.ErrorMessages.CustomerNotFound);
                }

                _mapper.Map(entity, existingCustomer);
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                var updated = await _unitOfWork.Customers.UpdateAsync(existingCustomer);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Customer updated successfully with ID: {CustomerId}", customerId);
                return _mapper.Map<CustomerDTO>(updated);
            }
            catch (Exception ex) when (!(ex is KeyNotFoundException))
            {
                _logger.LogError(ex, "Error updating customer with ID: {CustomerId}", customerId);
                throw new InvalidOperationException("Failed to update customer", ex);
            }
        }

        public async Task<CustomerDTO?> GetByUserIdAsync(IdDTO request)
        {
            try
            {
                _logger.LogInformation("Retrieving customer by user ID: {UserId}", request.Id);

                var customer = await _unitOfWork.Customers.GetAsync(c => c.UserId == request.Id);
                if (customer == null)
                {
                    _logger.LogWarning("Customer with user ID {UserId} not found", request.Id);
                    return null;
                }

                var customerDto = _mapper.Map<CustomerDTO>(customer);
                _logger.LogInformation("Customer retrieved successfully by user ID: {UserId}", request.Id);
                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer by user ID: {UserId}", request.Id);
                throw new InvalidOperationException("Failed to retrieve customer by user ID", ex);
            }
        }
    }
}
