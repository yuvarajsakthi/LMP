using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.UnitOfWork;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.Common;
using Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanApplicationService> _logger;
        private readonly IEligibilityService _eligibilityService;
        private readonly IPdfService _pdfService;

        public LoanApplicationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<LoanApplicationService> logger, IEligibilityService eligibilityService, IPdfService pdfService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _eligibilityService = eligibilityService;
            _pdfService = pdfService;
        }

        public async Task<PersonalLoanApplicationDTO> CreatePersonalLoanAsync(PersonalLoanApplicationDTO dto, IdDTO customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, new IdDTO { Id = (int)LoanType.Personal });
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");

            var entity = _mapper.Map<PersonalLoanApplication>(dto);
            entity.CustomerId = customerId.Id;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _unitOfWork.PersonalLoanApplications.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _pdfService.GenerateLoanApplicationPdfAsync(new IdDTO { Id = created.LoanApplicationBaseId });
            await CreateNotificationAsync(customerId.Id, $"Personal loan application #{created.LoanApplicationBaseId} submitted successfully");

            return _mapper.Map<PersonalLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetAllPersonalLoansAsync()
        {
            var loans = await _unitOfWork.PersonalLoanApplications.GetAllAsync();
            return _mapper.Map<IReadOnlyList<PersonalLoanApplicationDTO>>(loans);
        }

        public async Task<PersonalLoanApplicationDTO?> GetPersonalLoanByIdAsync(IdDTO id)
        {
            var loan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(id.Id);
            return loan == null ? null : _mapper.Map<PersonalLoanApplicationDTO>(loan);
        }

        public async Task<HomeLoanApplicationDTO> CreateHomeLoanAsync(HomeLoanApplicationDTO dto, IdDTO customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, new IdDTO { Id = (int)LoanType.Home });
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");

            var entity = _mapper.Map<HomeLoanApplication>(dto);
            entity.CustomerId = customerId.Id;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _unitOfWork.HomeLoanApplications.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _pdfService.GenerateLoanApplicationPdfAsync(new IdDTO { Id = created.LoanApplicationBaseId });
            await CreateNotificationAsync(customerId.Id, $"Home loan application #{created.LoanApplicationBaseId} submitted successfully");

            return _mapper.Map<HomeLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<HomeLoanApplicationDTO>> GetAllHomeLoansAsync()
        {
            var loans = await _unitOfWork.HomeLoanApplications.GetAllAsync();
            return _mapper.Map<IReadOnlyList<HomeLoanApplicationDTO>>(loans);
        }

        public async Task<HomeLoanApplicationDTO?> GetHomeLoanByIdAsync(IdDTO id)
        {
            var loan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(id.Id);
            return loan == null ? null : _mapper.Map<HomeLoanApplicationDTO>(loan);
        }

        public async Task<VehicleLoanApplicationDTO> CreateVehicleLoanAsync(VehicleLoanApplicationDTO dto, IdDTO customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, new IdDTO { Id = (int)LoanType.Vehicle });
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");

            var entity = _mapper.Map<VehicleLoanApplication>(dto);
            entity.CustomerId = customerId.Id;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _unitOfWork.VehicleLoanApplications.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _pdfService.GenerateLoanApplicationPdfAsync(new IdDTO { Id = created.LoanApplicationBaseId });
            await CreateNotificationAsync(customerId.Id, $"Vehicle loan application #{created.LoanApplicationBaseId} submitted successfully");

            return _mapper.Map<VehicleLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<VehicleLoanApplicationDTO>> GetAllVehicleLoansAsync()
        {
            var loans = await _unitOfWork.VehicleLoanApplications.GetAllAsync();
            return _mapper.Map<IReadOnlyList<VehicleLoanApplicationDTO>>(loans);
        }

        public async Task<VehicleLoanApplicationDTO?> GetVehicleLoanByIdAsync(IdDTO id)
        {
            var loan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(id.Id);
            return loan == null ? null : _mapper.Map<VehicleLoanApplicationDTO>(loan);
        }

        public async Task<IEnumerable<dynamic>> GetRecentApplicationsAsync(IdDTO customerId, IdDTO count)
        {
            var personalLoans = await _unitOfWork.PersonalLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            var homeLoans = await _unitOfWork.HomeLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            var vehicleLoans = await _unitOfWork.VehicleLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            
            var allLoans = personalLoans.Cast<dynamic>().Concat(homeLoans).Concat(vehicleLoans);
            return allLoans.OrderByDescending(l => l.SubmissionDate).Take(count.Id).Select(l => new { l.LoanApplicationBaseId, l.Status, l.SubmissionDate });
        }

        public async Task<IEnumerable<dynamic>> GetCustomerApplicationsAsync(IdDTO customerId)
        {
            var personalLoans = await _unitOfWork.PersonalLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            var homeLoans = await _unitOfWork.HomeLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            var vehicleLoans = await _unitOfWork.VehicleLoanApplications.GetAllAsync(l => l.CustomerId == customerId.Id);
            
            var allLoans = personalLoans.Cast<dynamic>().Concat(homeLoans).Concat(vehicleLoans);
            return allLoans.Select(l => new { l.LoanApplicationBaseId, l.Status, l.SubmissionDate });
        }

        public async Task<dynamic> UpdateLoanStatusAsync(IdDTO loanId, ApplicationStatus status)
        {
            var personalLoan = await _unitOfWork.PersonalLoanApplications.GetByIdAsync(loanId.Id);
            if (personalLoan != null)
            {
                personalLoan.Status = status;
                await _unitOfWork.PersonalLoanApplications.UpdateAsync(personalLoan);
                await _unitOfWork.SaveChangesAsync();
                return new { Status = status };
            }

            var homeLoan = await _unitOfWork.HomeLoanApplications.GetByIdAsync(loanId.Id);
            if (homeLoan != null)
            {
                homeLoan.Status = status;
                await _unitOfWork.HomeLoanApplications.UpdateAsync(homeLoan);
                await _unitOfWork.SaveChangesAsync();
                return new { Status = status };
            }

            var vehicleLoan = await _unitOfWork.VehicleLoanApplications.GetByIdAsync(loanId.Id);
            if (vehicleLoan != null)
            {
                vehicleLoan.Status = status;
                await _unitOfWork.VehicleLoanApplications.UpdateAsync(vehicleLoan);
                await _unitOfWork.SaveChangesAsync();
                return new { Status = status };
            }

            throw new ArgumentException("Loan application not found");
        }

        private async Task CreateNotificationAsync(int customerId, string message)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
            if (customer != null)
            {
                var notification = new Notification
                {
                    UserId = customer.UserId,
                    NotificationMessage = message
                };
                await _unitOfWork.Notifications.AddAsync(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
