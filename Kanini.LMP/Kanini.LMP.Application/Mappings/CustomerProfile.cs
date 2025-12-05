using AutoMapper;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.Customer;
using Kanini.LMP.Database.EntitiesDto.CustomerEntitiesDto.CustomerBasicDto.EMIPlan;

namespace Kanini.LMP.Application.Mappings
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {

            CreateMap<CustomerCreateDTO, Customer>()
                .ForMember(d => d.EligibilityScore, o => o.MapFrom(s => s.CreditScore));
            CreateMap<CustomerUpdateDTO, Customer>();
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.CreditScore, o => o.MapFrom(s => s.EligibilityScore))
                .ForMember(d => d.Age, o => o.MapFrom(s => s.Age))
                .ForMember(d => d.ProfileImageBase64, o => o.MapFrom(s => s.ProfileImage == null ? null : Convert.ToBase64String(s.ProfileImage)));
            CreateMap<Customer, CustomerResponseDTO>();
            CreateMap<Customer, CustomerProfileDto>()
                .ForMember(d => d.ProfileImageBase64, o => o.MapFrom(s => s.ProfileImage == null ? null : Convert.ToBase64String(s.ProfileImage)));

            CreateMap<EMIPlanCreateDTO, EMIPlan>();
            CreateMap<EMIPlanUpdateDTO, EMIPlan>();
            CreateMap<EMIPlan, EMIPlanDTO>().ReverseMap();

        }
    }
}