using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;

namespace Kanini.LMP.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            #region 2. Customer profile & dashboard
            CreateMap<CustomerCreateDTO, Customer>();
            CreateMap<CustomerUpdateDTO, Customer>();
            CreateMap<Customer, CustomerDto>()
                 .ForMember(d => d.ProfileImageBase64,
                            o => o.MapFrom(s => s.ProfileImage == null ? null : Convert.ToBase64String(s.ProfileImage)));
            CreateMap<Customer, CustomerResponseDTO>();
            CreateMap<Customer, CustomerProfileDto>()
                 .ForMember(d => d.ProfileImageBase64,
                            o => o.MapFrom(s => s.ProfileImage == null ? null : Convert.ToBase64String(s.ProfileImage)));
            //CreateMap<CustomerDashboardDto, CustomerDashboard>().ReverseMap();
            #endregion

            //#region 3. Eligibility & PendingLoan
            //CreateMap<EligibilityScoreDto, EligibilityScore>().ReverseMap();
            //CreateMap<PendingLoanDto, PendingLoan>().ReverseMap();
            //#endregion

            //#region 4. ViewStatus
            //CreateMap<ViewStatusDto, ViewStatus>().ReverseMap();
            //CreateMap<LoanStatusInfoDto, LoanStatusInfo>().ReverseMap();
            //#endregion

            #region 5. EMI Plan
            CreateMap<EMIPlanCreateDTO, EMIPlan>();
            CreateMap<EMIPlanUpdateDTO, EMIPlan>();
            CreateMap<EMIPlan, EMIPlanDTO>().ReverseMap();
            #endregion

            #region 6. Loan Application – Personal
            CreateMap<PersonalLoanApplicationCreateDTO, PersonalLoanApplication>();
            CreateMap<PersonalLoanApplicationDTO, PersonalLoanApplication>().ReverseMap();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationResponseDTO>();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationSummaryDTO>()
                 .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));
            #endregion

            #region 7. Loan Application – Home
            CreateMap<HomeLoanApplicationCreateDTO, HomeLoanApplication>();
            CreateMap<HomeLoanApplicationDTO, HomeLoanApplication>().ReverseMap();
            CreateMap<HomeLoanApplication, HomeLoanApplicationResponseDTO>();
            CreateMap<HomeLoanApplication, HomeLoanApplicationSummaryDTO>()
                 .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));
            #endregion

            #region 8. Loan Application – Vehicle
            CreateMap<VehicleLoanApplicationCreateDTO, VehicleLoanApplication>();
            CreateMap<VehicleLoanApplicationDTO, VehicleLoanApplication>().ReverseMap();
            //CreateMap<VehicleLoanApplication, VehicleLoanApplicationResponseDTO>();
            CreateMap<VehicleLoanApplication, VehicleLoanApplicationSummaryDTO>()
                 .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));
            CreateMap<VehicleLoanApplicationUpdateDTO, VehicleLoanApplication>();
            #endregion

            //#region 9. Vehicle add-ons
            //CreateMap<VehicleInsuranceDetailsDTO, VehicleInsuranceDetails>().ReverseMap();
            //CreateMap<VehicleValuationDTO, VehicleValuation>().ReverseMap();
            //#endregion

        }
    }
}