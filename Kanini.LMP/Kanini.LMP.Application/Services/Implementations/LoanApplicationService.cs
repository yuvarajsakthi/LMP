using AutoMapper;
using Kanini.LMP.Application.Services.Interfaces;
using Kanini.LMP.Data.Repositories.Interfaces;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.Enums;
using Microsoft.Extensions.Logging;

namespace Kanini.LMP.Application.Services.Implementations
{
    public class LoanApplicationService : ILoanApplicationService
    {
        private readonly ILMPRepository<LoanApplicationBase, int> _loanAppRepository;
        private readonly ILMPRepository<PersonalLoanApplication, int> _personalLoanRepository;
        private readonly ILMPRepository<HomeLoanApplication, int> _homeLoanRepository;
        private readonly ILMPRepository<VehicleLoanApplication, int> _vehicleLoanRepository;
        private readonly ILMPRepository<LoanApplicant, int> _loanApplicantRepository;
        private readonly IEligibilityService _eligibilityService;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanApplicationService> _logger;

        public LoanApplicationService(
            ILMPRepository<LoanApplicationBase, int> loanAppRepository,
            ILMPRepository<PersonalLoanApplication, int> personalLoanRepository,
            ILMPRepository<HomeLoanApplication, int> homeLoanRepository,
            ILMPRepository<VehicleLoanApplication, int> vehicleLoanRepository,
            ILMPRepository<LoanApplicant, int> loanApplicantRepository,
            IEligibilityService eligibilityService,
            IMapper mapper,
            ILogger<LoanApplicationService> logger)
        {
            _loanAppRepository = loanAppRepository;
            _personalLoanRepository = personalLoanRepository;
            _homeLoanRepository = homeLoanRepository;
            _vehicleLoanRepository = vehicleLoanRepository;
            _loanApplicantRepository = loanApplicantRepository;
            _eligibilityService = eligibilityService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PersonalLoanApplicationDTO> CreatePersonalLoanAsync(PersonalLoanApplicationCreateDTO dto, int customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, (int)LoanType.Personal);
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");
            
            var entity = _mapper.Map<PersonalLoanApplication>(dto);
            entity.CustomerId = customerId;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _personalLoanRepository.AddAsync(entity);
            return _mapper.Map<PersonalLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetAllPersonalLoansAsync()
        {
            var loans = await _personalLoanRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<PersonalLoanApplicationDTO>>(loans);
        }

        public async Task<PersonalLoanApplicationDTO?> GetPersonalLoanByIdAsync(int id)
        {
            var loan = await _personalLoanRepository.GetByIdAsync(id);
            return loan == null ? null : _mapper.Map<PersonalLoanApplicationDTO>(loan);
        }

        public async Task<HomeLoanApplicationDTO> CreateHomeLoanAsync(HomeLoanApplicationCreateDTO dto, int customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, (int)LoanType.Home);
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");
            
            var entity = _mapper.Map<HomeLoanApplication>(dto);
            entity.CustomerId = customerId;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _homeLoanRepository.AddAsync(entity);
            return _mapper.Map<HomeLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<HomeLoanApplicationDTO>> GetAllHomeLoansAsync()
        {
            var loans = await _homeLoanRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<HomeLoanApplicationDTO>>(loans);
        }

        public async Task<HomeLoanApplicationDTO?> GetHomeLoanByIdAsync(int id)
        {
            var loan = await _homeLoanRepository.GetByIdAsync(id);
            return loan == null ? null : _mapper.Map<HomeLoanApplicationDTO>(loan);
        }

        public async Task<VehicleLoanApplicationDTO> CreateVehicleLoanAsync(VehicleLoanApplicationCreateDTO dto, int customerId)
        {
            var eligibility = await _eligibilityService.CalculateEligibilityAsync(customerId, (int)LoanType.Vehicle);
            if (eligibility.EligibilityScore < 50)
                throw new InvalidOperationException($"Customer not eligible. Score: {eligibility.EligibilityScore}");
            
            var entity = _mapper.Map<VehicleLoanApplication>(dto);
            entity.CustomerId = customerId;
            entity.Status = ApplicationStatus.Submitted;
            var created = await _vehicleLoanRepository.AddAsync(entity);
            return _mapper.Map<VehicleLoanApplicationDTO>(created);
        }

        public async Task<IReadOnlyList<VehicleLoanApplicationDTO>> GetAllVehicleLoansAsync()
        {
            var loans = await _vehicleLoanRepository.GetAllAsync();
            return _mapper.Map<IReadOnlyList<VehicleLoanApplicationDTO>>(loans);
        }

        public async Task<VehicleLoanApplicationDTO?> GetVehicleLoanByIdAsync(int id)
        {
            var loan = await _vehicleLoanRepository.GetByIdAsync(id);
            return loan == null ? null : _mapper.Map<VehicleLoanApplicationDTO>(loan);
        }

        public async Task<IReadOnlyList<PersonalLoanApplicationDTO>> GetLoansByStatusAsync(ApplicationStatus status)
        {
            var loans = await _personalLoanRepository.GetAllAsync(l => l.Status == status);
            return _mapper.Map<IReadOnlyList<PersonalLoanApplicationDTO>>(loans);
        }

        public async Task<PersonalLoanApplicationDTO> UpdateLoanStatusAsync(int id, ApplicationStatus status)
        {
            var loan = await _loanAppRepository.GetByIdAsync(id);
            if (loan == null) throw new KeyNotFoundException($"Loan application {id} not found");
            loan.Status = status;
            var updated = await _loanAppRepository.UpdateAsync(loan);
            return _mapper.Map<PersonalLoanApplicationDTO>(updated);
        }

        public async Task<int> UploadDocumentAsync(int loanApplicationBaseId, int userId, string documentName, string documentType, byte[] documentData)
        {
            var loan = await _loanAppRepository.GetByIdAsync(loanApplicationBaseId);
            if (loan == null) throw new KeyNotFoundException($"Loan application {loanApplicationBaseId} not found");
            return loanApplicationBaseId;
        }

        public async Task<IReadOnlyList<int>> GetApplicantsByLoanAsync(int loanApplicationBaseId)
        {
            var applicants = await _loanApplicantRepository.GetAllAsync(a => a.LoanApplicationBaseId == loanApplicationBaseId);
            return applicants.Select(a => a.CustomerId).ToList();
        }

        public async Task<IEnumerable<dynamic>> GetRecentApplicationsAsync(int customerId, int count)
        {
            var loans = await _loanAppRepository.GetAllAsync(l => l.CustomerId == customerId);
            return loans.OrderByDescending(l => l.SubmissionDate).Take(count).Select(l => new { l.LoanApplicationBaseId, l.Status, l.SubmissionDate });
        }

        public async Task<IEnumerable<dynamic>> GetCustomerApplicationsAsync(int customerId)
        {
            var loans = await _loanAppRepository.GetAllAsync(l => l.CustomerId == customerId);
            return loans.Select(l => new { l.LoanApplicationBaseId, l.Status, l.SubmissionDate });
        }
    }
}


