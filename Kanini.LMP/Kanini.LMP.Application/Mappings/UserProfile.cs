using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;
using Kanini.LMP.Database.EntitiesDto.User;

namespace Kanini.LMP.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDTO, User>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, UserResponseDTO>();
        }
    }
}
