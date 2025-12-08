using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Mappings
{
    public class FaqProfile : Profile
    {
        public FaqProfile()
        {
            CreateMap<FaqDTO, Faq>()
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ReverseMap()
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore());
        }
    }
}