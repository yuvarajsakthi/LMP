using AutoMapper;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;

namespace Kanini.LMP.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerCreateDTO, Customer>();
            CreateMap<CustomerUpdateDTO, Customer>();
            CreateMap<Customer, CustomerDTO>()
                .ForMember(d => d.Age, o => o.MapFrom(s => s.Age));
            CreateMap<Customer, CustomerResponseDTO>()
                .ForMember(d => d.Age, o => o.MapFrom(s => s.Age));

            CreateMap<Customer, EligibilityScoreDto>()
                .ForMember(d => d.CreditScore, o => o.MapFrom(s => (int)s.EligibilityScore))
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.Occupation))
                .ForMember(d => d.MonthlyIncome, o => o.MapFrom(s => s.AnnualIncome / 12));

            CreateMap<CustomerDTO, EligibilityScoreDto>()
                .ForMember(d => d.CreditScore, o => o.MapFrom(s => (int)s.EligibilityScore))
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.Occupation))
                .ForMember(d => d.MonthlyIncome, o => o.MapFrom(s => s.AnnualIncome / 12));

            CreateMap<EligibilityProfileRequest, Customer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<EMIPlanCreateDTO, EMIPlan>();
            CreateMap<EMIPlanUpdateDTO, EMIPlan>();
            CreateMap<EMIPlan, EMIPlanDTO>().ReverseMap();
            CreateMap<EMIPlan, EMIPlanResponseDTO>();
        }
    }
}