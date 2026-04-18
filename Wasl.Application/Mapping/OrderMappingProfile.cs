namespace Wasl.Application.Mapping
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<OrderItem, OrderItemResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.Ignore());
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.StoreOwnerId, opt => opt.MapFrom(src => src.StoreOwnerId.ToString()))
                .ForMember(dest => dest.StoreOwnerName, opt => opt.MapFrom(src => src.StoreOwner != null ? src.StoreOwner.BusinessName : null))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId.ToString()))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.BusinessName : null))
                .ForMember(dest => dest.SupplierCity, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.City : null))
                .ForMember(dest => dest.SupplierPhone, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.PhoneNumber : null))
                .ForMember(dest => dest.SupplierEmail, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Email : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(dest => dest.TrackingNumber, opt => opt.MapFrom(src => src.TrackingNumber))
                .ForMember(dest => dest.ExpectedDeliveryDate, opt => opt.MapFrom(src => src.ExpectedDeliveryDate));
        }
    }
}
