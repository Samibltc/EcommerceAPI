using AutoMapper;
using EcommerceAPI.Business.Abstract;
using EcommerceAPI.Business.Concrete;
using EcommerceAPI.Business.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceAPI.Business.DependencyInjection;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => { cfg.AllowNullCollections = true; }, typeof(AutoMapperProfiles).Assembly);

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IAccountService, AccountService>();

        return services;
    }
}
