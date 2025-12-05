using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.Entities.CustomerEntities;
using Kanini.LMP.Database.EntitiesDtos.Authentication;
using Kanini.LMP.Database.EntitiesDtos.UserDtos;

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
            CreateMap<CustomerRegistrationDTO, User>();
            CreateMap<CustomerRegistrationDTO, Customer>();
        }
    }
}