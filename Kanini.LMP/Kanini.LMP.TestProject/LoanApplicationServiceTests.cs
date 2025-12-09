using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Kanini.LMP.Application.Services.Implementations;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.TestProject
{
    public class LoanApplicationServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<LoanApplicationService>> _mockLogger;
        private readonly Mock<IEligibilityService> _mockEligibilityService;
        private readonly Mock<IPdfService> _mockPdfService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly LoanApplicationService _service;

        public LoanApplicationServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<LoanApplicationService>>();
            _mockEligibilityService = new Mock<IEligibilityService>();
            _mockPdfService = new Mock<IPdfService>();
            _mockNotificationService = new Mock<INotificationService>();
            _service = new LoanApplicationService(_mockUnitOfWork.Object, _mockMapper.Object, _mockLogger.Object, 
                _mockEligibilityService.Object, _mockPdfService.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task GetPersonalLoanByIdAsync_ReturnsLoan_WhenLoanExists()
        {
            var loanId = 1;
            var loan = new PersonalLoanApplication { LoanApplicationBaseId = loanId };
            var loanDto = new PersonalLoanApplicationDTO { LoanApplicationBaseId = loanId };

            _mockUnitOfWork.Setup(u => u.PersonalLoanApplications.GetByIdAsync(loanId)).ReturnsAsync(loan);
            _mockMapper.Setup(m => m.Map<PersonalLoanApplicationDTO>(loan)).Returns(loanDto);

            var result = await _service.GetPersonalLoanByIdAsync(new IdDTO { Id = loanId });

            Assert.NotNull(result);
            Assert.Equal(loanId, result.LoanApplicationBaseId);
        }

        [Fact]
        public async Task GetPersonalLoanByIdAsync_ReturnsNull_WhenLoanDoesNotExist()
        {
            _mockUnitOfWork.Setup(u => u.PersonalLoanApplications.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((PersonalLoanApplication?)null);

            var result = await _service.GetPersonalLoanByIdAsync(new IdDTO { Id = 999 });

            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllPersonalLoansAsync_ReturnsAllLoans()
        {
            var loans = new List<PersonalLoanApplication> { new PersonalLoanApplication(), new PersonalLoanApplication() };
            var loanDtos = new List<PersonalLoanApplicationDTO> { new PersonalLoanApplicationDTO(), new PersonalLoanApplicationDTO() };

            _mockUnitOfWork.Setup(u => u.PersonalLoanApplications.GetAllAsync(null)).ReturnsAsync(loans);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<PersonalLoanApplicationDTO>>(loans)).Returns(loanDtos);

            var result = await _service.GetAllPersonalLoansAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetHomeLoanByIdAsync_ReturnsLoan_WhenLoanExists()
        {
            var loanId = 1;
            var loan = new HomeLoanApplication { LoanApplicationBaseId = loanId };
            var loanDto = new HomeLoanApplicationDTO { LoanApplicationBaseId = loanId };

            _mockUnitOfWork.Setup(u => u.HomeLoanApplications.GetByIdAsync(loanId)).ReturnsAsync(loan);
            _mockMapper.Setup(m => m.Map<HomeLoanApplicationDTO>(loan)).Returns(loanDto);

            var result = await _service.GetHomeLoanByIdAsync(new IdDTO { Id = loanId });

            Assert.NotNull(result);
            Assert.Equal(loanId, result.LoanApplicationBaseId);
        }

        [Fact]
        public async Task GetAllVehicleLoansAsync_ReturnsAllLoans()
        {
            var loans = new List<VehicleLoanApplication> { new VehicleLoanApplication(), new VehicleLoanApplication() };
            var loanDtos = new List<VehicleLoanApplicationDTO> { new VehicleLoanApplicationDTO(), new VehicleLoanApplicationDTO() };

            _mockUnitOfWork.Setup(u => u.VehicleLoanApplications.GetAllAsync(null)).ReturnsAsync(loans);
            _mockMapper.Setup(m => m.Map<IReadOnlyList<VehicleLoanApplicationDTO>>(loans)).Returns(loanDtos);

            var result = await _service.GetAllVehicleLoansAsync();

            Assert.Equal(2, result.Count);
        }
    }
}
