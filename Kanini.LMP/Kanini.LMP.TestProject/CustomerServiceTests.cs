using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.Common;

namespace Kanini.LMP.TestProject
{
    public class CustomerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _service = new CustomerService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetById_ReturnsCustomer_WhenCustomerExists()
        {
            var customerId = 1;
            var customer = new Customer { CustomerId = customerId, UserId = 1 };
            var customerDto = new CustomerDTO { CustomerId = customerId };

            _mockUnitOfWork.Setup(u => u.Customers.GetByIdAsync(customerId)).ReturnsAsync(customer);
            _mockMapper.Setup(m => m.Map<CustomerDTO>(customer)).Returns(customerDto);

            var result = await _service.GetById(new IdDTO { Id = customerId });

            Assert.NotNull(result);
            Assert.Equal(customerId, result.CustomerId);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenCustomerDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Customers.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

            var result = await _service.GetById(new IdDTO { Id = 999 });

            Assert.Null(result);
        }

        [Fact]
        public async Task Add_CreatesCustomer_Successfully()
        {
            var createDto = new CustomerCreateDTO { UserId = 1 };
            var customer = new Customer { CustomerId = 1, UserId = 1 };
            var customerDto = new CustomerDTO { CustomerId = 1 };

            _mockMapper.Setup(m => m.Map<Customer>(createDto)).Returns(customer);
            _mockUnitOfWork.Setup(u => u.Customers.AddAsync(It.IsAny<Customer>())).ReturnsAsync(customer);
            _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<CustomerDTO>(customer)).Returns(customerDto);

            var result = await _service.Add(createDto);

            Assert.NotNull(result);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Delete_ThrowsKeyNotFoundException_WhenCustomerDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.Customers.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Customer?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.Delete(new IdDTO { Id = 999 }));
        }

        [Fact]
        public async Task GetAll_ReturnsAllCustomers()
        {
            var customers = new List<Customer> { new Customer { CustomerId = 1 }, new Customer { CustomerId = 2 } };
            var customerDtos = new List<CustomerDTO> { new CustomerDTO { CustomerId = 1 }, new CustomerDTO { CustomerId = 2 } };

            _mockUnitOfWork.Setup(u => u.Customers.GetAllAsync(null)).ReturnsAsync(customers);
            _mockMapper.Setup(m => m.Map<List<CustomerDTO>>(customers)).Returns(customerDtos);

            var result = await _service.GetAll();

            Assert.Equal(2, result.Count);
        }
    }
}
