using AutoMapper;
using EcommerceAPI.Core.DTOs.Carts;
using EcommerceAPI.Core.DTOs.Customers;
using EcommerceAPI.Core.DTOs.Products;
using EcommerceAPI.Entities;

namespace EcommerceAPI.Business.Mapping;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Product
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();

        // Customer
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerRequest, Customer>();
        CreateMap<UpdateCustomerRequest, Customer>();

        // Cart
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name));
        CreateMap<Cart, CartDto>();
    }
}
