namespace Wasl.Application.Mapping
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notifications, NotificationResponse>();
        }
    }
}
