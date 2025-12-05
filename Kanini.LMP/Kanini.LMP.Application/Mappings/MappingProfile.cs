using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.Entities.LoanProductEntities.CommonLoanProductEntities;
using Kanini.LMP.Database.Entities.ManagerEntities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDtos.Authentication;

using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.AppliedLoans;
using Kanini.LMP.Database.EntitiesDto.ManagerEntitiesDto.ManagerDashboardDto.Manager.NewFolderBasicDto;
using Kanini.LMP.Database.EntitiesDto.PaymentTransaction;
using Kanini.LMP.Database.EntitiesDtos.DocumentDtos;
using Kanini.LMP.Database.Enums;

namespace Kanini.LMP.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ReverseMap()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
            
            CreateMap<CustomerRegistrationDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => UserRoles.Customer))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.Pending));

            // Customer mappings
            CreateMap<Customer, CustomerDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)))
                .ReverseMap()
                .ForMember(dest => dest.Age, opt => opt.Ignore());
            
            CreateMap<CustomerRegistrationDTO, Customer>()
                .ForMember(dest => dest.AadhaarNumber, opt => opt.MapFrom(src => string.Empty));

            // Notification mappings
            CreateMap<Notification, NotificationDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ReverseMap()
                .ForMember(dest => dest.User, opt => opt.Ignore());

            // Loan Application mappings
            CreateMap<LoanApplicationBase, AppliedLoanListDto>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.LoanApplicationBaseId))
                .ForMember(dest => dest.ApplicationNumber, opt => opt.MapFrom(src => $"APP-{src.LoanApplicationBaseId}"))
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedLoanAmount, opt => opt.Ignore());

            // Payment Transaction mappings
            CreateMap<PaymentTransaction, PaymentTransactionDTO>().ReverseMap();
            CreateMap<PaymentTransactionCreateDTO, PaymentTransaction>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // EMI Plan mappings
            CreateMap<EMIPlan, EMIPlanDto>().ReverseMap();
            CreateMap<EMIPlan, EMIPlanDTO>().ReverseMap();
            CreateMap<EMIPlanCreateDTO, EMIPlan>();

            // Loan Account mappings
            CreateMap<LoanAccount, LoanAccountDto>().ReverseMap();

            // Document mappings
            CreateMap<DocumentUpload, DocumentUploadDto>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.VerificationNotes, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedBy, opt => opt.Ignore())
                .ReverseMap();

            // Workflow mappings
            CreateMap<LoanOriginationWorkflow, WorkflowStepDto>().ReverseMap();

            // Loan Application mappings
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationDTO>()
                .ForMember(dest => dest.LoanDetails, opt => opt.Ignore())
                .ForMember(dest => dest.PersonalDetails, opt => opt.Ignore())
                .ForMember(dest => dest.AddressInformation, opt => opt.Ignore())
                .ForMember(dest => dest.FamilyEmergencyDetails, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentDetails, opt => opt.Ignore())
                .ForMember(dest => dest.FinancialInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Declaration, opt => opt.Ignore());

            CreateMap<HomeLoanApplication, HomeLoanApplicationDTO>()
                .ForMember(dest => dest.LoanDetails, opt => opt.Ignore())
                .ForMember(dest => dest.PersonalDetails, opt => opt.Ignore())
                .ForMember(dest => dest.AddressInformation, opt => opt.Ignore())
                .ForMember(dest => dest.FamilyEmergencyDetails, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentDetails, opt => opt.Ignore())
                .ForMember(dest => dest.FinancialInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Declaration, opt => opt.Ignore())
                .ForMember(dest => dest.BuilderInformation, opt => opt.Ignore())
                .ForMember(dest => dest.HomeLoanDetails, opt => opt.Ignore())
                .ForMember(dest => dest.PropertyDetails, opt => opt.Ignore());

            CreateMap<VehicleLoanApplication, VehicleLoanApplicationDTO>()
                .ForMember(dest => dest.LoanDetails, opt => opt.Ignore())
                .ForMember(dest => dest.PersonalDetails, opt => opt.Ignore())
                .ForMember(dest => dest.AddressInformation, opt => opt.Ignore())
                .ForMember(dest => dest.FamilyEmergencyDetails, opt => opt.Ignore())
                .ForMember(dest => dest.EmploymentDetails, opt => opt.Ignore())
                .ForMember(dest => dest.FinancialInformation, opt => opt.Ignore())
                .ForMember(dest => dest.Declaration, opt => opt.Ignore())
                .ForMember(dest => dest.DealerInformation, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleLoanDetails, opt => opt.Ignore())
                .ForMember(dest => dest.VehicleInformation, opt => opt.Ignore());

            // Manager Analytics mappings
            CreateMap<LoanApplicationBase, AppliedLoanListDto>()
                .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.LoanApplicationBaseId))
                .ForMember(dest => dest.ApplicationNumber, opt => opt.MapFrom(src => $"APP-{src.LoanApplicationBaseId}"))
                .ForMember(dest => dest.SubmissionDate, opt => opt.MapFrom(src => src.SubmissionDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(dest => dest.CustomerFullName, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.RequestedLoanAmount, opt => opt.Ignore());

            // Manager Workflow mappings
            CreateMap<LoanOriginationWorkflow, LoanOriginationWorkflowDTO>()
                .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.StepName.ToString()))
                .ForMember(dest => dest.StepStatus, opt => opt.MapFrom(src => src.StepStatus.ToString()));

            CreateMap<LoanOriginationWorkflow, WorkflowStepDto>()
                .ForMember(dest => dest.StepName, opt => opt.MapFrom(src => src.StepName.ToString()))
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.StepStatus == StepStatus.Completed))
                .ForMember(dest => dest.StatusIndicator, opt => opt.MapFrom(src => src.StepStatus.ToString()));
        }

        private static int CalculateAge(DateOnly dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.ToDateTime(TimeOnly.MinValue) > today.AddYears(-age))
                age--;
            return age;
        }
    }

    // Additional DTOs for mapping
    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
    }

    public class EMIPlanDto
    {
        public int EMIPlanId { get; set; }
        public int LoanAccountId { get; set; }
        public decimal EMIAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class LoanAccountDto
    {
        public int LoanAccountId { get; set; }
        public int LoanApplicationBaseId { get; set; }
        public decimal TotalLoanAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string CurrentPaymentStatus { get; set; } = string.Empty;
    }

    public class DocumentDto
    {
        public int DocumentLinkId { get; set; }
        public int LoanApplicationBaseId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}