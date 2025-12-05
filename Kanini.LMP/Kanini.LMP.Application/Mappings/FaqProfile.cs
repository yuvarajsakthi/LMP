using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Mappings
{
    public class FaqProfile : Profile
    {
        public FaqProfile()
        {
            CreateMap<FaqDTO, Faq>().ReverseMap();
        }
    }
}