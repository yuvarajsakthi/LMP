using AutoMapper;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.HomeLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.PersonalLoanApplication;
using Kanini.LMP.Database.EntitiesDto.LoanApplicationEntitiesDto.VehicleLoanApplication;

namespace Kanini.LMP.Application.Mappings
{
    public class LoanApplicationProfile : Profile
    {
        public LoanApplicationProfile()
        {
            CreateMap<PersonalLoanApplicationCreateDTO, PersonalLoanApplication>();
            CreateMap<PersonalLoanApplicationDTO, PersonalLoanApplication>().ReverseMap();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationResponseDTO>();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationSummaryDTO>()
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));

            CreateMap<HomeLoanApplicationCreateDTO, HomeLoanApplication>();
            CreateMap<HomeLoanApplicationDTO, HomeLoanApplication>().ReverseMap();
            CreateMap<HomeLoanApplication, HomeLoanApplicationResponseDTO>();
            CreateMap<HomeLoanApplication, HomeLoanApplicationSummaryDTO>()
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));

            CreateMap<VehicleLoanApplicationCreateDTO, VehicleLoanApplication>();
            CreateMap<VehicleLoanApplicationDTO, VehicleLoanApplication>().ReverseMap();
            CreateMap<VehicleLoanApplication, VehicleLoanApplicationSummaryDTO>()
                .ForMember(d => d.ApplicantName, o => o.MapFrom(s => s.PersonalDetails.FullName));
            CreateMap<VehicleLoanApplicationUpdateDTO, VehicleLoanApplication>();
        }
    }
}
