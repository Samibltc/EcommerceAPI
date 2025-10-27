using EcommerceAPI.Filters;
using FluentValidation;
using EcommerceAPI.Business;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceAPI.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers(o => o.Filters.Add<ValidationFilter>());

        services.AddEndpointsApiExplorer();
        services.AddProblemDetails(); // optional

        services.AddValidatorsFromAssembly(typeof(BusinessAssemblyMarker).Assembly);

        return services;
    }
}
