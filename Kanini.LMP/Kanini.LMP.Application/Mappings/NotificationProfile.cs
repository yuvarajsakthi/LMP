using AutoMapper;
using Kanini.LMP.Database.Entities;
using Kanini.LMP.Database.EntitiesDto;

namespace Kanini.LMP.Application.Mappings
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationDTO, Notification>().ReverseMap();
        }
    }
}
