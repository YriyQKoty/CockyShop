using AutoMapper;
using CockyShop.Models.App;
using CockyShop.Models.DTO;
using CockyShop.Models.Enums;
using CockyShop.Models.Identity;

namespace CockyShop.Mapper
{
    public class MapperConfigProfile : Profile
    {
        public MapperConfigProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDetails, OrderDetailsDto>()
                .ForSourceMember(o => o.Order, 
                    opt =>
                opt.DoNotValidate())
                .ForSourceMember(o => o.OrderId, 
                    opt => 
                opt.DoNotValidate());
            CreateMap<OrderStatus, OrderStatusDto>();
            CreateMap<OrderedProduct, OrderedProductDto>();
            CreateMap<AppUser, AppUserDto>();

        }
    }
}