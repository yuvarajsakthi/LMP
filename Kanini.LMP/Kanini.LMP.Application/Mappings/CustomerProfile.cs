using AutoMapper;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.CustomerDtos;
using Kanini.LMP.Database.EntitiesDtos.EMIPlanDtos;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;

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
                .ForMember(d => d.EmploymentType, o => o.MapFrom(s => s.Occupation));

            CreateMap<EMIPlanCreateDTO, EMIPlan>();
            CreateMap<EMIPlanUpdateDTO, EMIPlan>();
            CreateMap<EMIPlan, EMIPlanDTO>().ReverseMap();
            CreateMap<EMIPlan, EMIPlanResponseDTO>();
        }
    }
}