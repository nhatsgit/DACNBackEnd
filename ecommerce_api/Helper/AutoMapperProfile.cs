
using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models; // Thêm dòng này


public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ReverseMap();
        CreateMap<Category, CategoryDTO>()
            .ReverseMap();
        CreateMap<Brand, BrandDTO>()
            .ReverseMap();
        CreateMap<Order, OrderDTO>()
            .ReverseMap();
        CreateMap<OrderDetail, OrderDetailDTO>()
            .ReverseMap();
        CreateMap<ApplicationUser, UserDTO>()
            .ReverseMap();
    }
}