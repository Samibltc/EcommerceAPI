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
        // AutoMapper (config-action overload for broader compatibility)
        services.AddAutoMapper(cfg =>
        {
            // optional global config
            cfg.AllowNullCollections = true;
        }, typeof(AutoMapperProfiles).Assembly);

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICartService, CartService>();

        return services;
    }
}
