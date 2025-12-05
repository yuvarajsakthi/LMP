using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>().ReverseMap();
        }
    }
}