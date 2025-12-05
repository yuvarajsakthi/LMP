using AutoMapper;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos;

namespace Kanini.LMP.Application.Mappings
{
    public class LoanApplicationDTOProfile : Profile
    {
        public LoanApplicationDTOProfile()
        {
            // Base Loan Application mappings
            CreateMap<LoanApplicationBaseCreateDTO, LoanApplicationBase>();
            CreateMap<LoanApplicationBaseUpdateDTO, LoanApplicationBase>();
            CreateMap<LoanApplicationBase, LoanApplicationBaseDTO>().ReverseMap();
            CreateMap<LoanApplicationBase, LoanApplicationBaseResponseDTO>();

            // Personal Loan Application mappings
            CreateMap<PersonalLoanApplicationCreateDTO, PersonalLoanApplication>();
            CreateMap<PersonalLoanApplicationUpdateDTO, PersonalLoanApplication>();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationDTO>().ReverseMap();
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationResponseDTO>();

            // Home Loan Application mappings
            CreateMap<HomeLoanApplicationCreateDTO, HomeLoanApplication>();
            CreateMap<HomeLoanApplicationUpdateDTO, HomeLoanApplication>();
            CreateMap<HomeLoanApplication, HomeLoanApplicationDTO>().ReverseMap();
            CreateMap<HomeLoanApplication, HomeLoanApplicationResponseDTO>();

            // Vehicle Loan Application mappings
            CreateMap<VehicleLoanApplicationCreateDTO, VehicleLoanApplication>();
            CreateMap<VehicleLoanApplicationUpdateDTO, VehicleLoanApplication>();
            CreateMap<VehicleLoanApplication, VehicleLoanApplicationDTO>().ReverseMap();
            CreateMap<VehicleLoanApplication, VehicleLoanApplicationResponseDTO>();
        }
    }
}