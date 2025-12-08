using AutoMapper;
using Kanini.LMP.Database.Entities.LoanApplicationEntites;
using Kanini.LMP.Database.EntitiesDtos.LoanApplicationDtos;

namespace Kanini.LMP.Application.Mappings
{
    public class LoanApplicationDTOProfile : Profile
    {
        public LoanApplicationDTOProfile()
        {
            // Personal Loan Application mappings
            CreateMap<PersonalLoanApplication, PersonalLoanApplicationDTO>().ReverseMap();
            CreateMap<PersonalLoanApplication, UpdatePersonalLoanApplicationDTO>();

            // Home Loan Application mappings
            CreateMap<HomeLoanApplication, HomeLoanApplicationDTO>().ReverseMap();
            CreateMap<HomeLoanApplication, UpdateHomeLoanApplicationDTO>();

            // Vehicle Loan Application mappings
            CreateMap<VehicleLoanApplication, VehicleLoanApplicationDTO>().ReverseMap();
            CreateMap<VehicleLoanApplication, UpdateVehicleLoanApplicationDTO>();
        }
    }
}