
using AutoMapper;
using ecommerce_api.DTO;
using ecommerce_api.Models; // Thêm dòng này


public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ReverseMap();
    }
}